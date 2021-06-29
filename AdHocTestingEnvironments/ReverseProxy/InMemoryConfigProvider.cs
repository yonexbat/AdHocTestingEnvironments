using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Configuration;

namespace AdHocTestingEnvironments.ReverseProxy
{
    public class InMemoryConfigProvider : IProxyConfigProvider
    {
        private volatile InMemoryConfig _config;

        public InMemoryConfigProvider(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            _config = new InMemoryConfig(routes, clusters);
        }

        public IProxyConfig GetConfig() => _config;

        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            var oldConfig = _config;
            _config = new InMemoryConfig(routes, clusters);
            oldConfig.SignalChange();
        }

        private class InMemoryConfig : IProxyConfig
        {
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
            {
                ChangeToken = new CancellationChangeToken(_cts.Token);
                Routes = routes;
                Clusters = clusters;
            }

            public IReadOnlyList<RouteConfig> Routes { get; }

            public IReadOnlyList<ClusterConfig> Clusters { get; }

            public IChangeToken ChangeToken { get; }


            internal void SignalChange()
            {
                _cts.Cancel();
            }
        }
    }
}
