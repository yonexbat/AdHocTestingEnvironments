using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{


    public class EnvironmentInstanceService : IEnvironmentInstanceService
    {

        private readonly IKubernetesClientService _kubernetesClient;
        private readonly IEndpointResolverService _routingService;
        private readonly ILogger _logger;
        private readonly IEnvironmentService _environmentService;
        private Random random = new Random();

        public EnvironmentInstanceService(
            IKubernetesClientService kubernetesClient,
            IEndpointResolverService routingService,
            IEnvironmentService environmentService,
            ILogger<EnvironmentInstanceService> logger)
        {
            _kubernetesClient = kubernetesClient;
            _routingService = routingService;
            _environmentService = environmentService;
            _logger = logger;
        }

        public async Task<IList<EnvironmentInstance>> ListEnvironmentInstances()
        {
            return await _kubernetesClient.GetEnvironments();
        }


        public async Task<string> StartEnvironmentInstance(StartRequest startRequest)
        {
            _logger.LogInformation("Starting environment: {0}", startRequest?.ApplicationName);
            var environments = await ListEnvironmentInstances();
            if (environments.Count > 3)
            {
                throw new ArgumentException($"Max 3 environment instances allowed");
            }

            string randomString = CreateRandomString();
            string instanceName = $"{startRequest.ApplicationName}{randomString}";
            ApplicationInfo appInfo = await _environmentService.GetApplication(startRequest?.ApplicationName);

            await _kubernetesClient.StartEnvironment(new CreateEnvironmentInstanceData()
            {
                Image = appInfo.ContainerImage,
                InitSqlScript = appInfo.InitSql,
                Name = instanceName,
                NumHoursToRun = startRequest.NumHoursToRun,
                HasDatabase = appInfo.HasDatabase,
            });

            _routingService.AddCustomItem(new EndpointEntry()
            {
                Name = instanceName,
                Destination = $"http://{instanceName}",
            });

            return instanceName;
        }

        public async Task<string> StopEnvironmentInstance(string instanceName)
        {
            _logger.LogInformation("Stopping environment: {0}", instanceName);
            await _kubernetesClient.StopEnvironment(instanceName);
            return "Ok";
        }

        private string CreateRandomString()
        {
            const string chars = "abcdefghiklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 3)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}

