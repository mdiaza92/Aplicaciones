using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Internal
{
    public class PaymentMethodEntity
    {
        public PaymentMethodEntity()
        {
            InternalId = ObjectId.GenerateNewId(DateTime.Now);
            Id = Guid.NewGuid().ToString();
            CantidadLetra = null;
            EfectivoCheque = null;
            DetalleLetra = null;
        }

        [BsonId]
        public ObjectId InternalId { get; set; }

        [BsonRequired]
        public string Id { get; set; }

        [BsonRequired]
        public string IdReceipt { get; set; }

        public EPaymentType TipoPago { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int? CantidadLetra { get; set; }

        public EfectivoChequeEntity EfectivoCheque { get; set; }

        public List<LetterDetailEntity> DetalleLetra { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }
    }

    public class LetterDetailEntity
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Fecha { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Dias { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Monto { get; set; }
    }

    public class EfectivoChequeEntity
    {
        public string NombreArchivo { get; set; }

        public byte[] Archivo { get; set; }
    }
}
