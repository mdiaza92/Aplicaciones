using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Repository
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database = null;

        public MongoDbContext(IOptions<Settings> settings)
        {
            try
            {
                var client = new MongoClient(settings.Value.ConnectionString);
                if (client != null)
                    _database = client.GetDatabase(settings.Value.Database);

            }catch(Exception ex)
            {
                throw new Exception("No se puede acceder al servidor MongoDB.", ex);
            }
        }

        public IMongoCollection<EmployeeRequest> Employee
        {
            get
            {
                return _database.GetCollection<EmployeeRequest>("Employee");
            }
        }

        public IMongoCollection<ProviderEntity> Provider
        {
            get
            {
                return _database.GetCollection<ProviderEntity>("Provider");
            }
        }

        public IMongoCollection<ProductEntity> Product
        {
            get
            {
                return _database.GetCollection<ProductEntity>("Product");
            }
        }

        public IMongoCollection<BrandEntity> Brand
        {
            get
            {
                return _database.GetCollection<BrandEntity>("Brand");
            }
        }

        public IMongoCollection<KardexEntity> Kardex
        {
            get
            {
                return _database.GetCollection<KardexEntity>("Kardex");
            }
        }

        public IMongoCollection<OrderEntity> Order
        {
            get
            {
                return _database.GetCollection<OrderEntity>("Order");
            }
        }

        public IMongoCollection<ReceiptEntity> Receipt
        {
            get
            {
                return _database.GetCollection<ReceiptEntity>("Receipt");
            }
        }

        public IMongoCollection<PaymentMethodEntity> PaymentMethod
        {
            get
            {
                return _database.GetCollection<PaymentMethodEntity>("PaymentMethod");
            }
        }
    }
}
