using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WA_HyJ.Models.InternalModels;

namespace WA_HyJ.Interfaces
{
    public interface IBrand
    {
        Task<List<BrandModel>> GetAllAsync(string token = null);

        Task<Uri> AddAsync(BrandModel model, string token = null);

        Task<HttpStatusCode> DeleteAsync(string id, string token = null);

        Task<HttpStatusCode> UpdateAsync(BrandModel model, string token = null);

        Task<BrandModel> DetailsAsync(string id, string token = null);
    }
}
