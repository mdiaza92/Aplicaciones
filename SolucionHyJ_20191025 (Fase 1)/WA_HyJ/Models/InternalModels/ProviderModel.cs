using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_HyJ.Models
{
    public class ProviderModel
    {
        public string Id { get; set; }

        public string RUC { get; set; }

        public string RazonSocial { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string Contacto { get; set; }

        public string Celular { get; set; }

        public string Email { get; set; }

        public string Descripcion { get; set; }

        public DateTime UpdatedOn { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}