using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WA_HyJ.Models;

namespace WA_HyJ.Interfaces
{
    public interface IProvider
    {
        /// <summary>
        /// Obtener lista de proveedores
        /// </summary>
        /// <param name="token">Token de seguridad (JWT)</param>
        Task<List<ProviderModel>> GetAllAsync(string token = null);

        Task<Uri> AddAsync(ProviderModel model, string token = null);

        Task<HttpStatusCode> DeleteAsync(string id, string token = null);

        Task<HttpStatusCode> UpdateAsync(ProviderModel model, string token = null);

        Task<ProviderModel> DetailsAsync(string id, string token = null);
    }
}