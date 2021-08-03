using AdHocTestingEnvironments.Data;
using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Entities;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class EnvironmentService: IEnvironmentService
    {
        private readonly ILogger _logger;
        private readonly AdHocTestingEnvironmentsContext _dbContext;

        public EnvironmentService(
            ILogger<EnvironmentService> logger,
            AdHocTestingEnvironmentsContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        public async Task<ApplicationInfo> GetApplication(string name)
        {
            ApplicationInfoEntity info = await _dbContext.InfoEntities.Where(x => x.Name == name)
                .SingleAsync();

            var res = new ApplicationInfo()
            {
                Name = info.Name,
                ContainerImage = info.ContainerImage,
                HasDatabase = info.HasDatabase,
                InitSql = info.InitSql,
            };
            return res;
        }

        public async Task<IList<Application>> ListEnvironmetns()
        {
            return await _dbContext.InfoEntities
                .OrderBy(x => x.Name)
                .Select(x => new Application { Name = x.Name })
                .ToListAsync();
        }
    }
}
