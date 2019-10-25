using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WS_HyJ.Models.Internal;

namespace WS_HyJ.Models.Responses.Receipt
{
    public class SaveReceiptResponse : BaseResponse
    {
        public ReceiptResponse _receiptResponse { get; set; }

        public SaveReceiptResponse(bool success, string message, HttpStatusCode statusCode, ReceiptResponse receiptResponse) : base(success, message, statusCode)
        {
            _receiptResponse = receiptResponse;
        }

        public SaveReceiptResponse(bool success, string message, ReceiptResponse receiptResponse) : base(success, message)
        {
            _receiptResponse = receiptResponse;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="receiptResponse">Saved Receipt.</param>
        /// <returns>Response.</returns>
        public SaveReceiptResponse(ReceiptResponse receiptResponse) : this(true, string.Empty, HttpStatusCode.OK, receiptResponse)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public SaveReceiptResponse(string message) : this(false, message, new ReceiptResponse())
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="statusCode">Status Response</param>
        /// <returns>Response.</returns>
        public SaveReceiptResponse(string message, HttpStatusCode statusCode) : this(false, message, statusCode, null)
        { }
    }
}
