using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;
using WS_HyJ.Models.Requests;
using WS_HyJ.Models.Responses.Order;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Interfaces
{
    public interface IOrderDAO
    {
        Task<SaveOrderResponse> Add(OrderRequest model);
        Task<SaveOrderResponse> Update(OrderEntity model);
        Task<SaveOrderResponse> Delete(string id);
        Task<SaveOrderResponse> Get(string id);
        Task<IEnumerable<OrderResponse>> GetAll();

        /// <summary>
        /// Actualizar el status del pedido a recibido
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="status">Estado (default: Recibido)</param>
        Task<SaveOrderResponse> UpdateStatus(string id, EOrderStatus status = EOrderStatus.Recibido);

        /// <summary>
        /// Actualizar el parámetro "receipt" de la orden seleccionada.
        /// </summary>
        /// <param name="idOrder">ID de la órden</param>
        /// <param name="idReceipt">ID del recibo</param>
        Task<SaveOrderResponse> UpdateReceipt(string idOrder, string idReceipt);

        /// <summary>
        /// Obtener el pedido en base al comprobante
        /// </summary>
        /// <param name="idReceipt">ID del comprobante</param>
        Task<SaveOrderResponse> GetOrderByReceipt(string idReceipt);

        /// <summary>
        /// Actualiza el detalle del pedido.
        /// </summary>
        /// <param name="idOrder">ID del pedido</param>
        /// <param name="orderDetails">Detalle del pedido</param>
        Task<SaveOrderResponse> UpdateOrderDetails(string idOrder, List<OrderDetailRequest> orderDetails);

        /// <summary>
        /// Validar el pedido
        /// </summary>
        /// <param name="id">ID del pedido</param>
        /// <param name="validate">Validar</param>
        Task<bool> ValidateOrder(string id, bool validate = true);
    }
}
