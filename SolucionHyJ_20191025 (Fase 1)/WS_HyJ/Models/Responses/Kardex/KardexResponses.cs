using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Kardex
{
    public class ProductsByProvider
    {
        public ProductEntity Product { get; set; }

        public short MinimumAmount { get; set; }

        public short CurrentAmount { get; set; }
    }
}
