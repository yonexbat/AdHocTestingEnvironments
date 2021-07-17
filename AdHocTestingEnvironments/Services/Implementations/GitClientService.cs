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

        public async Task CheckOut()
        {
            string localPath = GetPath();
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
        }

        public Task<IList<EnvironmentInstance>> GetEnvironments()
        {
            throw new NotImplementedException();
        }

        public async Task<string> StartEnvironment(CreateEnvironmentInstanceData instanceInfo)
        {
            
            IList<IKubernetesObject> objects = _kubernetesObjectBuilder.CreateObjectDefenitions(instanceInfo);
            string localPath = GetPath();
            using (Repository repo = new Repository(localPath))
            {
                foreach (Object kubernetesObject in objects)
                {

                    string path;
                    switch (kubernetesObject)
                    {
                        case V1ConfigMap configMap:
                            string yamlContent = Yaml.SaveToString(configMap);
                            path = await SaveToFile(yamlContent, $"{instanceInfo.Name}-configmap.yaml");
                            break;
                        case V1Deployment deployment:
                            yamlContent = Yaml.SaveToString(deployment);
                            path = await SaveToFile(yamlContent, $"{instanceInfo.Name}-deployment.yaml");
                            break;
                        case V1Service service:
                            yamlContent = Yaml.SaveToString(service);
                            path = await SaveToFile(yamlContent, $"{instanceInfo.Name}-service.yaml");
                            break;
                        default:
                            throw new ArgumentException("not supported");
                    }

                    Commands.Stage(repo, path);

                    _logger.LogInformation("Created Kubernetes Object");
                }
                CommitChanges(repo);
                PushChanges(repo);
            }           
            return "Ok";
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

            Signature signature = new Signature("adhoctestingenvironments", "hello@adhoctestingenvironments.com", DateTimeOffset.Now);
            {

            };

            // options.Credentials = credentials;
            var pushRefSpec = $"refs/heads/{_branch}";

            repo.Network.Push(remote, pushRefSpec, options);
        }

        private void CommitChanges(Repository repo)
        {
            Signature signature = new Signature("adhoctestingenvironments", "hello@adhoctestingenvironments.com", DateTimeOffset.Now);
            {

            };

            repo.Commit("egotistic commit", signature, signature);
        }

        private async Task<string> SaveToFile(string content, string filename)
        {
            string directory = $"{GetPath()}{Path.DirectorySeparatorChar}kustomize{Path.DirectorySeparatorChar}";
            Directory.CreateDirectory(directory);

            string path = $"{directory}{filename}";
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                await outputFile.WriteAsync(content);
            }
            return path;
        }

        public Task<string> StopEnvironment(string appName)
        {
            throw new NotImplementedException();
        }

        private string GetPath()
        {
            return $"{Path.GetTempPath()}git";
        }

        private void ForceDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                _logger.LogInformation("Directory already exist. Will delete it and recreate it");
                var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);
            } 
            else
            {
                _logger.LogInformation("Directory does not exist");
            }
        }
    }
}
