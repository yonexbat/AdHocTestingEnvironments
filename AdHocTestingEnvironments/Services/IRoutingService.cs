using AdHocTestingEnvironments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IRoutingService
    {
        IList<RoutingEntry> GetItems();

        RoutingEntry GetItem(string app);

        void DeleteItem(string app);

        RoutingEntry AddItem(RoutingEntry item);
    }
}
