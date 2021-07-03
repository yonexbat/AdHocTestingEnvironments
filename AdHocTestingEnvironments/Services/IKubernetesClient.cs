using AdHocTestingEnvironments.Model.Kubernetes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IKubernetesClient
    {
        public Task<string> StartEnvironment(InstanceInfo instanceInfo);

        public Task<string> StopEnvironment(string appName);
    }
}
