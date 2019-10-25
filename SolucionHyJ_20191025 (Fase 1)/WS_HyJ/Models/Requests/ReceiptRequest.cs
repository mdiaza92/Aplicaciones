using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Requests
{
    public class ReceiptRequest
    {
        [Required]
        public string IdOrden { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFactura { get; set; }

        public EReceiptType Tipo { get; set; }
    }
}
