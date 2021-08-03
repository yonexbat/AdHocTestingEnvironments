using AdHocTestingEnvironments.DirectReverseProxy;
using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class RequestRouterService : IRequestRouterService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public RequestRouterService(IServiceProvider services, ILogger<RequestRouterService> logger)
        {
            _serviceProvider = services;
            _logger = logger;
        }

        public async Task RouteRequest(HttpContext httpContext, IHttpForwarder forwarder)
        {
            _logger.LogInformation("RouteRequest");
            var path = httpContext.Request.Path;
            _logger.LogInformation("Path: {0}", path);

            string destinationUrl = await GetDestination (path);


            var invoker = new CustomHttpMessageInvoker();
            var transformer = new RequestTransformer(); // or HttpTransformer.Default;
            var requestOptions = new ForwarderRequestConfig { Timeout = TimeSpan.FromSeconds(100) };


            var error = await forwarder.SendAsync(httpContext, destinationUrl, invoker, requestOptions, transformer);

            // Check if the proxy operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = httpContext.Features.Get<IForwarderErrorFeature>();
                Exception exception = errorFeature.Exception;
                _logger.LogError(exception, "Error while proxying: {0}", exception.Message);
                await httpContext.Response.WriteAsync("Error");
            }
        }


        private async Task<string> GetDestination(string path)
        {
            string pattern = @"^\/endpoint\/(?<dest>\w+)(\/|$).*";
            var match = Regex.Match(path, pattern);
            if (match.Success)
            {
                string routeName = match.Groups["dest"].Value;
                EndpointEntry item = await GetEndpointEntry(routeName);
                return item.Destination;
            }

            throw new ArgumentException("No match found");
        }

        private async Task<EndpointEntry> GetEndpointEntry(string routename)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                IEndpointResolverService endpointResolverService = scope.ServiceProvider.GetRequiredService<IEndpointResolverService>();
                return await endpointResolverService.GetItem(routename);
            }
        }
    }
}
