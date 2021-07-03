using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

namespace AdHocTestingEnvironments.ReverseProxy
{
    public static class ProxyInitializtation
    {
        public static void InitReverseProxy(this IServiceCollection services)
        {

            var routes = new[]
            {
                new RouteConfig()
                {
                    ClusterId = "cluster1",
                    RouteId = "route1",
                    Match = new RouteMatch
                    {
                        Path = "/something/{**remainder}",
                    }
                }
            };

            var clusters = new[]
            {
                new ClusterConfig()
                {
                    ClusterId ="cluster1",
                    Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                         { "destination1", new DestinationConfig() { Address = "https://www.google.com/" } }
                    },
                }
            };

            services.AddReverseProxy()
                .LoadFromMemory(routes, clusters);
        }
    }
}
