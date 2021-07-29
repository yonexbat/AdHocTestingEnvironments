using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class EnvironmentKillerService : IEnvironmentKillerService
    {

        private readonly ILogger _logger;
        private readonly IEnvironmentInstanceService _environmentService;
        private readonly ICurrentTimeService _currentTimeService;
        private readonly bool _enabled;

        public EnvironmentKillerService(IConfiguration configuration, IEnvironmentInstanceService environmentService, ICurrentTimeService currentTimeService, ILogger<EnvironmentKillerService> logger)
        {
            _logger = logger;
            _environmentService = environmentService;
            _currentTimeService = currentTimeService;
            _enabled = configuration.GetValue<bool>("EnironmentKillerServiceEnabled");
        }

        public async Task KillDueEnvironments()
        {
            _logger.LogInformation($"Killing environments if due");
            if(!_enabled)
            {
                _logger.LogInformation($"EnvironmentKillerService is disabled.");
                return;
            }

            var list = await _environmentService.ListEnvironmentInstances();
            DateTimeOffset currentTime = _currentTimeService.GetCurrentUtcTime();
            var envsToDelete = list
                .Where(x => x.StartTime.HasValue)
                .Where(x => x.StartTime + new TimeSpan(x.NumHoursToRun ?? 1, 0, 0) < currentTime);

            foreach (EnvironmentInstance instance in envsToDelete)
            {
                _logger.LogInformation($"Killing environment {instance.Name}.");
                await _environmentService.StopEnvironmentInstance(instance.Name);
            }
        }
    }
}
