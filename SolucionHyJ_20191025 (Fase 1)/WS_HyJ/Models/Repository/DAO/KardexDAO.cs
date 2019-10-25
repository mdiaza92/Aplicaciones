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
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Models.Repository.DAO
{
    public class KardexDAO : IKardexDAO
    {
        private readonly MongoDbContext _context = null;
        private readonly IProductDAO _productDAO;
        private readonly IProviderDAO _providerDAO;

        public KardexDAO(IOptions<Settings> settings, IProductDAO productDAO, IProviderDAO providerDAO)
        {
            _context = new MongoDbContext(settings);
            _productDAO = productDAO;
            _providerDAO = providerDAO;
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveKardexResponse> Add(KardexRequest model)
        {
            try
            {
                //buscar producto por ID
                var producto = await _productDAO.Get(model.IdProducto);

                if (producto == null)
                    return new SaveKardexResponse($"No se encontró el producto relacionado con el ID {model.IdProducto}");

                KardexEntity pi = new KardexEntity()
                {
                    MinimumAmount = model.MinimumAmount,
                    CurrentAmount = model.CurrentAmount,
                    IdProducto = producto._productEntity.Id
                };

                var exist = await ExistModel(pi);

                if (!exist)
                {
                    await _context.Kardex.InsertOneAsync(pi);
                    return new SaveKardexResponse(pi);
                }
                else
                    return new SaveKardexResponse($"Ya existe la producto registrado en el inventario físico del sistema.", HttpStatusCode.Ambiguous);

            }
            catch (AppException ex)
            {
                return new SaveKardexResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Validar si existe el producto o material en el stock
        /// </summary>
        private async Task<bool> ExistModel(KardexEntity model)
        {
            try
            {
                return await _context.Kardex.Find(_ => _.IdProducto == model.IdProducto).AnyAsync();
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<SaveKardexResponse> Delete(string id)
        {
            try
            {
                DeleteResult actionResult
                    = await _context.Kardex.DeleteOneAsync(
                        Builders<KardexEntity>.Filter.Eq("Id", id));

                var response = actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;

                if (response)
                    return new SaveKardexResponse(new KardexEntity() { Id = id });
                else
                    return new SaveKardexResponse($"No se pudo eliminar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveKardexResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveKardexResponse> Get(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                var model = await _context.Kardex
                                .Find(_ => _.Id == id
                                        || _.InternalId == internalId)
                                .FirstOrDefaultAsync();

                return model != null ? new SaveKardexResponse(model) : 
                    new SaveKardexResponse($"No se encontró el elemento con ID {id}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveKardexResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        public async Task<IEnumerable<KardexEntity>> GetAll()
        {
            try
            {
                return await _context.Kardex.Find(_ => true).ToListAsync();
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private async Task<KardexEntity> GetKardexByProduct(string idProducto)
        {
            try
            {
                return await _context.Kardex
                                .Find(_ => true && _.IdProducto == idProducto)
                                .FirstOrDefaultAsync();
            }
            catch (AppException ex)
            {
                throw ex;
            }
        }

        private async Task<SaveKardexResponse> UpdateCurrentAmount(string idProducto, short Amount)
        {
            try
            {
                //buscar kardex por ID
                var kardex = await GetKardexByProduct(idProducto);

                //calcular cantidad
                int cantidadStock = kardex.CurrentAmount != null ? kardex.CurrentAmount.Value + Amount : Amount;

                var filter = Builders<KardexEntity>.Filter.Eq(s => s.Id, kardex.Id);
                var update = Builders<KardexEntity>.Update
                                .Set(s => s.CurrentAmount, Convert.ToInt16(cantidadStock))
                                .CurrentDate(s => s.UpdatedOn);

                UpdateResult actionResult
                    = await _context.Kardex.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                   && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveKardexResponse(new KardexEntity() { Id = kardex.Id });
                else
                    return new SaveKardexResponse($"No se pudo actualizar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveKardexResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveKardexResponse> Update(string id, KardexRequest model)
        {
            try
            {
                //buscar producto por ID
                var producto = await _productDAO.Get(model.IdProducto);

                var filter = Builders<KardexEntity>.Filter.Eq(s => s.Id, id);
                var update = Builders<KardexEntity>.Update
                                .Set(s => s.CurrentAmount, model.CurrentAmount)
                                .Set(s => s.IdProducto, producto._productEntity.Id)
                                .Set(s => s.MinimumAmount, model.MinimumAmount)
                                .CurrentDate(s => s.UpdatedOn);

                UpdateResult actionResult
                    = await _context.Kardex.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                   && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveKardexResponse(new KardexEntity() { Id = id });
                else
                    return new SaveKardexResponse($"No se pudo actualizar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveKardexResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IEnumerable<ProductsByProvider>> GetProductsByProvider(string idProvider)
        {
            try
            {
                //buscar si el ID del proveedor existe
                var existProvider = await _providerDAO.GetProvider(idProvider);

                if (existProvider.Success)
                {
                    List<ProductsByProvider> productsByProviders = new List<ProductsByProvider>();

                    var lista = await _productDAO.GetProductsByProvider(idProvider);
                    
                    if (lista.Any())
                    {
                        foreach (var item in lista)
                        {
                            var getamount = await GetAmountsByProduct(item.Id);

                            productsByProviders.Add(new ProductsByProvider()
                            {
                                Product = (await _productDAO.Get(item.Id))._productEntity,
                                CurrentAmount = getamount.Success ? getamount._model.CurrentAmount ?? (Int16)0 : (Int16)0,
                                MinimumAmount = getamount.Success ? getamount._model.MinimumAmount ?? (Int16)0 : (Int16)0
                            });
                        }
                    }

                    return productsByProviders;
                }
                else
                    throw new Exception("No existe el proveedor seleccionado.");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<SaveKardexResponse> GetAmountsByProduct(string idProduct)
        {
            try
            {
                var model = await _context.Kardex
                                .Find(_ => true && _.IdProducto == idProduct)
                                .FirstOrDefaultAsync();

                return model != null ? new SaveKardexResponse(model) :
                    new SaveKardexResponse($"No se encontró el producto con ID {idProduct}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveKardexResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        public async Task UpdateStockByOrder(string idpedido)
        {
            try
            {
                //buscar pedido por ID
                var pedido = await _context.Order
                                .Find(_ => _.Id == idpedido)
                                .FirstOrDefaultAsync();

                var lista = pedido.DetallePedido.Select(x => new { x.IdProducto, x.Cantidad }).ToList();
                
                //Recorremos el detalle del pedido, el cual ya tiene las cantidades validadas
                foreach (var item in lista)
                {
                    //buscar si el id del producto ya existe en el kardex
                    var exist = await ExistModel(new KardexEntity() { IdProducto = item.IdProducto });

                    //Si no existe, lo agrega
                    if (!exist)
                    {
                        KardexRequest kardex = new KardexRequest()
                        {
                            IdProducto = item.IdProducto,
                            MinimumAmount = null,
                            CurrentAmount = Convert.ToInt16(item.Cantidad)
                        };

                        await Add(kardex);
                    }
                    //sino, lo actualiza
                    else
                    {
                        await UpdateCurrentAmount(item.IdProducto, Convert.ToInt16(item.Cantidad));
                    }
                }
            }
            catch (AppException ex)
            {
                throw ex;
            }
        }

        public async Task ReduceStockByOrder(string idpedido)
        {
            //buscar pedido por ID
            var pedido = await _context.Order
                            .Find(_ => _.Id == idpedido)
                            .FirstOrDefaultAsync();

            var detalle = pedido.DetallePedido.Select(x => new { x.Cantidad, x.IdProducto }).ToList();

            foreach (var item in detalle)
            {
                var kardex = await _context.Kardex
                            .Find(_ => _.IdProducto == item.IdProducto)
                            .FirstOrDefaultAsync();

                if (kardex.CurrentAmount.HasValue)
                {
                    //calcular cantidad
                    int reducirStock = kardex.CurrentAmount.Value - item.Cantidad;

                    if (reducirStock > 0)
                    {
                        var filter = Builders<KardexEntity>.Filter.Eq(s => s.Id, kardex.Id);
                        var update = Builders<KardexEntity>.Update
                                        .Set(s => s.CurrentAmount, Convert.ToInt16(reducirStock))
                                        .CurrentDate(s => s.UpdatedOn);

                        await _context.Kardex.UpdateOneAsync(filter, update);
                    }
                    else
                        await Delete(kardex.Id);
                }
            }
        }
    }
}
