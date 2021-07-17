using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public class EnvironmentKillerService : IEnvironmentKillerService
    {

        private readonly ILogger _logger;
        private readonly IKubernetesClientService _kubernetesClientService;
        private readonly ICurrentTimeService _currentTimeService;

        public EnvironmentKillerService(IKubernetesClientService kubernetesClientService, ICurrentTimeService currentTimeService, ILogger<EnvironmentKillerService> logger)
        {
            _logger = logger;
            _kubernetesClientService = kubernetesClientService;
            _currentTimeService = currentTimeService;
        }

        public async Task KillDueEnvironments()
        {
            _logger.LogInformation($"Killing environments if due");

            var list = await _kubernetesClientService.GetEnvironments();
            DateTimeOffset currentTime = _currentTimeService.GetCurrentUtcTime();
            var envsToDelete = list
                .Where(x => x.StartTime.HasValue)
                .Where(x => (x.StartTime + new TimeSpan(x.NumHoursToRun ?? 1, 0, 0)) < currentTime);
            
            foreach(var instance in envsToDelete)
            {
                _logger.LogInformation($"Killing environment {instance.Name}.");
                await _kubernetesClientService.StopEnvironment(instance.Name);
            }
        }
    }
}
