using AdHocTestingEnvironments.Services.Interfaces;
using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class KubernetesFactory : IKubernetesFactory
    {

        private readonly string _accessToken;
        private readonly string _host;
        private readonly ILogger _logger;


        public KubernetesFactory(IConfiguration configuration, ILogger<KubernetesFactory> logger)
        {
            _logger = logger;
            _logger = logger;
            _host = configuration.GetValue<string>("KubernetesHost");
            _accessToken = configuration.GetValue<string>("KubernetesAccessToken");
        }

        public IKubernetes CreateClient()
        {
            _logger.LogInformation("Api host: {0}", _host);

            KubernetesClientConfiguration config = new KubernetesClientConfiguration();
            config.AccessToken = _accessToken;
            config.Host = _host;
            config.SkipTlsVerify = true;
            IKubernetes client = new Kubernetes(config);
            return client;
        }
    }
}
