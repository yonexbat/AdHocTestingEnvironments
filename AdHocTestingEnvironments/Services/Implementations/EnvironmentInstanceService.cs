using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.EnvironmentConfig;
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

        private readonly EnvironmentConfigOptions _environmentConfigOptions;
        private readonly IKubernetesClientService _kubernetesClient;
        private readonly IEndpointResolverService _routingService;
        private readonly ILogger _logger;
        private Random random = new Random();

        public EnvironmentInstanceService(
            IOptions<EnvironmentConfigOptions> options,
            IKubernetesClientService kubernetesClient,
            IEndpointResolverService routingService,
            ILogger<EnvironmentInstanceService> logger)
        {
            _environmentConfigOptions = options.Value;
            _kubernetesClient = kubernetesClient;
            _routingService = routingService;
            _logger = logger;
        }

        public async Task<IList<EnvironmentInstance>> ListEnvironmentInstances()
        {
            return await _kubernetesClient.GetEnvironments();
        }

        public Task<IList<Application>> ListEnvironmetns()
        {
            IList<Application> res = _environmentConfigOptions.Environments.Select(x => new Application()
            {
                Name = x.Name,
            }).ToList();
            return Task.FromResult(res);
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
            AdHocEnvironmentConfig config = _environmentConfigOptions.Environments.Where(x => x.Name == startRequest.ApplicationName).Single();

            await _kubernetesClient.StartEnvironment(new CreateEnvironmentInstanceData()
            {
                Image = config.ContainerImage,
                InitSqlScript = config.InitSql,
                Name = instanceName,
                NumHoursToRun = startRequest.NumHoursToRun,
                HasDatabase = config.HasDatabase,
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

