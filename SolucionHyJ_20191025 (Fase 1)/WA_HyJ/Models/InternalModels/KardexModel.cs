using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_HyJ.Models.InternalModels
{
    public class KardexModel
    {
        
    }

    public class ProductsByProvider
    {
        public ProductModel Product { get; set; }

        public int MinimumAmount { get; set; }

        public int CurrentAmount { get; set; }
    }
}