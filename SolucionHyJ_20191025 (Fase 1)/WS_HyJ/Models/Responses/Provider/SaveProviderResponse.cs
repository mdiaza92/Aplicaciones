using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Kardex
{
    public class SaveProviderResponse : BaseResponse
    {
        public ProviderEntity _model { get; private set; }

        public SaveProviderResponse(bool success, string message, HttpStatusCode statusCode, ProviderEntity model) : base(success, message, statusCode)
        {
            _model = model;
        }

        public SaveProviderResponse(bool success, string message, ProviderEntity model) : base(success, message)
        {
            _model = model;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="model">Saved Brand.</param>
        /// <returns>Response.</returns>
        public SaveProviderResponse(ProviderEntity model) : this(true, string.Empty, HttpStatusCode.OK, model)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveProviderResponse(string message) : this(false, message, null)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Status Response</param>
        /// <returns>Response.</returns>
        public SaveProviderResponse(string message, HttpStatusCode statusCode) : this(false, message, statusCode, null)
        { }
    }
}
