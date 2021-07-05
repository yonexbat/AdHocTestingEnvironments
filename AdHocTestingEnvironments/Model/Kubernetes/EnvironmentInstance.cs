using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Kubernetes
{
    public class EnvironmentInstance
    {
        public string Name { get; set; }

        public string Status { get; set; }

        public DateTime? StartTime { get; set; }

        public int? NumHoursToRun { get; set; }
    }
}
