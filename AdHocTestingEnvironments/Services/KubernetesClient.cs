using AdHocTestingEnvironments.Model.Kubernetes;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public class KubernetesClient : IKubernetesClient
    {
        private readonly string _accessToken;
        private readonly string _host;
        private readonly string _namespace;
        private readonly ILogger _logger;
        private const string CREATOR_VALUE = "adhoctestingenvironments";
        private const string CREATOR_KEY = "creator";

        public KubernetesClient(IConfiguration configuration, ILogger<KubernetesClient> logger)
        {
            _logger = logger;
            _host = configuration.GetValue<string>("KubernetesHost");
            _accessToken = configuration.GetValue<string>("KubernetesAccessToken");
            _namespace = configuration.GetValue<string>("KubernetesNamespace");
        }

        public async Task<string> StartEnvironment(CreateEnvironmentInstanceData instanceInfo)
        {
            _logger.LogInformation("Starting environment. Imgage: {0}, AppName: {1}.", instanceInfo.Image, instanceInfo.Name);

            IKubernetes client = CreateClient();

            // ConfigMap
            var configMap = CreateConfigMap(instanceInfo.Name, instanceInfo.InitSqlScript);
            V1ConfigMap resConfigMap = await client.CreateNamespacedConfigMapAsync(configMap, _namespace);
            string jsonString = JsonSerializer.Serialize(resConfigMap);
            _logger.LogInformation("Created ConfigMap: {0}", jsonString);


            // Deployment
            var deployment = CreateDeployment(instanceInfo.Name, instanceInfo.Image);
            var resDeployment = await client.CreateNamespacedDeploymentAsync(deployment, _namespace);
            jsonString = JsonSerializer.Serialize(resDeployment);
            _logger.LogInformation("Created Deployment: {0}", jsonString);


            // Service
            V1Service service = CreateService(instanceInfo.Name);
            var resService = await client.CreateNamespacedServiceAsync(service, _namespace);
            jsonString = JsonSerializer.Serialize(resService);
            _logger.LogInformation("Created Service: {0}", jsonString);

            return "Ok";
        }

        public async Task<string> StopEnvironment(string appName)
        {
            _logger.LogInformation("Stopping environment: {0}", appName);
            IKubernetes client = CreateClient();
            
            await client.DeleteNamespacedServiceAsync(appName, _namespace);
            _logger.LogInformation("Stopped service {0}", appName);

            await client.DeleteNamespacedDeploymentAsync(appName, _namespace);
            _logger.LogInformation("Stopped deployment {0}. Pods take a while to shut down", appName);

            await client.DeleteNamespacedConfigMapAsync(appName, _namespace);
            _logger.LogInformation("Stopped configmap: {0}", appName);

            return "Ok";
        }

        public async Task<IList<EnvironmentInstance>> GetEnvironments()
        {
            _logger.LogInformation("Getting environments");
            IKubernetes client = CreateClient();
            V1ServiceList serviceList = await client.ListNamespacedServiceAsync(_namespace);
            V1PodList podList = await client.ListNamespacedPodAsync(_namespace);

            if (serviceList.Items != null)
            {
                List<EnvironmentInstance> appInstances = serviceList.Items
                    .Where(x => x.Metadata.Labels?.Any() == true)
                    .Where(x => x.Metadata.Labels.Any(l => l.Key == CREATOR_KEY && l.Value == CREATOR_VALUE))
                    .Select(x => x.Metadata.Name)
                    .Select(x => new EnvironmentInstance()
                    {
                        Name = x,
                        Status = "Unknown",
                    })
                    .ToList();

                appInstances.ForEach(x =>
                {
                    V1Pod pod = podList.Items
                        .Where(p => (p.Metadata?.Name ?? string.Empty).StartsWith(x.Name))
                        .SingleOrDefault();

                    if(pod != null)
                    {
                        x.Status = pod.Status?.Phase;
                    }
                });

                return appInstances;
            }
            _logger.LogDebug("No services found");
            return new List<EnvironmentInstance>();
        }

        private IKubernetes CreateClient()
        {            
            _logger.LogInformation("Api host: {0}", _host);
            _logger.LogInformation("Namespace: {0)", _namespace);

            KubernetesClientConfiguration config = new KubernetesClientConfiguration();
            config.AccessToken = _accessToken;
            config.Host = _host;
            config.SkipTlsVerify = true;
            IKubernetes client = new Kubernetes(config);
            return client;
        }

        private V1Service CreateService(string appName)
        {
            V1Service service = new V1Service();
            service.Metadata = new V1ObjectMeta();
            service.Metadata.Name = appName;
            service.Metadata.Labels = new Dictionary<string, string>();
            service.Metadata.Labels[CREATOR_KEY] = CREATOR_VALUE;
            service.Spec = new V1ServiceSpec();
            service.Spec.Selector = new Dictionary<string, string>();
            service.Spec.Selector["app"] = appName;
            service.Spec.Ports = new List<V1ServicePort>();
            V1ServicePort port = new V1ServicePort();
            port.Protocol = "TCP";
            port.Port = 80;
            port.TargetPort = 80;
            service.Spec.Type = "ClusterIP";
            service.Spec.Ports.Add(port);
            return service;
        }

        private V1Deployment CreateDeployment(string appName, string image)
        {
            // Deployment
            var deployment = CreateDeploymentHeader(appName);

            // Deployment Volume
            deployment.Spec.Template.Spec.Volumes = new List<V1Volume>();
            var volume = CreateVolumeForPlsqlContainer(appName);
            deployment.Spec.Template.Spec.Volumes.Add(volume);

            var appContainer = CreateAppContainer(appName, image);           
            deployment.Spec.Template.Spec.Containers.Add(appContainer);

            var psqlContainer = CreatePsqlContainer(appName);
            deployment.Spec.Template.Spec.Containers.Add(psqlContainer);

            return deployment;
        }

        private V1Deployment CreateDeploymentHeader(string appName)
        {
            var deployment = new V1Deployment();
            deployment.Metadata = new V1ObjectMeta();
            deployment.Metadata.Name = appName;
            deployment.Metadata.Labels = new Dictionary<string, string>();
            deployment.Metadata.Labels[CREATOR_KEY] = CREATOR_VALUE;
            deployment.Spec = new V1DeploymentSpec();
            deployment.Spec.Selector = new V1LabelSelector();
            deployment.Spec.Selector.MatchLabels = new Dictionary<string, string>();
            deployment.Spec.Selector.MatchLabels["app"] = appName;
            deployment.Spec.Template = new V1PodTemplateSpec();
            deployment.Spec.Template.Metadata = new V1ObjectMeta();
            deployment.Spec.Template.Metadata.Labels = new Dictionary<string, string>();
            deployment.Spec.Template.Metadata.Labels["app"] = appName;
            deployment.Spec.Template.Spec = new V1PodSpec();
            deployment.Spec.Template.Spec.Containers = new List<V1Container>();
            return deployment;
        }

        private V1Volume CreateVolumeForPlsqlContainer(string appName)
        {
            var volume = new V1Volume();
            volume.Name = "initscript";
            volume.ConfigMap = new V1ConfigMapVolumeSource();
            volume.ConfigMap.Name = appName;
            return volume;
        }

        private V1Container CreateAppContainer(string appName, string image)
        {
            var appContainer = new V1Container();
            appContainer.Name = appName;
            appContainer.Image = image;
            appContainer.Env = new List<V1EnvVar>();
            appContainer.Env.Add(new V1EnvVar()
            {
                Name = "PathBase",
                Value = $"endpoint/{appName}",
            });
            appContainer.Env.Add(new V1EnvVar()
            {
                Name = "ConnectionStrings__SampleWebAppContext",
                Value = $"Server=localhost;Port=5432;Database=test;User Id=postgres;Password=verysecret",
            });
            appContainer.Env.Add(new V1EnvVar()
            {
                Name = "DatabaseTech",
                Value = $"NpgSql",
            });
            return appContainer;
        }

        private V1Container CreatePsqlContainer(string appName)
        {
            var psqlContainer = new V1Container();
            psqlContainer.Name = $"{appName}psql";
            psqlContainer.Image = "postgres";
            psqlContainer.Env = new List<V1EnvVar>();
            psqlContainer.Env.Add(new V1EnvVar()
            {
                Name = "POSTGRES_PASSWORD",
                Value = $"verysecret",
            });
            psqlContainer.VolumeMounts = new List<V1VolumeMount>();
            psqlContainer.VolumeMounts.Add(new V1VolumeMount()
            {
                Name = "initscript",
                MountPath = "/docker-entrypoint-initdb.d",
            });
            return psqlContainer;
        }

        private V1ConfigMap CreateConfigMap(string appName, string initScript)
        {
            var configMap = new V1ConfigMap();
            configMap.Metadata = new V1ObjectMeta();
            configMap.Metadata.Name = appName;
            configMap.Data = new Dictionary<string, string>();
            configMap.Data["script.sql"] = initScript;
            return configMap;
        }
    }
}
