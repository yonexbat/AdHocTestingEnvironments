using AdHocTestingEnvironments.Data;
using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Entities;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class EndpointResolverService : IEndpointResolverService
    {
        private readonly ILogger<EndpointResolverService> _logger;
        private readonly AdHocTestingEnvironmentsContext _dbContext;

        public EndpointResolverService(AdHocTestingEnvironmentsContext context, ILogger<EndpointResolverService> logger)
        {
            _logger = logger;
            _dbContext = context;
        }


        public async Task<EndpointEntry> AddCustomItem(EndpointEntry item)
        {
            EndpointEntryEntity entity = new EndpointEntryEntity()
            {
                Destination = item.Destination,
                Name = item.Name,
            };

            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return item;
        }

        public async Task DeleteCustomItem(string app)
        {
            var item =  await _dbContext.Endpoints.Where(x => x.Name == app)
                .SingleAsync();

            _dbContext.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<EndpointEntry> GetItem(string app)
        {

            var endpoint = await _dbContext.Endpoints.Where(x => x.Name == app)
               .SingleOrDefaultAsync();

            if (endpoint != null)
            {
                _logger.LogInformation("Explicit route found");
                return new EndpointEntry()
                {
                    Destination = endpoint.Destination,
                    Name = endpoint.Name,
                };
            }

            _logger.LogInformation("Creating implicit route");
            return new EndpointEntry()
            {
                Destination = $"http://{app}",
            };
        }

        public async Task<IList<EndpointEntry>> GetCustomItems()
        {
            return await _dbContext.Endpoints.Select(x => new EndpointEntry()
            {
                Name = x.Name,
                Destination = x.Destination,
            }).ToListAsync();
        }

        public async Task UpdateItem(EndpointEntry item)
        {
            var entity = await _dbContext.Endpoints.Where(x => x.Name == item.Name)
                 .SingleAsync();

            entity.Destination = entity.Destination;
            await _dbContext.SaveChangesAsync();
        }
    }
}
