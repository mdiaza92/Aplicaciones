using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Kardex
{
    public class SaveKardexResponse : BaseResponse
    {
        public KardexEntity _model { get; private set; }

        public SaveKardexResponse(bool success, string message, HttpStatusCode statusCode, KardexEntity model) : base(success, message, statusCode)
        {
            _model = model;
        }

        public SaveKardexResponse(bool success, string message, KardexEntity model) : base(success, message)
        {
            _model = model;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="model">Saved Brand.</param>
        /// <returns>Response.</returns>
        public SaveKardexResponse(KardexEntity model) : this(true, string.Empty, HttpStatusCode.OK, model)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveKardexResponse(string message) : this(false, message, null)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Status Response</param>
        /// <returns>Response.</returns>
        public SaveKardexResponse(string message, HttpStatusCode statusCode) : this(false, message, statusCode, null)
        { }
    }
}
