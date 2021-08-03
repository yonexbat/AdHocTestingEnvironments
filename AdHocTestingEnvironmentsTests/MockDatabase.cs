using AdHocTestingEnvironments.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdHocTestingEnvironmentsTests
{
    public class MockDatabase
    {
        public static AdHocTestingEnvironmentsContext CreateDbContext(Guid guid)
        {
            var options = new DbContextOptionsBuilder<AdHocTestingEnvironmentsContext>()
               .UseInMemoryDatabase(guid.ToString())
               .Options;

            return new AdHocTestingEnvironmentsContext(options);
        }
    }
}
