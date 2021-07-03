﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IKubernetesClient
    {
        public Task<string> StartEnvironment(string image, string appName, string initScript);
    }
}
