using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Kardex;

namespace WS_HyJ.Interfaces
{
    public interface IKardexDAO
    {
        Task<SaveKardexResponse> Add(KardexRequest model);
        Task<SaveKardexResponse> Update(string id, KardexRequest model);
        Task<SaveKardexResponse> Delete(string id);
        Task<SaveKardexResponse> Get(string id);
        Task<IEnumerable<KardexEntity>> GetAll();

        /// <summary>
        /// Obtener la lista de productos disponibles y stock actual por proveedor
        /// </summary>
        /// <param name="idProvider">ID del proveedor</param>
        Task<IEnumerable<ProductsByProvider>> GetProductsByProvider(string idProvider);

        Task<SaveKardexResponse> GetAmountsByProduct(string idProduct);

        /// <summary>
        /// Actualiza el stock actual en base al pedido
        /// </summary>
        /// <param name="idpedido">ID del pedido</param>
        Task UpdateStockByOrder(string idpedido);

        /// <summary>
        /// Disminuye el stock en base a un pedido
        /// </summary>
        /// <param name="idpedido">ID del pedido</param>
        /// <returns></returns>
        Task ReduceStockByOrder(string idpedido);
    }
}
