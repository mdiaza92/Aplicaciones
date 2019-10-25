using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WA_HyJ.Models.Request
{
    public class OrderRequest
    {
        public string IdUsuario { get; set; } = null;

        [Required(ErrorMessage = "Es necesario agregar el campo {0}.")]
        public List<OrderDetailRequest> DetallePedido { get; set; }
    }

    public class OrderDetailRequest
    {
        [Required(ErrorMessage = "Es necesario agregar el campo {0}.")]
        public string IdProducto { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} necesita estar en el rango de {1} a {2}.")]
        public int Cantidad { get; set; }
    }
}