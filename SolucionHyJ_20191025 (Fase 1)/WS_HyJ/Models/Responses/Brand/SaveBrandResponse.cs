using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Kardex
{
    public class SaveBrandResponse : BaseResponse
    {
        public BrandEntity _model { get; private set; }

        public SaveBrandResponse(bool success, string message, HttpStatusCode statusCode, BrandEntity model) : base(success, message, statusCode)
        {
            _model = model;
        }

        public SaveBrandResponse(bool success, string message, BrandEntity model) : base(success, message)
        {
            _model = model;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="model">Saved Brand.</param>
        /// <returns>Response.</returns>
        public SaveBrandResponse(BrandEntity model) : this(true, string.Empty, HttpStatusCode.OK, model)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveBrandResponse(string message) : this(false, message, null)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Status Response</param>
        /// <returns>Response.</returns>
        public SaveBrandResponse(string message, HttpStatusCode statusCode) : this(false, message, statusCode, null)
        { }
    }
}
