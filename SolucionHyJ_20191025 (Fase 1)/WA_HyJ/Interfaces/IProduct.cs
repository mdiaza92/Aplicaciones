using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ
{
    public interface IProduct
    {
        Task<List<ProductModel>> GetAllAsync(string token = null);

        Task<Uri> AddAsync(ProductModel model, string token = null);

        Task<HttpStatusCode> DeleteAsync(string id, string token = null);

        Task<HttpStatusCode> UpdateAsync(ProductModel model, string token = null);

        Task<ProductModel> DetailsAsync(string id, string token = null);
    }
}
