﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services
{
    public interface IEnvironmentKillerService
    {
        Task KillDueEnvironments();
    }
}