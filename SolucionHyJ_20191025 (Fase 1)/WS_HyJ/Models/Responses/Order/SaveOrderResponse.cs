using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Order
{
    public class SaveOrderResponse : BaseResponse
    {
        public OrderResponse _orderEntity { get; set; }

        public SaveOrderResponse(bool success, string message, HttpStatusCode statusCode, OrderResponse orderEntity) : base(success, message, statusCode)
        {
            _orderEntity = orderEntity;
        }

        public SaveOrderResponse(bool success, string message, OrderResponse productEntity) : base(success, message)
        {
            _orderEntity = productEntity;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="productEntity">Saved Brand.</param>
        /// <returns>Response.</returns>
        public SaveOrderResponse(OrderResponse productEntity) : this(true, string.Empty, HttpStatusCode.OK, productEntity)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveOrderResponse(string message) : this(false, message, null)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Status Response</param>
        /// <returns>Response.</returns>
        public SaveOrderResponse(string message, HttpStatusCode statusCode) : this(false, message, statusCode, null)
        { }
    }
}
