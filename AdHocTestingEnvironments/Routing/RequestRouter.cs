using AdHocTestingEnvironments.DirectReverseProxy;
using AdHocTestingEnvironments.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace AdHocTestingEnvironments.Routing
{
    public class RequestRouter
    {
        private readonly IRoutingService _routingService;

        public RequestRouter(IRoutingService routingService)
        {
            _routingService = routingService;
        }

        public async Task RouteRequest(HttpContext httpContext, IHttpForwarder forwarder)
        {
            var invoker = new CustomHttpMessageInvoker();
            var transformer = new RequestTransformer(); // or HttpTransformer.Default;
            var requestOptions = new ForwarderRequestConfig { Timeout = TimeSpan.FromSeconds(100) };

            var path = httpContext.Request.Path;
            string destinationUrl = GetDestination(path);

            var error = await forwarder.SendAsync(httpContext, destinationUrl, invoker, requestOptions, transformer);

            // Check if the proxy operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                var exception = errorFeature.Exception;
            }
        }


        public string GetDestination(string path)
        {
            string pattern = @"\/endpoint\/(?<dest>\w+)\/.*";
            var match = Regex.Match(path, pattern);
            if(match.Success)
            {
                string routeName = match.Groups["dest"].Value;
                var item = _routingService.GetItem(routeName);
                return item.Destination;
            }

            throw new ArgumentException("No match found");

        }
    }
}
