using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services.Interfaces;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class KubernetesClientService : IKubernetesClientService
    {

        private readonly ILogger _logger;
        private readonly IKubernetesFactory _kubernetesFactory;
        private readonly IKubernetesObjectBuilder _kubernetesObjectBuilder;
        private readonly string _namespace;

        private const string CREATOR_VALUE = "adhoctestingenvironments";
        private const string CREATOR_KEY = "creator";
        private const string UPTIME = "uptime";

        public KubernetesClientService(IConfiguration configuration, IKubernetesFactory kubernetesFactory, 
            IKubernetesObjectBuilder kubernetesObjectBuilder, ILogger<KubernetesClientService> logger)
        {
            _logger = logger;
            _kubernetesFactory = kubernetesFactory;
            _namespace = configuration.GetValue<string>("KubernetesNamespace");
            _kubernetesObjectBuilder = kubernetesObjectBuilder;
        }

        public async Task<string> StartEnvironment(CreateEnvironmentInstanceData instanceInfo)
        {
            _logger.LogInformation("Starting environment. Imgage: {0}, AppName: {1}.", instanceInfo.Image, instanceInfo.Name);

            using (IKubernetes client = CreateClient())
            {

                IList<IKubernetesObject> objects = _kubernetesObjectBuilder.CreateObjectDefinitions(instanceInfo);
                foreach(IKubernetesObject kubernetesObject in objects)
                {
                    Object res = kubernetesObject switch
                    {
                        V1ConfigMap configMap => await client.CreateNamespacedConfigMapAsync(configMap, _namespace),
                        V1Deployment deployment => await client.CreateNamespacedDeploymentAsync(deployment, _namespace),
                        V1Service service => await client.CreateNamespacedServiceAsync(service, _namespace),
                        _ => throw new ArgumentException(),
                    };
                    string jsonString = JsonSerializer.Serialize(res);
                    _logger.LogInformation("Created Kubernetes Object: {0}", jsonString);
                }               
            }
            return "Ok";
        }

        public async Task<string> StopEnvironment(string appName)
        {
            _logger.LogInformation("Stopping environment: {0}", appName);
            using (IKubernetes client = CreateClient())
            {
                await client.DeleteNamespacedServiceAsync(appName, _namespace);
                _logger.LogInformation("Stopped service {0}", appName);

                await client.DeleteNamespacedDeploymentAsync(appName, _namespace);
                _logger.LogInformation("Stopped deployment {0}. Pods take a while to shut down", appName);

                await client.DeleteNamespacedConfigMapAsync(appName, _namespace);
                _logger.LogInformation("Deleted configmap: {0}", appName);
            }

            return "Ok";
        }

        public async Task<IList<EnvironmentInstance>> GetEnvironments()
        {
            _logger.LogInformation("Getting environments");
            V1ServiceList serviceList;
            V1PodList podList;

            using (IKubernetes client = CreateClient())
            {
                serviceList = await client.ListNamespacedServiceAsync(_namespace);
                podList = await client.ListNamespacedPodAsync(_namespace);
            }

            if (serviceList.Items != null)
            {
                List<EnvironmentInstance> appInstances = serviceList.Items
                    .Where(x => x.Metadata?.Labels?.Any() == true)
                    .Where(x => x.Metadata.Labels.Any(l => l.Key == CREATOR_KEY && l.Value == CREATOR_VALUE))
                    .Select(x => new EnvironmentInstance()
                    {
                        Name = x.Metadata.Name,
                        Status = "Unknown",
                        NumHoursToRun = x.Metadata.Labels.ContainsKey(UPTIME) ? int.Parse(x.Metadata.Labels[UPTIME]) : null,
                    })
                    .ToList();

                appInstances.ForEach(x =>
                {
                    V1Pod pod = podList.Items
                        .Where(p => (p.Metadata?.Name ?? string.Empty).StartsWith(x.Name))
                        .SingleOrDefault();

                    if (pod != null)
                    {
                        if (pod.Status?.Conditions?.Any(x => x.Type == "Ready" && x.Status == "True") == true)
                        {
                            x.Status = "Ready";
                        }
                        else
                        {
                            x.Status = "Not ready yet";
                        }

                        x.StartTime = pod.Status?.StartTime;
                    }
                });

                return appInstances;
            }
            _logger.LogDebug("No services found");
            return new List<EnvironmentInstance>();
        }

        private IKubernetes CreateClient()
        {
            _logger.LogInformation("Namespace: {0}", _namespace);
            return _kubernetesFactory.CreateClient();
        }
    }
}
