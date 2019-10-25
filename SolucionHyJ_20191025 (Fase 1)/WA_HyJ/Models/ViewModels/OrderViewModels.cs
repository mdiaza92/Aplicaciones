using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WA_HyJ.Models.ViewModels
{
    public class RegistrarPedidos
    {
        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "Es necesario elegir el campo {0}.")]
        public string IdProveedor { get; set; }

        public IEnumerable<SelectListItem> Proveedores { get; set; }
    }

    public class ListarProductosPorProveedor
    {
        public string IdProducto { get; set; }

        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public string Detalle { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }
    }

    public class ProductosSeleccionados
    {
        public string IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class EliminarOrden
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public decimal? Total { get; set; }
    }

    public class ListarPedidos
    {
        public string Id { get; set; }

        [Display(Name = "Código")]
        public string Codigo { get; set; }

        public decimal? Total { get; set; }

        public string Estado { get; set; }

        public bool Validado { get; set; }

        [Display(Name = "Comprobante")]
        public string Comprobante { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime CreatedOn { get; set; }
    }
}