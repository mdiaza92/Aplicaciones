using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Models.Response
{
    public class OrderResponse
    {
        public string Id { get; set; }

        public int Numero { get; set; }

        public string Codigo { get; set; }

        public decimal? Total { get; set; }

        public string Estado { get; set; }

        public string IdUsuario { get; set; }

        public string NumeroFactura { get; set; }

        public bool Validado { get; set; }

        public List<OrderDetailResponse> DetallePedido { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    public class OrderDetailResponse
    {
        public string Id { get; set; }

        public ProductModel Producto { get; set; }

        public int Cantidad { get; set; }

        public decimal SubTotal { get; set; }
    }
}