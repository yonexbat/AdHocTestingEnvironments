using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class EndpointResolverService : IEndpointResolverService
    {
        private readonly ILogger<EndpointResolverService> _logger;

        public EndpointResolverService(ILogger<EndpointResolverService> logger)
        {
            _logger = logger;
        }

        private readonly IDictionary<string, EndpointEntry> _routes = new Dictionary<string, EndpointEntry>();

        public EndpointEntry AddCustomItem(EndpointEntry item)
        {
            _routes[item.Name] = item;
            return item;
        }

        public void DeleteCustomItem(string app)
        {
            _routes.Remove(app);
        }

        public EndpointEntry GetItem(string app)
        {
            if (_routes.ContainsKey(app))
            {
                _logger.LogInformation("Explicit route found");
                return _routes[app];
            }

            _logger.LogInformation("Creating implicit route");
            return new EndpointEntry()
            {
                Destination = $"http://{app}",
            };
        }

        public IList<EndpointEntry> GetCustomItem()
        {
            return _routes.Values.ToList();
        }
    }
}
