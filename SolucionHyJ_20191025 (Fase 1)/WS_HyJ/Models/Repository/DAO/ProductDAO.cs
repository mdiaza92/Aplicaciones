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
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Models.Repository.DAO
{
    public class ProductDAO : IProductDAO
    {
        private readonly MongoDbContext _context = null;
        private readonly IBrandDAO _brandDAO;
        private readonly IProviderDAO _providerDAO;

        public ProductDAO(IOptions<Settings> settings, IBrandDAO brandDAO, IProviderDAO providerDAO)
        {
            _context = new MongoDbContext(settings);
            _brandDAO = brandDAO;
            _providerDAO = providerDAO;
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveProductResponse> Delete(string id)
        {
            try
            {
                DeleteResult actionResult
                    = await _context.Product.DeleteOneAsync(
                        Builders<ProductEntity>.Filter.Eq("Id", id));
            
                var result = actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;

                DeleteResult delete = await _context.Kardex.DeleteOneAsync(
                        Builders<KardexEntity>.Filter.Eq(x => x.IdProducto, id));

                var responseDelete = delete.IsAcknowledged && delete.DeletedCount > 0;

                string response = null;

                if (result)
                    response = $"Se eliminó correctamente el registro con ID {id}.";

                if (responseDelete)
                    response += $" Además se eliminó el registro vinculado con el stock.";

                if (result)
                    return new SaveProductResponse(result, response, new ProductEntity() { Id = id });
                else
                    return new SaveProductResponse("No fue posible eliminar el registro.");
            }
            catch (AppException ex)
            {
                return new SaveProductResponse(ex.Message);
            }
        }

        public async Task<IEnumerable<ProductResponse>> GetAll()
        {
            try
            {
                List<ProductResponse> all = new List<ProductResponse>();

                var lista = await _context.Product.Find(_ => true)
                    .ToListAsync();

                foreach (var item in lista)
                {
                    all.Add(new ProductResponse()
                    {
                        Numero = item.Numero,
                        Codigo = item.Codigo,
                        PrecioUnitario = item.PrecioUnitario,
                        CreatedOn = item.CreatedOn,
                        Descripcion = item.Descripcion,
                        Id = item.Id,
                        IdProveedor = item.IdProveedor,
                        InternalId = item.InternalId,
                        Marca = item.Marca,
                        Moneda = item.Moneda.ToDescriptionString(),
                        Nombre = item.Nombre,
                        Peso = item.Peso,
                        Proveedor = (await _providerDAO.GetProvider(item.IdProveedor))._model,
                        Tipo = item.Tipo,
                        UM = item.UM.ToDescriptionString(),
                        UpdatedOn = item.UpdatedOn
                    });
                }

                return all;
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<SaveProductResponse> Add(ProductRequest model)
        {
            try
            {
                var marca = await _brandDAO.Get(model.IdMarca);
                
                ProductEntity p = new ProductEntity()
                {
                    PrecioUnitario = model.PrecioUnitario,
                    Descripcion = model.Descripcion,
                    IdProveedor = model.IdProveedor,
                    Marca = marca.Success ? marca._model : null,
                    Moneda = model.Moneda,
                    Nombre = model.Nombre,
                    Peso = model.Peso,
                    Tipo = model.Tipo,
                    UM = model.UM
                };

                var exist = await ExistModel(p);

                if (!exist)
                {
                    //obtener el ultimo valor del atributo numero y aumentarle uno
                    p.Numero = (await GetAll()).ToList().Max(x => x.Numero) + 1;
                    p.Codigo = $"P-{p.Numero.ToString("D4")}";

                    await _context.Product.InsertOneAsync(p);
                    return new SaveProductResponse(p);
                }
                else
                    return new SaveProductResponse($"Ya existe la producto en el sistema.", HttpStatusCode.Ambiguous);

            }
            catch (AppException ex)
            {
                return new SaveProductResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private async Task<bool> ExistModel(ProductEntity model)
        {
            try
            {
                return await _context.Product.Find(_ => _.Nombre == model.Nombre).AnyAsync();
            }
            catch (AppException ex)
            {
                return false;
                throw ex;
            }
        }

        public async Task<SaveProductResponse> Update(string id, ProductRequest model)
        {
            var filter = Builders<ProductEntity>.Filter.Eq(s => s.Id, id);
            var update = Builders<ProductEntity>.Update
                            .Set(s => s.Nombre, model.Nombre)
                            .Set(s => s.PrecioUnitario, model.PrecioUnitario)
                            .Set(s => s.Tipo, model.Tipo)
                            .Set(s => s.Moneda, model.Moneda)
                            .Set(s => s.Peso, model.Peso)
                            .Set(s => s.UM, model.UM)
                            .Set(s => s.Descripcion, model.Descripcion)
                            .Set(s => s.IdProveedor, model.IdProveedor)
                            .CurrentDate(s => s.UpdatedOn);

            try
            {
                string stringresponse = null;

                UpdateResult actionResult
                    = await _context.Product.UpdateOneAsync(filter, update);

                var response1 = (actionResult.IsAcknowledged && actionResult.ModifiedCount > 0);

                stringresponse = response1 == false ? "No se pudo actualizar el producto." : null;

                if (string.IsNullOrEmpty(stringresponse))
                    return new SaveProductResponse(new ProductEntity() { Id = id });
                else
                    return new SaveProductResponse(stringresponse.Trim());
            }
            catch (AppException ex)
            {
                return new SaveProductResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveProductResponse> Get(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                var model = await _context.Product
                                .Find(_ => _.Id == id
                                        || _.InternalId == internalId)
                                .FirstOrDefaultAsync();
                
                if (model != null)
                    return new SaveProductResponse(model);
                else
                    return new SaveProductResponse($"No se encontró el elemento con ID {id}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveProductResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByProvider(string idProvider)
        {
            try
            {
                List<ProductResponse> all = new List<ProductResponse>();

                var getAll = (await GetAll()).ToList();

                if (getAll.Any(x => x.IdProveedor == idProvider))
                {
                    return getAll.Where(x => x.IdProveedor == idProvider).ToList();
                }
                else
                    throw new Exception("No existe en la lista un proveedor con el ID buscado.");
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}
