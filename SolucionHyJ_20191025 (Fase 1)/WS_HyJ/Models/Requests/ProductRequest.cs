using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Helpers;
using WS_HyJ.Models.Enum;
using WS_HyJ.Models.Internal;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Requests
{
    public class ProductRequest
    {
        public string IdProveedor { get; set; }

        [Required(ErrorMessage = "Es necesario ingresar el campo {0}.")]
        public string Nombre { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El campo {0} debe de estar entre los rangos {1} a {2}.")]
        public decimal PrecioUnitario { get; set; }

        public ECurrencyType Moneda { get; set; }
        
        public EUnitOfMeasurement UM { get; set; }

        public string IdMarca { get; set; }

        public decimal Peso { get; set; }

        public string Tipo { get; set; }

        public string Descripcion { get; set; } = null;
    }
}
