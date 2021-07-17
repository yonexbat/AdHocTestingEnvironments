using AdHocTestingEnvironments.Model.Kubernetes;
using k8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IKubernetesObjectBuilder
    {
        IList<IKubernetesObject> CreateObjectDefenitions(CreateEnvironmentInstanceData instanceInfo);
    }
}
