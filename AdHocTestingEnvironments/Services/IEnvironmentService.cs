using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.Kubernetes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IEnvironmentService
    {
        Task<string> StartEnvironmentInstance(string appName);

        Task<string> StopEnvironmentInstance(string instanceName);

        Task<IList<AdHocTestingEnvironmentInstanceViewModel>> ListEnvironmentInstances();

        Task<IList<TestingEnvironemntViewModel>> ListEnvironmetns();
    }
}
