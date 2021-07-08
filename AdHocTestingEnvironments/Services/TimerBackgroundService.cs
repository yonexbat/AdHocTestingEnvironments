using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public class TimerBackgroundService : BackgroundService, IDisposable
    {

        private int executionCount = 0;
        private readonly ILogger<TimerBackgroundService> _logger;
        private readonly IServiceProvider _services;

        public TimerBackgroundService(ILogger<TimerBackgroundService> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;

        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "BackgroundService is working. Count: {Count}", count);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    IEnvironmentKillerService environmentKillerService = scope.ServiceProvider.GetRequiredService<IEnvironmentKillerService>();
                    await environmentKillerService.KillDueEnvironments();
                }


                await Task.Delay(1000*60*60, stoppingToken);
            }            
        }

        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service is stopping");

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
