using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Services.WeatherAppServices.Base
{
    /// <summary>
    /// Base Service Response with Status Code and Response Object
    /// </summary>
    public class BaseServiceResponse<T>
    {
        public BaseServiceResponse(string Class = null, string Method = null)
        {
            Messages = new AppCenterBase(Class, Method);
        }

        public HttpStatusCode StatusCodeResponse { get; set; }
        public AppCenterBase Messages { get; set; }
        public T Response { get; set; }
    }

    public class AppCenterBase
    {
        public AppCenterBase(string Class = null, string Method = null)
        {
            if (!string.IsNullOrEmpty(Class))
            {
                Properties = new Dictionary<string, string> { { Class, Method } };
            }
        }

        public string Message { get; set; }
        public Exception Exception { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

}
