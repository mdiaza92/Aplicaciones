using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Interfaces
{
    public interface IKardex
    {
        Task<List<ProductsByProvider>> GetStockAsync(string idProvider, string token = null);
    }
}
