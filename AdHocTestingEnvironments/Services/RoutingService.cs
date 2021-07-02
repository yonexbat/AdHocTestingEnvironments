using AdHocTestingEnvironments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public class RoutingService : IRoutingService
    {
        private readonly IDictionary<string, RoutingEntry> _routes = new Dictionary<string, RoutingEntry>();

        public RoutingEntry AddItem(RoutingEntry item)
        {
            _routes[item.Name] = item;
            return item;
        }

        public void DeleteItem(string app)
        {
            _routes.Remove(app);
        }

        public RoutingEntry GetItem(string app)
        {
            return _routes[app];
        }

        public IList<RoutingEntry> GetItems()
        {
            return _routes.Values.ToList();
        }
    }
}
