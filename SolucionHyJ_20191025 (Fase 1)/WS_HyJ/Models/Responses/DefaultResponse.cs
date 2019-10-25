using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WS_HyJ.Models.Responses
{
    public class DefaultResponse
    {
        public bool _Response { get; set; }
        public HttpStatusCode _StatusCode { get; set; }
    }
}
