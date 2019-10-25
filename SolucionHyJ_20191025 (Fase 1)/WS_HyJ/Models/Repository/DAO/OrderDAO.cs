using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Helpers;
using WS_HyJ.Interfaces;
using WS_HyJ.Models.Enum;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Order;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Repository.DAO
{
    public class OrderDAO : IOrderDAO
    {
        private readonly MongoDbContext _context = null;
        private readonly IProductDAO _productDAO;
        private readonly IConfiguration _configuration;

        public OrderDAO(IOptions<Settings> settings, IProductDAO productDAO, IConfiguration configuration)
        {
            _context = new MongoDbContext(settings);
            _productDAO = productDAO;
            _configuration = configuration;
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveOrderResponse> Add(OrderRequest model)
        {
            SaveOrderResponse response = null;

            try
            {
                List<OrderDetailEntity> orderDetails = new List<OrderDetailEntity>();

                List<string> responseIdProducto = new List<string>();

                foreach (var item in model.DetallePedido)
                {
                    var Producto = await _productDAO.Get(item.IdProducto);

                    if (Producto.Success)
                    {
                        orderDetails.Add(new OrderDetailEntity()
                        {
                            Cantidad = item.Cantidad,
                            IdProducto = item.IdProducto,
                            SubTotal = CalculateSubTotal(item.Cantidad, Producto._productEntity)
                        });
                    }
                    else
                        responseIdProducto.Add(item.IdProducto);
                }

                if (orderDetails.Any())
                {
                    OrderEntity order = new OrderEntity()
                    {
                        DetallePedido = orderDetails,
                        Estado = Enums.EOrderStatus.Pendiente,
                        IdUsuario = model.IdUsuario,
                        Total = CalculateTotal(orderDetails),
                        Numero = await CalculateNumber()
                    };

                    order.Codigo = $"O-{order.Numero.ToString("D5")}";

                    await _context.Order.InsertOneAsync(order);

                    string responseText = null;

                    if (responseIdProducto.Any())
                        responseText = $"Existen productos que no han sido encontrados: '{string.Join(", ", responseIdProducto.ToArray())}'";

                    response = new SaveOrderResponse(true, $"Se generó la orden {order.Codigo}.{(!string.IsNullOrEmpty(responseText) ? $" {responseText}." : string.Empty )}", HttpStatusCode.Created, new OrderResponse() { Id = order.Id });
                }
                else
                    response = new SaveOrderResponse("No se encontró ningún detalle del pedido por lo que no se generó la orden.");
            }
            catch (AppException ex)
            {
                response = new SaveOrderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }

            return response;
        }

        private async Task<int> CalculateNumber()
        {
            var list = await GetAll();

            return list.Any() ? list.ToList().Max(x => x.Numero) + 1 : 1;
        }

        private decimal? CalculateTotal(List<OrderDetailEntity> orderDetails)
        {
            decimal? total = 0;

            if (!orderDetails.Any()) total = null;

            foreach (var item in orderDetails)
            {
                total += item.SubTotal;
            }

            if (total.HasValue)
            {
                string IGV = _configuration["IGV"];

                var response = decimal.TryParse(IGV, out decimal result);

                total = total.Value + (total.Value * result);

                return decimal.Round(total.Value, 2, MidpointRounding.AwayFromZero);
            }
            else
                return total;
        }

        private decimal CalculateSubTotal(int cantidad, ProductEntity Producto)
        {
            decimal subtotal = 0;

            if (Producto != null)
                subtotal = decimal.Round(Producto.PrecioUnitario * cantidad, 2, MidpointRounding.AwayFromZero);

            return subtotal;
        }

        public async Task<SaveOrderResponse> Delete(string id)
        {
            try
            {
                DeleteResult actionResult
                    = await _context.Order.DeleteOneAsync(
                        Builders<OrderEntity>.Filter.Eq("Id", id));

                var response = actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;

                if (response)
                    return new SaveOrderResponse(new OrderResponse() { Id = id });
                else
                    return new SaveOrderResponse($"No se pudo eliminar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveOrderResponse> Get(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                var model = await _context.Order
                                .Find(_ => _.Id == id
                                        || _.InternalId == internalId)
                                .FirstOrDefaultAsync();

                List<OrderDetailResponse> responseOD = new List<OrderDetailResponse>();

                foreach (var item in model.DetallePedido)
                {
                    responseOD.Add(new OrderDetailResponse()
                    {
                        Cantidad = item.Cantidad,
                        Id = item.Id,
                        Producto = (await _productDAO.Get(item.IdProducto))._productEntity,
                        SubTotal = item.SubTotal
                    });
                }

                OrderResponse response = new OrderResponse()
                {
                    Codigo = model.Codigo,
                    CreatedOn = model.CreatedOn,
                    Id = model.Id,
                    Numero = model.Numero,
                    Estado = model.Estado.ToDescriptionString(),
                    IdUsuario = model.IdUsuario,
                    Total = model.Total,
                    UpdatedOn = model.UpdatedOn,
                    DetallePedido = responseOD,
                    NumeroFactura = await ObtenerNumeroFactura(model.IdRecibo),
                    Validado = model.Validado
                };

                return model != null ? new SaveOrderResponse(response) : new SaveOrderResponse($"No se encontró el elemento con ID {id}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Obtener el número de factura de una orden.
        /// </summary>
        /// <param name="idRecibo">ID del recibo</param>
        private async Task<string> ObtenerNumeroFactura(string idRecibo)
        {
            var model = await _context.Receipt
                                .Find(_ => _.Id == idRecibo)
                                .FirstOrDefaultAsync();

            return model != null ? model.NumeroFactura : null;
        }

        public async Task<IEnumerable<OrderResponse>> GetAll()
        {
            try
            {
                List<OrderResponse> responseO = new List<OrderResponse>();
                List<OrderDetailResponse> responseOD = new List<OrderDetailResponse>();

                var list = await _context.Order.Find(_ => true).ToListAsync();

                foreach (var itemOrder in list)
                {
                    responseO.Add((await Get(itemOrder.Id))._orderEntity);
                }

                return responseO;
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<SaveOrderResponse> Update(OrderEntity model)
        {
            var filter = Builders<OrderEntity>.Filter.Eq(s => s.Id, model.Id);
            var update = Builders<OrderEntity>.Update
                .Set(s => s.DetallePedido, model.DetallePedido);

            try
            {
                UpdateResult actionResult
                    = await _context.Order.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveOrderResponse(new OrderResponse() { Id = model.Id });
                else
                    return new SaveOrderResponse($"No se pudo actualizar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveOrderResponse> UpdateStatus(string id, EOrderStatus status)
        {
            var filter = Builders<OrderEntity>.Filter.Eq(s => s.Id, id);
            var update = Builders<OrderEntity>.Update
                .Set(s => s.Estado, status);

            try
            {
                UpdateResult actionResult
                    = await _context.Order.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveOrderResponse(new OrderResponse() { Id = id });
                else
                    return new SaveOrderResponse($"No se pudo actualizar el estado de la órden.");
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Validar el pedido en base al comprobante
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="validate"></param>
        public async Task<bool> ValidateOrder(string id, bool validate)
        {
            var filter = Builders<OrderEntity>.Filter.Eq(s => s.Id, id);
            var update = Builders<OrderEntity>.Update
                .Set(s => s.Validado, validate);

            try
            {
                UpdateResult actionResult
                    = await _context.Order.UpdateOneAsync(filter, update);

                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (AppException ex)
            {
                throw ex;
            }
        }

        public async Task<SaveOrderResponse> UpdateReceipt(string idOrder, string idReceipt)
        {
            var filter = Builders<OrderEntity>.Filter.Eq(s => s.Id, idOrder);
            var update = Builders<OrderEntity>.Update
                .Set(s => s.IdRecibo, idReceipt);

            try
            {
                UpdateResult actionResult
                    = await _context.Order.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveOrderResponse(new OrderResponse() { Id = idOrder });
                else
                    return new SaveOrderResponse($"No se pudo actualizar el elemento con el número de recbido.");
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveOrderResponse> GetOrderByReceipt(string idReceipt)
        {
            try
            {
                var model = await _context.Order
                                .Find(_ => _.IdRecibo == idReceipt)
                                .FirstOrDefaultAsync();

                if (model == null)
                    return new SaveOrderResponse("No se encontró el pedido asociado.");

                var response = await Get(model.Id);

                return response != null ? new SaveOrderResponse(response._orderEntity) : new SaveOrderResponse($"No se encontró la orden asociada al recibo con ID {idReceipt}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        public async Task<SaveOrderResponse> UpdateOrderDetails(string idOrder, List<OrderDetailRequest> orderDetails)
        {
            List<OrderDetailEntity> orders = new List<OrderDetailEntity>();

            foreach (var item in orderDetails)
            {
                var Producto = await _productDAO.Get(item.IdProducto);

                orders.Add(new OrderDetailEntity() {
                    Cantidad = item.Cantidad,
                    IdProducto = item.IdProducto,
                    SubTotal = CalculateSubTotal(item.Cantidad, Producto._productEntity)
                });
            }

            //Actualizar el pedido
            var filter = Builders<OrderEntity>.Filter.Eq(s => s.Id, idOrder);
            var update = Builders<OrderEntity>.Update
                .Set(s => s.DetallePedido, orders)
                .Set(s => s.Total, CalculateTotal(orders));

            try
            {
                UpdateResult actionResult
                    = await _context.Order.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveOrderResponse(new OrderResponse() { Id = idOrder });
                else
                    return new SaveOrderResponse($"No se pudo actualizar el detalle de la órden.");
            }
            catch (AppException ex)
            {
                return new SaveOrderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
