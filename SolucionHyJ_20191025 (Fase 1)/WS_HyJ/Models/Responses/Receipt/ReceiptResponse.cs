using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Responses.Order;

namespace WS_HyJ.Models.Responses.Receipt
{
    public class ReceiptResponse
    {
        public string Id { get; set; }

        public string NumeroFactura { get; set; }

        public DateTime FechaFactura { get; set; }

        public string Tipo { get; set; }

        public string Estado { get; set; }

        /// <summary>
        /// Sólo es usado para mostrar si el comprobante a pesar de estar con estado conforme,
        /// tiene el valor de tipo de pago en null
        /// </summary>
        public bool TieneMedioPago { get; set; }

        public OrderResponse Pedido { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
