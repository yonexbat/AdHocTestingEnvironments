using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdHocTestingEnvironmentsTests
{
    public class YamlTest
    {
        [Fact]
        void SaveAsYaml()
        {
            V1Deployment deployment = CreateDeployment("sfsd", "dsfds");
            string configMapAsString = Yaml.SaveToString(deployment);
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
            deployment.Metadata.Labels["creator"] = "my";
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
