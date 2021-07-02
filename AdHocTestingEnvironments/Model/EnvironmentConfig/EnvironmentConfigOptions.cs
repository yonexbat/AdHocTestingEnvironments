using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.EnvironmentConfig
{
    public class EnvironmentConfigOptions
    {
        public IList<AdHocEnvironment> Environments { get; set; } = new List<AdHocEnvironment>();
    }
}
