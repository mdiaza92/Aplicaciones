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
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Models.Repository.DAO
{
    public class BrandDAO : IBrandDAO
    {
        private readonly MongoDbContext _context = null;

        public BrandDAO(IOptions<Settings> settings)
        {
            _context = new MongoDbContext(settings);
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveBrandResponse> Add(BrandEntity model)
        {
            SaveBrandResponse response = null;

            try
            {
                var exist = await ExistBrand(model.Nombre);

                if (!exist)
                {
                    await _context.Brand.InsertOneAsync(model);
                    response = new SaveBrandResponse(model);
                }
                else
                    response = new SaveBrandResponse($"Ya existe la marca '{model.Nombre}' en el sistema.", HttpStatusCode.Ambiguous);

            }
            catch (AppException ex)
            {
                response = new SaveBrandResponse(ex.Message, HttpStatusCode.InternalServerError);
            }

            return response;
        }

        private async Task<bool> ExistBrand(string nombre)
        {
            try
            {
                return await _context.Brand.Find(_ => _.Nombre == nombre).AnyAsync();
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<SaveBrandResponse> Delete(string id)
        {
            try
            {
                DeleteResult actionResult
                    = await _context.Brand.DeleteOneAsync(
                        Builders<BrandEntity>.Filter.Eq("Id", id));

                var response = actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;

                if (response)
                    return new SaveBrandResponse(new BrandEntity() { Id = id });
                else
                    return new SaveBrandResponse($"No se pudo eliminar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveBrandResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveBrandResponse> Get(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                var model = await _context.Brand
                                .Find(_ => _.Id == id
                                        || _.InternalId == internalId)
                                .FirstOrDefaultAsync();

                return model != null ? new SaveBrandResponse(model) : new SaveBrandResponse($"No se encontró el elemento con ID {id}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveBrandResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        public async Task<IEnumerable<BrandEntity>> GetAll()
        {
            try
            {
                return await _context.Brand.Find(_ => true).ToListAsync();
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<SaveBrandResponse> Update(BrandEntity model)
        {
            var filter = Builders<BrandEntity>.Filter.Eq(s => s.Id, model.Id);
            var update = Builders<BrandEntity>.Update
                            .Set(s => s.Nombre, model.Nombre);

            try
            {
                UpdateResult actionResult
                    = await _context.Brand.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveBrandResponse(model);
                else
                    return new SaveBrandResponse($"No se pudo actualizar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveBrandResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
