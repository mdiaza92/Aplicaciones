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
using WS_HyJ.Models.Responses.Receipt;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Repository.DAO
{
    public class ReceiptDAO : IReceiptDAO
    {
        private readonly MongoDbContext _context = null;
        private readonly IOrderDAO _orderDAO;
        private readonly IKardexDAO _kardexDAO;
        private readonly IPaymentMethodDAO _paymentMethodDAO;

        public ReceiptDAO(IOptions<Settings> settings, IOrderDAO orderDAO, IKardexDAO kardexDAO, IPaymentMethodDAO paymentMethodDAO)
        {
            _context = new MongoDbContext(settings);
            _orderDAO = orderDAO;
            _kardexDAO = kardexDAO;
            _paymentMethodDAO = paymentMethodDAO;
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveReceiptResponse> Add(ReceiptRequest model)
        {
            try
            {
                ReceiptEntity p = new ReceiptEntity()
                {
                    Estado = EReceiptStatus.Pendiente,
                    NumeroFactura = model.NumeroFactura,
                    Tipo = model.Tipo,
                    FechaFactura = model.FechaFactura.Date
                };

                var exist = await ExistFactura(p);

                if (!exist)
                {
                    //Registramos el recibo en el sistema
                    await _context.Receipt.InsertOneAsync(p);

                    //Actualizamos el pedido elejido con el ID de la factura generada
                    var response = await _orderDAO.UpdateReceipt(model.IdOrden, p.Id);

                    if (response.Success)
                        //Actualizamos el estado del pedido ya que se actualizó el número de factura
                        await _orderDAO.UpdateStatus(model.IdOrden);

                    return new SaveReceiptResponse(new ReceiptResponse() { Id = p.Id });
                }
                else
                    return new SaveReceiptResponse($"Ya existe el recibo en el sistema.", HttpStatusCode.Ambiguous);

            }
            catch (AppException ex)
            {
                return new SaveReceiptResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private async Task<bool> ExistFactura(ReceiptEntity p)
        {
            try
            {
                return await _context.Receipt.Find(_ => _.NumeroFactura == p.NumeroFactura).AnyAsync();
            }
            catch (AppException ex)
            {
                return false;
                throw ex;
            }
        }

        public async Task<SaveReceiptResponse> Delete(string id)
        {
            try
            {
                await ReducirStock(id);

                DeleteResult actionResult
                    = await _context.Receipt.DeleteOneAsync(
                        Builders<ReceiptEntity>.Filter.Eq(x => x.Id, id));

                var result = actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;

                string response = result ? $"Se eliminó correctamente el registro con ID {id}." : "No fue posible eliminar el registro.";

                if (result)
                    return new SaveReceiptResponse(true, response, new ReceiptResponse() { Id = id });
                else
                    return new SaveReceiptResponse(response);
            }
            catch (AppException ex)
            {
                return new SaveReceiptResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private async Task ReducirStock(string id)
        {
            var order = await ObtenerPedidoPorComprobante(id);

            await _kardexDAO.ReduceStockByOrder(order.Id);

            await _orderDAO.UpdateReceipt(order.Id, null);
            await _orderDAO.ValidateOrder(order.Id, false);
            await _orderDAO.UpdateStatus(order.Id, EOrderStatus.Pendiente);

            //eliminar tipo de pago en base al comprobante
            await EliminarComprobante(id);
        }

        private async Task<bool> EliminarComprobante(string id)
        {
            DeleteResult actionResult
                    = await _context.PaymentMethod.DeleteOneAsync(
                        Builders<PaymentMethodEntity>.Filter.Eq(x => x.IdReceipt, id));

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }

        public async Task<SaveReceiptResponse> Get(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                var model = await _context.Receipt
                                .Find(_ => _.Id == id
                                        || _.InternalId == internalId)
                                .FirstOrDefaultAsync();

                if (model != null)
                {
                    ReceiptResponse response = new ReceiptResponse()
                    {
                        CreatedOn = model.CreatedOn,
                        Estado = model.Estado.ToDescriptionString(),
                        Id = model.Id,
                        NumeroFactura = model.NumeroFactura,
                        Tipo = model.Tipo.ToDescriptionString(),
                        UpdatedOn = model.UpdatedOn,
                        Pedido = await ObtenerPedidoPorComprobante(model.Id),
                        TieneMedioPago = await ValidarComprobanteYMedioPago(model),
                        FechaFactura = model.FechaFactura.Date
                    };

                    return new SaveReceiptResponse(response);
                }
                else
                    return new SaveReceiptResponse($"No se encontró el elemento con ID {id}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveReceiptResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        private async Task<bool> ValidarComprobanteYMedioPago(ReceiptEntity model)
        {
            var PT = await _paymentMethodDAO.GetPaymentTypeByReceipt(model.Id);

            //validar si el estado del comprobante es "conforme" y no tiene un tipo de pago confirmado
            return (model.Estado == EReceiptStatus.Conforme && PT != null) ? true : false;
        }

        /// <summary>
        /// Obtener el pedido en base al comprobante
        /// </summary>
        /// <param name="id">ID del recibo o comprobante</param>
        private async Task<OrderResponse> ObtenerPedidoPorComprobante(string id)
        {
            var response = await _orderDAO.GetOrderByReceipt(id);

            if (response.Success)
                return response._orderEntity;
            else
                return null;
        }

        public async Task<IEnumerable<ReceiptResponse>> GetAll()
        {
            try
            {
                List<ReceiptResponse> all = new List<ReceiptResponse>();

                var lista = await _context.Receipt.Find(_ => true)
                    .ToListAsync();

                foreach (var item in lista)
                {
                    all.Add((await Get(item.Id))._receiptResponse);
                }

                return all;
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        /// <summary>
        /// Actualizar el estado del comprobante
        /// </summary>
        /// <param name="idpedido">ID del pedido</param>
        /// <returns></returns>
        public async Task<bool> UpdateStatusByOrder(string idpedido)
        {
            var model = await _context.Order
                                .Find(_ => _.Id == idpedido)
                                .FirstOrDefaultAsync();

            var filter = Builders<ReceiptEntity>.Filter.Eq(s => s.Id, model.IdRecibo);
            var update = Builders<ReceiptEntity>.Update
                .Set(s => s.Estado, EReceiptStatus.Conforme);

            try
            {
                UpdateResult actionResult
                    = await _context.Receipt.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                return response;
            }
            catch (AppException ex)
            {
                throw ex;
            }
        }

        public Task<SaveReceiptResponse> Update(string id, ReceiptRequest model)
        {
            throw new NotImplementedException();
        }
    }
}
