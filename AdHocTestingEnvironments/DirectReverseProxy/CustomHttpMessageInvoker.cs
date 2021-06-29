using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.DirectReverseProxy
{
    public class CustomHttpMessageInvoker : HttpMessageInvoker           
    {
        public CustomHttpMessageInvoker() : base(new SocketsHttpHandler()
        {
            /*
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false*/
        })
        {

        }
    }
}
