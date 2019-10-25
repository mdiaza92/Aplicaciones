using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Helpers;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Requests
{
    public class LetterMethodPaymentRequest
    {
        [Required]
        public EPaymentType TipoPago { get; set; }

        [Required]
        public int CantidadLetra { get; set; }

        [Required]
        public List<LetterDetailRequest> DetalleLetra { get; set; }
    }

    public class LetterDetailRequest
    {
        public DateTime Fecha { get; set; }

        public int Dias { get; set; }

        public decimal? Monto { get; set; }
    }

    public class OtherMethodPaymentRequest
    {
        [Required]
        public EPaymentType TipoPago { get; set; }

        [Required]
        public IFormFile Imagen { get; set; }
    }
}
