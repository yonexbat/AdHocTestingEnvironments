using AdHocTestingEnvironments.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IEndpointResolverService
    {
        Task<IList<EndpointEntry>> GetCustomItem();

        Task<EndpointEntry> GetItem(string app);

        Task DeleteCustomItem(string app);

        Task<EndpointEntry> AddCustomItem(EndpointEntry item);
    }
}
