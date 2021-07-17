using AdHocTestingEnvironments.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class CurrentTimeService : ICurrentTimeService
    {
        public DateTimeOffset GetCurrentUtcTime()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
