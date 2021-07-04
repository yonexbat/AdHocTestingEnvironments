using AdHocTestingEnvironments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public class EndpointResolverService : IEndpointResolverService
    {
        private readonly IDictionary<string, EndpointEntry> _routes = new Dictionary<string, EndpointEntry>();

        public EndpointEntry AddItem(EndpointEntry item)
        {
            _routes[item.Name] = item;
            return item;
        }

        public void DeleteItem(string app)
        {
            _routes.Remove(app);
        }

        public EndpointEntry GetItem(string app)
        {
            return _routes[app];
        }

        public IList<EndpointEntry> GetItems()
        {
            return _routes.Values.ToList();
        }
    }
}
