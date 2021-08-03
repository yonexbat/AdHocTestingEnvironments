using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Services.Implementations;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Yarp.ReverseProxy.Forwarder;

namespace AdHocTestingEnvironmentsTests
{
    public class RequestRouterServiceTest
    {
        [Fact]
        public async Task RouteOk1()
        {
            (IRequestRouterService requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/endpoint/myapp", "myapp", "http://www.ciri.com");
            await requestRouter.RouteRequest(context, httpForwarder);
        }

        [Fact]
        public async Task RouteOk2()
        {
            (IRequestRouterService requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/endpoint/myapp/", "myapp", "http://www.ciri.com");
            await requestRouter.RouteRequest(context, httpForwarder);

        }

        [Fact]
        public async Task RouteOk3()
        {
            (IRequestRouterService requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/endpoint/myapp/helloThere?ewrew", "myapp", "http://www.ciri.com");
            await requestRouter.RouteRequest(context, httpForwarder);
        }

        [Fact]
        public async Task RouteNok1()
        {
            (IRequestRouterService requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/notexisting", "myapp", "http://www.ciri.com");
            await Assert.ThrowsAsync<ArgumentException>(() => requestRouter.RouteRequest(context, httpForwarder));

        }

        private (IRequestRouterService requestRouter, HttpContext context, IHttpForwarder httpForwarder) SetUp(string requestPath, string appId, string destination)
        {
            var routingServiceMock = new Mock<IEndpointResolverService>();

            EndpointEntry epe = new EndpointEntry()
            {
                Name = appId,
                Destination = destination,
            };

            routingServiceMock.Setup(f => f.GetItem(It.IsAny<string>())).Returns(Task.FromResult(epe));

            var httpForwarderMock = new Mock<IHttpForwarder>();


            var contextMock = new DefaultHttpContext();
            contextMock.Request.Path = requestPath;
            var mockLogger = new Mock<ILogger<RequestRouterService>>().Object;


            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(IEndpointResolverService)))
                .Returns(routingServiceMock.Object);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);


            RequestRouterService requestRouter = new RequestRouterService(serviceProviderMock.Object, mockLogger);
            return (requestRouter, contextMock, httpForwarderMock.Object);
        }
    }
}
