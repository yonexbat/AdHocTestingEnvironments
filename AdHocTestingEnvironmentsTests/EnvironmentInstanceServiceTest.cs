using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.EnvironmentConfig;
using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services.Implementations;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Configuration;
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
    public class EnvironmentInstanceServiceTest
    {
        [Fact]
        public async Task ListEnvironmentInstancesOk()
        {
            IEnvironmentInstanceService service = CreateService();
            var res = await service.ListEnvironmentInstances();
            Assert.True(res.Any());
        }

        [Fact]
        public async Task StartEnvironmentInstanceOk()
        {
            IEnvironmentInstanceService service = CreateService();
            var startRquest = new StartRequest()
            {
                ApplicationName  = "sampleapp",
                NumHoursToRun = 2,

            };

            var start = await service.StartEnvironmentInstance(startRquest);
        }


        [Fact]
        public async Task StopEnvironmentInstanceOk()
        {
            IEnvironmentInstanceService service = CreateService();
            var startRquest = new StartRequest()
            {
                ApplicationName = "sampleapp",
                NumHoursToRun = 2,

            };
            var start = await service.StartEnvironmentInstance(startRquest);
            await service.StopEnvironmentInstance(start);
        }


        private IEnvironmentInstanceService CreateService()
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

            var kubernetesClientMock = new Mock<IKubernetesClientService>();
            var instanceListTask = Task.FromResult<IList<EnvironmentInstance>>(new List<EnvironmentInstance>()
                { 
                    new EnvironmentInstance() { Name = "test", Status = "Running" }, 
                    new EnvironmentInstance() { Name = "other", Status = "ContainerCreating"}, 
                }
            );
            kubernetesClientMock.Setup(kc => kc.GetEnvironments()).Returns(instanceListTask);

            var routingServiceMock = new Mock<IEndpointResolverService>();

            var mockLogger = new Mock<ILogger<EnvironmentInstanceService>>().Object;


            var environmentServiceMock = new Mock<IEnvironmentService>();
            Task<ApplicationInfo> appInfo = Task.FromResult<ApplicationInfo>(new ApplicationInfo()
            {

            });
            environmentServiceMock.Setup(s => s.GetApplication(It.IsAny<string>())).Returns(appInfo);

            IEnvironmentInstanceService service = new EnvironmentInstanceService(
                kubernetesClientMock.Object, 
                routingServiceMock.Object,
                environmentServiceMock.Object,
                mockLogger);

            return service;
        }
    }
}
