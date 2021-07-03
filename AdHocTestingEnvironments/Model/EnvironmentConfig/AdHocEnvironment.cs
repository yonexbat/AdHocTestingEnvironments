using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model
{
    public class AdHocEnvironment
    {
        public string Name { get; set; }

        public string ContainerImage { get; set; }
        
        public string InitSql { get; set; }
    }
}
