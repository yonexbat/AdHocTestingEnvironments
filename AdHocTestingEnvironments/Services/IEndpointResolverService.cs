using AdHocTestingEnvironments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IEndpointResolverService
    {
        IList<EndpointEntry> GetCustomItem();

        EndpointEntry GetItem(string app);

        void DeleteCustomItem(string app);

        EndpointEntry AddCustomItem(EndpointEntry item);
    }
}
