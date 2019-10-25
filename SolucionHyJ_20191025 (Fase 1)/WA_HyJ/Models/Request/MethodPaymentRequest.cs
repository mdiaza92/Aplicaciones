using System;
using System.Collections.Generic;
using System.Web;
using static WA_HyJ.Helpers.CustomValidation;

namespace WA_HyJ.Models.Request
{
    public class LetterMethodPaymentRequest
    {
        public int TipoPago { get; set; }

        public int? CantidadLetra { get; set; }

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
        public int TipoPago { get; set; }
        
        public HttpPostedFileBase Imagen { get; set; }
    }
}
