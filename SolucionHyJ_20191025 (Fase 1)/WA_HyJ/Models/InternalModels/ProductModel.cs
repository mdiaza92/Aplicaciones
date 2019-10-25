using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WA_HyJ.Models.ViewModels;

namespace WA_HyJ.Models.InternalModels
{
    public class ProductModel
    {
        public string Id { get; set; }

        public int Numero { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public decimal PrecioUnitario { get; set; }

        public string Moneda { get; set; }

        public string UM { get; set; }

        public decimal Peso { get; set; }

        public string Tipo { get; set; }

        public string Descripcion { get; set; }

        public string IdProveedor { get; set; }

        public string IdMarca { get; set; }

        public MarcaModel Marca { get; set; }

        public ProviderModel Proveedor { get; set; }
    }

    public class MarcaModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
}