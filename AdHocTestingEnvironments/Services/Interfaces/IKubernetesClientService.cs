using AdHocTestingEnvironments.Model.Kubernetes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IKubernetesClientService
    {
        public Task<string> StartEnvironment(CreateEnvironmentInstanceData instanceInfo);

        public Task<string> StopEnvironment(string appName);

        public Task<IList<EnvironmentInstance>> GetEnvironments();
    }
}
