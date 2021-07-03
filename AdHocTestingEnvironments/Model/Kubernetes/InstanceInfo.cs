using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Kubernetes
{
    public class InstanceInfo
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string InitSqlScript { get; set; }
    }
}
