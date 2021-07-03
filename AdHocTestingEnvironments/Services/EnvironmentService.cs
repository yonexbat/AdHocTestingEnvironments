using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.EnvironmentConfig;
using AdHocTestingEnvironments.Model.Kubernetes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
   

    public class EnvironmentService : IEnvironmentService
    {

        private readonly EnvironmentConfigOptions _environmentConfigOptions;
        private readonly IKubernetesClient _kubernetesClient;
        private readonly IRoutingService _routingService;
        private readonly ILogger _logger;
        private Random random = new Random();

        public EnvironmentService(
            IOptions<EnvironmentConfigOptions> options, 
            IKubernetesClient kubernetesClient,
            IRoutingService routingService,
            ILogger<EnvironmentService> logger)
        {
            _environmentConfigOptions = options.Value;
            _kubernetesClient = kubernetesClient;
            _routingService = routingService;
            _logger = logger;
        }

        public async Task<IList<AdHocTestingEnvironmentInstanceViewModel>> ListEnvironmentInstances()
        {
            var env = await _kubernetesClient.GetEnvironments();
            return env.Select(x => new AdHocTestingEnvironmentInstanceViewModel()
            {
                Name = x.Name,
                Status = x.Status,
            }).ToList();
                
        }

        public async Task<IList<TestingEnvironemntViewModel>> ListEnvironmetns()
        {
            return _environmentConfigOptions.Environments.Select(x => new TestingEnvironemntViewModel()
            {
                Name = x.Name,
            }).ToList();
        }

        public async Task<string> StartEnvironmentInstance(string appName)
        {
            string randomString = CreateRandomString();
            string instanceName = $"{appName}{randomString}";
            AdHocEnvironmentConfig config = _environmentConfigOptions.Environments.Where(x => x.Name == appName).Single();
            await _kubernetesClient.StartEnvironment(new CreateEnvironmentInstanceData()
            {
                Image = config.ContainerImage,
                Name = instanceName,
                InitSqlScript = config.InitSql,
            });

            _routingService.AddItem(new RoutingEntry()
            {
                Name = instanceName,
                Destination = $"http://{instanceName}",
            }); 

            return instanceName;
        }

        public async Task<string> StopEnvironmentInstance(string instanceName)
        {
            _routingService.DeleteItem(instanceName);
            await _kubernetesClient.StopEnvironment(instanceName);
            return "Ok";
        }

        private string CreateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 3)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
