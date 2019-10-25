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
using WS_HyJ.Models.Responses;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Models.Repository.DAO
{
    public class ProviderDAO : IProviderDAO
    {
        private readonly MongoDbContext _context = null;

        public ProviderDAO(IOptions<Settings> settings)
        {
            _context = new MongoDbContext(settings);
        }

        private ObjectId GetInternalId(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task<SaveProviderResponse> Add(ProviderEntity model)
        {
            try
            {
                var exist = await ExistRUCProvider(model.RUC);

                if (!exist)
                {
                    await _context.Provider.InsertOneAsync(model);
                    return new SaveProviderResponse(model);
                }
                else
                    return new SaveProviderResponse($"Ya existe el RUC '{model.RUC}' en el sistema.", HttpStatusCode.Ambiguous);

            }
            catch (AppException ex)
            {
                return new SaveProviderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveProviderResponse> Delete(string id)
        {
            try
            {
                DeleteResult actionResult
                    = await _context.Provider.DeleteOneAsync(
                        Builders<ProviderEntity>.Filter.Eq("Id", id));

                var response = actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;

                if (response)
                    return new SaveProviderResponse(new ProviderEntity() { Id = id });
                else
                    return new SaveProviderResponse($"No se pudo eliminar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveProviderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<SaveProviderResponse> GetProvider(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                var model = await _context.Provider
                                .Find(_ => _.Id == id
                                        || _.InternalId == internalId)
                                .FirstOrDefaultAsync();

                return model != null ? new SaveProviderResponse(model) : new SaveProviderResponse($"No se encontró el elemento con ID {id}", HttpStatusCode.NotFound);
            }
            catch (AppException ex)
            {
                return new SaveProviderResponse(ex.Message, HttpStatusCode.BadRequest);
            }
        }

        public async Task<bool> ExistRUCProvider(string RUC)
        {
            try
            {
                return await _context.Provider
                                .Find(_ => _.RUC == RUC)
                                .AnyAsync();
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<IEnumerable<ProviderEntity>> GetProviders()
        {
            try
            {
                return await _context.Provider.Find(_ => true).ToListAsync();
            }
            catch (AppException ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<SaveProviderResponse> Update(ProviderEntity model)
        {
            var filter = Builders<ProviderEntity>.Filter.Eq(s => s.Id, model.Id);
            var update = Builders<ProviderEntity>.Update
                            .Set(s => s.Celular, model.Celular)
                            .Set(s => s.Contacto, model.Contacto)
                            .Set(s => s.Descripcion, model.Descripcion)
                            .Set(s => s.Direccion, model.Direccion)
                            .Set(s => s.Email, model.Email)
                            .Set(s => s.RazonSocial, model.RazonSocial)
                            .Set(s => s.RUC, model.RUC)
                            .Set(s => s.Telefono, model.Telefono)
                            .CurrentDate(s => s.UpdatedOn);

            try
            {
                UpdateResult actionResult
                    = await _context.Provider.UpdateOneAsync(filter, update);

                var response = actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;

                if (response)
                    return new SaveProviderResponse(model);
                else
                    return new SaveProviderResponse($"No se pudo actualizar el elemento.");
            }
            catch (AppException ex)
            {
                return new SaveProviderResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
