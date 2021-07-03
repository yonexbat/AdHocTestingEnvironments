using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.DirectReverseProxy
{
    public class CustomHttpMessageInvoker : HttpMessageInvoker           
    {
        public CustomHttpMessageInvoker() : base(new SocketsHttpHandler()
        {
            SslOptions = new SslClientAuthenticationOptions()
            {
                RemoteCertificateValidationCallback = (
                    object sender,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors) => true,
            },
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
