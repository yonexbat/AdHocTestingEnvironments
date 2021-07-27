using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services.Implementations;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdHocTestingEnvironmentsTests
{
    public class EnvironmentKillerServiceTest
    {
        [Fact]
        public async Task StopEnvironmentInstanceOk()
        {
            var currentTimeServiceMock = new Mock<ICurrentTimeService>();
            currentTimeServiceMock.Setup(cts => cts.GetCurrentUtcTime()).Returns(new DateTimeOffset(2021, 7, 17, 1, 0, 0, TimeSpan.Zero));

            var kubernetesClientMock = new Mock<IEnvironmentService>();
            var instanceListTask = Task.FromResult<IList<EnvironmentInstance>>(new List<EnvironmentInstance>()
                {
                    new EnvironmentInstance() { Name = "One", Status = "Running", StartTime = new DateTime(2021, 7, 17, 1, 0, 0), NumHoursToRun = 1 },
                    new EnvironmentInstance() { Name = "Two", Status = "Running", StartTime = new DateTime(2021, 7, 17, 1, 0, 0), NumHoursToRun = 2 },
                    new EnvironmentInstance() { Name = "Three", Status = "Running", StartTime = new DateTime(2021, 7, 17, 1, 0, 0), NumHoursToRun = 3 },
                    new EnvironmentInstance() { Name = "Four", Status = "Running", StartTime =  new DateTime(2021, 7, 17, 1, 0, 0), NumHoursToRun = 4},
                }
            );            
            int deleteCount = 0;
            kubernetesClientMock.Setup(kc => kc.ListEnvironmentInstances())
                .Returns(instanceListTask)
                .Callback(() => deleteCount++);

            var inMemorySettings = new Dictionary<string, string> {
                {"EnironmentKillerServiceEnabled", "false"},               
            };

            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();


            var service =  new EnvironmentKillerService(mockConfiguration, kubernetesClientMock.Object, currentTimeServiceMock.Object, new Mock<ILogger<EnvironmentKillerService>>().Object);

            await service.KillDueEnvironments();
            Assert.Equal(1, deleteCount);
        }

    }
}
