using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Kardex
{
    public class SaveProductResponse : BaseResponse
    {
        public ProductEntity _productEntity { get; private set; }

        public SaveProductResponse(bool success, string message, HttpStatusCode statusCode, ProductEntity productEntity) : base(success, message, statusCode)
        {
            _productEntity = productEntity;
        }

        public SaveProductResponse(bool success, string message, ProductEntity productEntity) : base(success, message)
        {
            _productEntity = productEntity;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="productEntity">Saved Product.</param>
        /// <returns>Response.</returns>
        public SaveProductResponse(ProductEntity productEntity) : this(true, string.Empty, HttpStatusCode.OK, productEntity)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveProductResponse(string message) : this(false, message, new ProductEntity())
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Status Response</param>
        /// <returns>Response.</returns>
        public SaveProductResponse(string message, HttpStatusCode statusCode) : this(false, message, statusCode, null)
        { }
    }
}
