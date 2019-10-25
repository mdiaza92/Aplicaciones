using System.ComponentModel.DataAnnotations;
using static WA_HyJ.Helpers.CustomValidation;

namespace WA_HyJ.Models.ViewModels
{
    public class RegistroProveedores
    {
        [Required(ErrorMessage = "El campo es requerido.")]
        [RUCValidator]
        public string RUC { get; set; }

        [Display(Name = "Razon Social")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string RazonSocial { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }
        
        public string Contacto { get; set; }

        public string Celular { get; set; }
    }

    /// <summary>
    /// ViewModel para actualizar proveedores
    /// </summary>
    public class ActualizarProveedores
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El campo es requerido.")]
        [RUCValidator]
        public string RUC { get; set; }

        [Display(Name = "Razon Social")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string RazonSocial { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string Contacto { get; set; }

        public string Celular { get; set; }
    }

    public class DetalleProveedor
    {
        public string Id { get; set; }
        
        public string RUC { get; set; }

        [Display(Name = "Razon Social")]
        public string RazonSocial { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string Contacto { get; set; }

        public string Celular { get; set; }
    }

    public class ListarProveedores
    {
        public string Id { get; set; }

        public string RUC { get; set; }

        [Display(Name = "Razon Social")]
        public string RazonSocial { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        [Display(Name = "Vendedor")]
        public string Contacto { get; set; }

        public string Celular { get; set; }
    }

    public class EliminarProveedores
    {
        public string Id { get; set; }
        public string RazonSocial { get; set; }
        public string RUC { get; set; }
    }
}