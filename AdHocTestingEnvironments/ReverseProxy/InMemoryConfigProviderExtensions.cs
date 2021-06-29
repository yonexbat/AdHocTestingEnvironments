using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

namespace AdHocTestingEnvironments.ReverseProxy
{
    public static class InMemoryConfigProviderExtensions
    {
        public static IReverseProxyBuilder LoadFromMemory(this IReverseProxyBuilder builder, IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            IProxyConfigProvider provider = new InMemoryConfigProvider(routes, clusters);
            builder.Services.AddSingleton<IProxyConfigProvider>(provider);
            return builder;
        }
    }
}
