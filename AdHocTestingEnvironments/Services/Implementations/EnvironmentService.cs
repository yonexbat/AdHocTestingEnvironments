using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.EnvironmentConfig;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class EnvironmentService: IEnvironmentService
    {
        private readonly EnvironmentConfigOptions _environmentConfigOptions;
        private readonly ILogger _logger;

        public EnvironmentService(
            IOptions<EnvironmentConfigOptions> options,
            ILogger<EnvironmentService> logger)
        {
            _environmentConfigOptions = options.Value;
            _logger = logger;
        }

        public Task<ApplicationInfo> GetApplication(string name)
        {
            AdHocEnvironmentConfig config = _environmentConfigOptions.Environments.Single(x => x.Name == name);
            var res = new ApplicationInfo()
            {
                Name = config.Name,
                ContainerImage = config.ContainerImage,
                HasDatabase = config.HasDatabase,
                InitSql = config.InitSql,
            };
            return Task.FromResult(res);
        }

        public Task<IList<Application>> ListEnvironmetns()
        {
            IList<Application> res = _environmentConfigOptions.Environments.Select(x => new Application()
            {
                Name = x.Name,
            }).ToList();
            return Task.FromResult(res);
        }
    }
}
