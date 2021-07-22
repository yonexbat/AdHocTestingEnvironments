using AdHocTestingEnvironments.Model.GitService;
using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services.Interfaces;
using k8s;
using k8s.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class GitClientService : IGitClientService
    {

        private readonly string _gitUrl;
        private readonly ILogger _logger;
        private readonly IKubernetesObjectBuilder _kubernetesObjectBuilder;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _branch;

        public GitClientService(
            IConfiguration configuration, 
            IKubernetesObjectBuilder kubernetesObjectBuilder,
            ILogger<GitClientService> logger)
        {
            _gitUrl = configuration.GetValue<string>("GitUrl");
            _userName = configuration.GetValue<string>("GitUser");
            _password = configuration.GetValue<string>("GitPw");
            _branch = configuration.GetValue<string>("GitBranch");
            _kubernetesObjectBuilder = kubernetesObjectBuilder;
            _logger = logger;
        }

        public Task CheckOut()
        {
            string localPath = GetPathToGitRepository();
            _logger.LogInformation("Local git path: {0}, Git Repo: {1}", localPath, _gitUrl);
            
            try
            {
                // delete directory first.
                ForceDeleteDirectory(localPath);
                Repository.Clone(_gitUrl, localPath);
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }            
            return Task.CompletedTask;
        }

        public Task<IList<EnvironmentInstance>> GetEnvironments()
        {
            Kustomize kustomize = GetKustomize();
            IList<EnvironmentInstance> services = kustomize
                .Resources
                .Where(x => x.EndsWith("-service.yaml"))
                .Select(x => new EnvironmentInstance() {
                    Name = x.Substring(0, x.Length - 13)
                }).ToList();

            return Task.FromResult(services);
        }

        public async Task<string> StartEnvironment(CreateEnvironmentInstanceData instanceInfo)
        {            
            IList<IKubernetesObject> objects = _kubernetesObjectBuilder.CreateObjectDefinitions(instanceInfo);
            string localPath = GetPathToGitRepository();
            using (Repository repo = new Repository(localPath))
            {
                foreach (Object kubernetesObject in objects)
                {

                    string path = kubernetesObject switch 
                    {
                        V1ConfigMap configMap => await SaveKubernetesObjToFile(configMap, "Configmap", instanceInfo.Name),
                        V1Deployment deployment => await SaveKubernetesObjToFile(deployment, "Deployment", instanceInfo.Name),
                        V1Service service => await SaveKubernetesObjToFile(service, "Service", instanceInfo.Name),
                        _ => throw new ArgumentException($"Type {kubernetesObject.GetType().Name} not suported."),
                    };


                    Commands.Stage(repo, path);

                    _logger.LogInformation("Created Kubernetes Object");
                }

                await AddToKustomizeFile(repo, instanceInfo);

                CommitChanges(repo);
                PushChanges(repo);
            }           
            return "Ok";
        }


        public async Task<string> StopEnvironment(string appName)
        {
            string localPath = GetPathToGitRepository();
            using (Repository repo = new Repository(localPath))
            {
                IList<string> fileList = GetPathList(appName);
                foreach (var file in fileList)
                {
                    File.Delete(file);
                    Commands.Stage(repo, file);
                }

                await RemoveFromKustomzeFile(repo, appName);

                CommitChanges(repo);
                PushChanges(repo);
            }
            return "Ok";
        }

        private async Task AddToKustomizeFile(Repository repo, CreateEnvironmentInstanceData instanceInfo)
        {
            Kustomize kustomize = GetKustomize();
            kustomize.Resources.Add($"{instanceInfo.Name}-configmap.yaml");
            kustomize.Resources.Add($"{instanceInfo.Name}-deployment.yaml");
            kustomize.Resources.Add($"{instanceInfo.Name}-service.yaml");

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(kustomize);
            var path = GetKustomizeFilePath();

            using (StreamWriter outputFile = new StreamWriter(path, false))
            {
                await outputFile.WriteAsync(yaml);
            }
            Commands.Stage(repo, path);
        }

        private async Task RemoveFromKustomzeFile(Repository repo, string appName)
        {
            Kustomize kustomize = GetKustomize();
            var path = GetKustomizeFilePath();

            kustomize.Resources = kustomize.Resources.Where(x => !x.StartsWith($"{appName}-")).ToList();

            var serializer = new SerializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();
            var yaml = serializer.Serialize(kustomize);

            using (StreamWriter outputFile = new StreamWriter(path, false))
            {
                await outputFile.WriteAsync(yaml);
            }
            Commands.Stage(repo, path);
        }

        private Kustomize GetKustomize()
        {
            string kustizefilePath = GetKustomizeFilePath();
            if (File.Exists(kustizefilePath))
            {
                var yaml =  File.ReadAllText(kustizefilePath);
                var deserializer = new DeserializerBuilder()
                      .WithNamingConvention(CamelCaseNamingConvention.Instance)
                      .Build();

                return deserializer.Deserialize<Kustomize>(yaml);
            }
            return new Kustomize() { Resources = new List<string>() };
        }

        private void PushChanges(Repository repo)
        {
            var remote = repo.Network.Remotes["origin"];
            var options = new PushOptions()
            {
                CredentialsProvider = (user, valid, hostname) =>
                {
                    return new UsernamePasswordCredentials { Username = _userName, Password = _password, };
                },
            };

            Signature signature = CreatGitSignature();

            // options.Credentials = credentials;
            var pushRefSpec = $"refs/heads/{_branch}";

            repo.Network.Push(remote, pushRefSpec, options);
        }

        private void CommitChanges(Repository repo)
        {
            Signature signature = CreatGitSignature();
            repo.Commit("commit by adhoctestingenvironments", signature, signature);
        }

        private Signature CreatGitSignature()
        {
            return new Signature("adhoctestingenvironments", "hello@adhoctestingenvironments.com", DateTimeOffset.Now);
        }

        private async Task<string> SaveKubernetesObjToFile(IKubernetesObject kubernetesObject, string kind, string instanceName)
        {
            kubernetesObject.Kind = kind;
            kubernetesObject.ApiVersion = "v1";
            Object kubernetesObjAsPurObject = (Object) kubernetesObject;
            string yamlContent = Yaml.SaveToString(kubernetesObjAsPurObject);
            return await SaveToFile(yamlContent, $"{instanceName}-{kind.ToLower()}.yaml");
        }

        private async Task<string> SaveToFile(string content, string filename)
        {
            string directory = GetKustomizeDirecotryPath();
            Directory.CreateDirectory(directory);

            string path = $"{directory}{filename}";
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                await outputFile.WriteAsync(content);
            }
            return path;
        }

        private IList<string> GetPathList(string appName)
        {
            string path = GetKustomizeDirecotryPath();
            return new List<string>
            {
                $"{path}{appName}-configmap.yaml",
                $"{path}{appName}-deployment.yaml",
                $"{path}{appName}-service.yaml",
            };
        }

        private string GetPathToGitRepository()
        {
            return $"{Path.GetTempPath()}git";
        }

        private string GetKustomizeDirecotryPath()
        {
            string directory = $"{GetPathToGitRepository()}{Path.DirectorySeparatorChar}kustomize{Path.DirectorySeparatorChar}";
            return directory;
        }

        private string GetKustomizeFilePath()
        {
            string directory = GetKustomizeDirecotryPath();
            return $"{directory}kustomize.yaml";
        }

        private void ForceDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                _logger.LogInformation("Directory already exist. Will delete it and recreate it later on.");
                var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);
                _logger.LogInformation("Directory deleted successfully");
            } 
            else
            {
                _logger.LogInformation("Directory does not exist");
            }
        }
    }
}
