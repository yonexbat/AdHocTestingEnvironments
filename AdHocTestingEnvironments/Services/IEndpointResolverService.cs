using AdHocTestingEnvironments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IEndpointResolverService
    {
        IList<EndpointEntry> GetItems();

        EndpointEntry GetItem(string app);

        void DeleteItem(string app);

        EndpointEntry AddItem(EndpointEntry item);
    }
}
