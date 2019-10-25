using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Internal
{
    /// <summary>
    /// Pedidos
    /// </summary>
    public class OrderEntity
    {
        public OrderEntity()
        {
            InternalId = ObjectId.GenerateNewId(DateTime.Now);
            Id = Guid.NewGuid().ToString();
        }

        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonRequired]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        public int Numero { get; set; }

        /// <summary>
        /// Correlativo (O-0001)
        /// </summary>
        public string Codigo { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Total { get; set; }

        /// <summary>
        /// Estado del pedido (default: pendiente)
        /// </summary>
        public EOrderStatus Estado { get; set; } = EOrderStatus.Pendiente;

        public string IdUsuario { get; set; }

        public string IdRecibo { get; set; }

        /// <summary>
        /// Campo usado para confirmar si la orden ya se validó en conjunto con el comprobante.
        /// </summary>
        [BsonRepresentation(BsonType.Boolean)]
        public bool Validado { get; set; } = false;

        public List<OrderDetailEntity> DetallePedido { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }

    public class OrderDetailEntity
    {
        public OrderDetailEntity()
        {
            InternalId = ObjectId.GenerateNewId(DateTime.Now);
            Id = Guid.NewGuid().ToString();
        }

        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonRequired]
        public string Id { get; set; }

        [BsonRequired]
        public string IdProducto { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Cantidad { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal SubTotal { get; set; }

        //[BsonIgnore]
        //public ProductEntity Producto { get; set; }
    }
}
