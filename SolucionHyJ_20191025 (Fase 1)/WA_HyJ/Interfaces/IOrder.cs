using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WA_HyJ.Models.InternalModels;
using WA_HyJ.Models.Request;
using WA_HyJ.Models.Response;

namespace WA_HyJ.Interfaces
{
    public interface IOrder
    {
        Task<List<OrderResponse>> GetAllAsync(string token = null);

        Task<Uri> AddAsync(OrderRequest model, string token = null);

        Task<HttpStatusCode> UpdateStatus(string id, string token = null);

        Task<HttpStatusCode> DeleteAsync(string id, string token = null);

        Task<HttpStatusCode> UpdateAsync(OrderRequest model, string token = null);

        Task<OrderResponse> DetailsAsync(string id, string token = null);

        /// <summary>
        /// Actualizar el detalle de un pedido específico.
        /// </summary>
        /// <param name="idPedido">ID del pedido</param>
        /// <param name="details">Detalle del pedido</param>
        /// <param name="token">Token de seguridad</param>
        /// <returns></returns>
        Task<HttpStatusCode> UpdateOrderDetailsAsync(string idPedido, List<OrderDetailRequest> details, string token = null);
    }
}
