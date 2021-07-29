using AdHocTestingEnvironments.Model;
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
           IEnvironmentService service = CreateService();
           var res = await service.ListEnvironmetns();
           Assert.True(res.Any());
       }

        private IEnvironmentService CreateService()
        {
            EnvironmentConfigOptions options = new EnvironmentConfigOptions()
            {
                Environments = new List<AdHocEnvironmentConfig>()
                {
                    new AdHocEnvironmentConfig()
                    {
                        Name = "sampleapp",
                        ContainerImage = "myimage",
                        InitSql = "CREATE DATABASE TEST;",
                    },
                },
            };
            var optionsMock = new Mock<IOptions<EnvironmentConfigOptions>>();
            optionsMock.Setup(ap => ap.Value).Returns(options);


            var mockLogger = new Mock<ILogger<EnvironmentService>>().Object;


            IEnvironmentService service = new EnvironmentService(optionsMock.Object,
                mockLogger);

            return service;
        }
    }
}
