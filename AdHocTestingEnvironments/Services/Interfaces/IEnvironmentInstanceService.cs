using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.Kubernetes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IEnvironmentInstanceService
    {
        Task<string> StartEnvironmentInstance(StartRequest dto);

        Task<string> StopEnvironmentInstance(string instanceName);

        Task<IList<EnvironmentInstance>> ListEnvironmentInstances();

        Task<IList<Application>> ListEnvironmetns();
    }
}
