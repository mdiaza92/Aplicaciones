using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Receipt;

namespace WS_HyJ.Interfaces
{
    /// <summary>
    /// Hace referencia a las facturas
    /// </summary>
    public interface IReceiptDAO
    {
        Task<SaveReceiptResponse> Add(ReceiptRequest model);
        Task<SaveReceiptResponse> Update(string id, ReceiptRequest model);
        Task<SaveReceiptResponse> Delete(string id);
        Task<SaveReceiptResponse> Get(string id);
        Task<IEnumerable<ReceiptResponse>> GetAll();
        /// <summary>
        /// Actualizar el status del comprobante en base al pedido
        /// </summary>
        /// <param name="idpedido">ID del pedido</param>
        Task<bool> UpdateStatusByOrder(string idpedido);
    }
}
