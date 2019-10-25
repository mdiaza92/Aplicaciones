using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WS_HyJ.Models.Responses
{
    public abstract class BaseResponse
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
        public HttpStatusCode StatusCode { get; protected set; }

        public BaseResponse(bool success, string message, HttpStatusCode statusCode)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
        }

        public BaseResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
