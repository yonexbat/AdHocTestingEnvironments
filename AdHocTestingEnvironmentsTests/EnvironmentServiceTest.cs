﻿using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Model.EnvironmentConfig;
using AdHocTestingEnvironments.Model.Kubernetes;
using AdHocTestingEnvironments.Services;
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
    public class EnvironmentServiceTest
    {
        [Fact]
        public async Task ListEnvironmentsOk()
        {
            IEnvironmentService service = CreateService();
            var res = await service.ListEnvironmetns();
            Assert.True(res.Any());
        }

        [Fact]
        public async Task ListEnvironmentInstancesOk()
        {
            IEnvironmentService service = CreateService();
            var res = await service.ListEnvironmentInstances();
            Assert.True(res.Any());
        }

        [Fact]
        public async Task StartEnvironmentInstanceOk()
        {
            IEnvironmentService service = CreateService();
            var start = await service.StartEnvironmentInstance("sampleapp");
        }


        [Fact]
        public async Task StopEnvironmentInstanceOk()
        {
            IEnvironmentService service = CreateService();
            var start = await service.StartEnvironmentInstance("sampleapp");
            await service.StopEnvironmentInstance(start);
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

            var kubernetesClientMock = new Mock<IKubernetesClient>();
            var instanceListTask = Task.FromResult<IList<EnvironmentInstance>>(new List<EnvironmentInstance>()
                { 
                    new EnvironmentInstance() { Name = "test", Status = "Running" }, 
                    new EnvironmentInstance() { Name = "other", Status = "ContainerCreating"}, 
                }
            );
            kubernetesClientMock.Setup(kc => kc.GetEnvironments()).Returns(instanceListTask);

            var routingServiceMock = new Mock<IRoutingService>();

            var mockLogger = new Mock<ILogger<EnvironmentService>>().Object;

            IEnvironmentService service = new EnvironmentService(optionsMock.Object, 
                kubernetesClientMock.Object, 
                routingServiceMock.Object,
                mockLogger);

            return service;
        }
    }
}