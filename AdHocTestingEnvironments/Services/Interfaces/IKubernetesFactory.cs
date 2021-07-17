using k8s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface IKubernetesFactory
    {
        IKubernetes CreateClient();
    }
}
