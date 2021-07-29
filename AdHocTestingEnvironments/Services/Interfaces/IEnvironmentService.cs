using AdHocTestingEnvironments.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IEnvironmentService
    {
        Task<IList<Application>> ListEnvironmetns();

        Task<ApplicationInfo> GetApplication(string name);
    }
}
