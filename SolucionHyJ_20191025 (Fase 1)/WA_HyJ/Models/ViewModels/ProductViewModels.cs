using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WA_HyJ.Models.ViewModels
{
    public class RegistrarProductos
    {
        [Required(ErrorMessage = "El cambio {0} es requerido.")]
        public string Nombre { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue)]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        public int Moneda { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Peso { get; set; }

        [Display(Name = "Unidad de Medida")]
        public int UnidadMedida { get; set; }

        public string Tipo { get; set; }

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Proveedor")]
        public string IdProveedor { get; set; }

        [Display(Name = "Marca")]
        public string IdMarca { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(1, int.MaxValue)]
        [Display(Name = "Stock mínimo")]
        public int StockMinimo { get; set; } = 1;

        public IEnumerable<SelectListItem> Monedas { get; set; }

        public IEnumerable<SelectListItem> Tipos { get; set; }

        public IEnumerable<SelectListItem> Proveedores { get; set; }

        public IEnumerable<SelectListItem> Unidades { get; set; }

        public IEnumerable<SelectListItem> Marcas { get; set; }
    }

    public class ActualizarProductos
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El cambio {0} es requerido.")]
        public string Nombre { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue)]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        public int Moneda { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Peso { get; set; }

        [Display(Name = "Unidad de Medida")]
        public int UnidadMedida { get; set; }

        public string Tipo { get; set; }

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        [Display(Name = "Proveedor")]
        public string IdProveedor { get; set; }

        [Display(Name = "Marca")]
        public string IdMarca { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(1, int.MaxValue)]
        [Display(Name = "Stock mínimo")]
        public int StockMinimo { get; set; }

        public IEnumerable<SelectListItem> Monedas { get; set; }

        public IEnumerable<SelectListItem> Tipos { get; set; }

        public IEnumerable<SelectListItem> Proveedores { get; set; }

        public IEnumerable<SelectListItem> Unidades { get; set; }

        public IEnumerable<SelectListItem> Marcas { get; set; }
    }

    public class ListarProductos
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public decimal Costo { get; set; }

        public string Moneda { get; set; }

        public decimal Peso { get; set; }

        public string Unidad { get; set; }

        public string Tipo { get; set; }

        public string Marca { get; set; }

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        public string Proveedor { get; set; }
        
        [Display(Name = "Stock mínimo")]
        public int StockMinimo { get; set; }
    }

    public class EliminarProducto
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }
    }

    public class DetalleProducto
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public decimal Costo { get; set; }

        public string Moneda { get; set; }

        public decimal Peso { get; set; }

        public string Unidad { get; set; }

        public string Tipo { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public string Proveedor { get; set; }
    }
}