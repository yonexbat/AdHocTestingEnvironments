using k8s;
using k8s.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public class KubernetesClient : IKubernetesClient
    {
        private readonly string _accessToken;
        private readonly string _host;
        private readonly ILogger _logger;

        public KubernetesClient(IConfiguration configuration, ILogger<KubernetesClient> logger)
        {
            _logger = logger;
            _host = configuration.GetValue<string>("KubernetesHost");
            _accessToken = configuration.GetValue<string>("KubernetesAccessToken");
        }

        public async Task<string> StartEnvironment(string image, string appName, string initScript)
        {
            _logger.LogInformation("Starging environment. Imgage: {0}, AppName: {1}.", image, appName);
            _logger.LogInformation("Api host: {0}", _host);

            KubernetesClientConfiguration config = new KubernetesClientConfiguration();
            config.AccessToken = _accessToken;
            config.Host = _host;
            config.SkipTlsVerify = true;            
            IKubernetes client = new Kubernetes(config);

            //ConfigMap
            var configMap = new V1ConfigMap();
            configMap.Metadata = new V1ObjectMeta();
            configMap.Metadata.Name = $"sql{appName}";
            configMap.Data = new Dictionary<string, string>();
            configMap.Data["script.sql"] = initScript;
            var resConfigMap = await client.CreateNamespacedConfigMapAsync(configMap, "default");



            // Deployment
            var deployment = new V1Deployment();
            deployment.Metadata = new V1ObjectMeta();
            deployment.Metadata.Name = $"{appName}";
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

            // Deployment Volume
            deployment.Spec.Template.Spec.Volumes = new List<V1Volume>();
            var volume = new V1Volume();
            volume.Name = "initscript";
            volume.ConfigMap = new V1ConfigMapVolumeSource();
            volume.ConfigMap.Name = $"sql{appName}";
            deployment.Spec.Template.Spec.Volumes.Add(volume);

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

            deployment.Spec.Template.Spec.Containers.Add(appContainer);

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

            deployment.Spec.Template.Spec.Containers.Add(psqlContainer);

            var resDeployment = await client.CreateNamespacedDeploymentAsync(deployment, "default");


            //Service
            V1Service service = new V1Service();
            service.Metadata = new V1ObjectMeta();
            service.Metadata.Name = appName;
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


            var resService = await client.CreateNamespacedServiceAsync(service, "default");

            return "Ok";
        }


    }
}
