using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IGitClientService : IKubernetesClientService
    {
        Task CheckOut();

        // Hack, dirty and what not.
        void ChangeUserNameAndPassword(string usernam, string password);
    }
}
