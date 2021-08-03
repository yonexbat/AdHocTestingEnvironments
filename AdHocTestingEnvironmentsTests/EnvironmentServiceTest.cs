using AdHocTestingEnvironments.Data;
using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Entities;
using AdHocTestingEnvironments.Model.EnvironmentConfig;
using AdHocTestingEnvironments.Services.Implementations;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdHocTestingEnvironmentsTests
{
    public class EnvironmentServiceTest
    {        
       [Fact]
       public async Task ListEnvironmentsOk()
       {
            Guid guid = Guid.NewGuid();
            using (AdHocTestingEnvironmentsContext dbContext = MockDatabase.CreateDbContext(guid)) {
                ApplicationInfoEntity entity = new ApplicationInfoEntity()
                {
                    Name = "text",
                    ContainerImage = "contimage",
                    HasDatabase = false,
                };
                await dbContext.AddAsync(entity);
                await dbContext.SaveChangesAsync();
            }

            using (AdHocTestingEnvironmentsContext dbContext = MockDatabase.CreateDbContext(guid))
            {
                var service = CreateService(dbContext);
                var x = await service.ListEnvironmetns();
                Assert.Equal(1, x.Count);
            }
       }

        private IEnvironmentService CreateService(AdHocTestingEnvironmentsContext context)
        {           
            var mockLogger = new Mock<ILogger<EnvironmentService>>().Object;
            var service = new EnvironmentService(mockLogger, context);
            return service;
        }
    }
}
