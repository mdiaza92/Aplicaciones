using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Kardex
{
    public class ProductResponse
    {
        public ObjectId InternalId { get; set; }

        public string Id { get; set; }

        public int Numero { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public decimal PrecioUnitario { get; set; }

        public string Moneda { get; set; }

        public string UM { get; set; }

        public decimal? Peso { get; set; }

        public string Tipo { get; set; }

        public string Descripcion { get; set; } = null;

        public string IdProveedor { get; set; }

        public BrandEntity Marca { get; set; }

        public ProviderEntity Proveedor { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
