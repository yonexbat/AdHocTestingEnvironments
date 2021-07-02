using AdHocTestingEnvironments.Model.EnvironmentConfig;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
   

    public class EnvironmentService : IEnvironmentService
    {
        public EnvironmentService(IOptions<EnvironmentConfigOptions> options)
        {

        }
    }
}
