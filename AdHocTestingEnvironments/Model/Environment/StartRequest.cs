using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Environment
{
    public class StartRequest
    {
        public string ApplicationName { get; set; }

        public int NumHoursToRun { get; set; }
    }
}
