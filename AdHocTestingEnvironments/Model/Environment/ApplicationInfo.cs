using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Environment
{
    public class ApplicationInfo
    {
        public string Name { get; set; }
        public string ContainerImage { get; set; }
        public bool HasDatabase { get; set; }
        public string InitSql { get; set; }
    }
}
