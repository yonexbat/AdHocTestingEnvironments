using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Interfaces
{
    public interface ICurrentTimeService
    {
        DateTimeOffset GetCurrentUtcTime();
    }
}
