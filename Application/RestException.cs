using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application
{
    public class RestException : Exception
    {
        public HttpStatusCode Code { get; set; }
        public object Error { get; set; }
        public RestException(HttpStatusCode code, object error = null)
        {
            Code = code;
            Error = error;
        }
    }
}
