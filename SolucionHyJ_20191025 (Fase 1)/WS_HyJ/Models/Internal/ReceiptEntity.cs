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
    /// Comprobante
    /// </summary>
    public class ReceiptEntity
    {
        public ReceiptEntity()
        {
            InternalId = ObjectId.GenerateNewId(DateTime.Now);
            Id = Guid.NewGuid().ToString();
            Tipo = EReceiptType.Factura;
            UpdatedOn = DateTime.Now;
            CreatedOn = DateTime.Now;
        }

        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonRequired]
        public string Id { get; set; }

        /// <summary>
        /// Número de factura ingresado por el personal
        /// </summary>
        [BsonRequired]
        public string NumeroFactura { get; set; }
        
        [BsonDateTimeOptions(Kind = DateTimeKind.Local, Representation = BsonType.DateTime)]
        [BsonRequired]
        public DateTime FechaFactura { get; set; }

        /// <summary>
        /// Tipo de comprobante (default: Factura)
        /// </summary>
        public EReceiptType Tipo { get; set; }

        /// <summary>
        /// Estado de revisión del comprobante
        /// </summary>
        public EReceiptStatus Estado { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }
    }
}
