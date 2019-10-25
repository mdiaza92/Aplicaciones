using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WA_HyJ.Models.Request
{
    public class ReceiptRequest
    {
        [Required]
        public string IdOrden { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

        [Required]
        public DateTime FechaFactura { get; set; }

        public int Tipo { get; set; }
    }
}