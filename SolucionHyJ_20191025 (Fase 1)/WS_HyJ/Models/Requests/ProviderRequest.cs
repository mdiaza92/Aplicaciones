using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WS_HyJ.Models.Requests
{
    public class ProviderRequest
    {
        [Required]
        public string RUC { get; set; }

        [Required]
        public string RazonSocial { get; set; }

        public string Direccion { get; set; }

        [MaxLength(10, ErrorMessage = "El campo {0} sólo acepta {1} caracteres como máximo.")]
        public string Telefono { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} sólo acepta {1} caracteres como máximo.")]
        public string Contacto { get; set; }

        [MaxLength(9, ErrorMessage = "El campo {0} sólo acepta {1} caracteres como máximo.")]
        public string Celular { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} sólo acepta {1} caracteres como máximo.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}
