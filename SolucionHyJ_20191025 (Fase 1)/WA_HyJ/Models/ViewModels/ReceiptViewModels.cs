using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WA_HyJ.Models.Response;
using static WA_HyJ.Helpers.CustomValidation;

namespace WA_HyJ.Models.ViewModels
{
    public class RegistrarFactura
    {
        [Required(ErrorMessage = "Es necesario agregar el campo {0}.")]
        public string Registro { get; set; }

        public string IdOrden { get; set; }

        [Required(ErrorMessage = "Es necesario agregar el campo {0}.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de registro")]
        public DateTime FechaComprobante { get; set; }

        public string CodigoOrden { get; set; }

        [Display(Name = "Tipo de comprobante")]
        [Required(ErrorMessage = "Es necesario agregar el campo {0}.")]
        public int IdTipoComprobante { get; set; }

        public IEnumerable<SelectListItem> TipoComprobante { get; set; }
    }

    public class ListarComprobantes
    {
        public string Id { get; set; }

        public string Comprobante { get; set; }

        public string Tipo { get; set; }

        public string Estado { get; set; }

        public bool Validado { get; set; }

        public bool TieneMedioPago { get; set; }

        [Display(Name = "Fecha de registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }
    }

    public class DetalleComprobante
    {
        public string Id { get; set; }

        public string Tipo { get; set; }

        public string Comprobante { get; set; }

        public double IGV { get; set; }

        public OrderResponse Pedido { get; set; }
    }

    public class EliminarComprobante
    {
        public string Id { get; set; }

        public string Comprobante { get; set; }

        public string Tipo { get; set; }

        public OrderResponse Pedido { get; set; }

        [Display(Name = "Fecha de registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }
    }

    public class ElegirMedioPago
    {
        public string IdComprobante { get; set; }

        public string IdPedido { get; set; }

        [Display(Name = "Medio de pago")]
        public int IdMedioPago { get; set; }

        public IEnumerable<SelectListItem> MediosPago { get; set; }
    }

    public class GenerarLetras
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha del comprobante")]
        public DateTime Fecha { get; set; }

        [Range(1, 4, ErrorMessage = "El campo {0} debe de estar entre {1} y {2}.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int Cantidad { get; set; }

        public decimal Total { get; set; }

        public List<LetraDetalle> Detalle { get; set; }
    }

    public class LetraDetalle
    {
        public int Numero { get; set; }

        public decimal Monto { get; set; }
        
        public int Dias { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Fecha { get; set; }
    }
}