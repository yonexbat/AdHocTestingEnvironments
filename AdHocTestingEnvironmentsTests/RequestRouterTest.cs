using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Routing;
using AdHocTestingEnvironments.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Yarp.ReverseProxy.Forwarder;

namespace AdHocTestingEnvironmentsTests
{
    public class RequestRouterTest
    {
        [Fact]
        public async Task RouteOk1()
        {
            (RequestRouter requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/endpoint/myapp", "myapp", "http://www.ciri.com");
            await requestRouter.RouteRequest(context, httpForwarder);

        }

        [Fact]
        public async Task RouteOk2()
        {
            (RequestRouter requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/endpoint/myapp/", "myapp", "http://www.ciri.com");
            await requestRouter.RouteRequest(context, httpForwarder);

        }

        [Fact]
        public async Task RouteOk3()
        {
            (RequestRouter requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/endpoint/myapp/helloThere?ewrew", "myapp", "http://www.ciri.com");
            await requestRouter.RouteRequest(context, httpForwarder);
        }

        [Fact]
        public async Task RouteNok1()
        {
            (RequestRouter requestRouter, HttpContext context, IHttpForwarder httpForwarder) = SetUp("/notexisting", "myapp", "http://www.ciri.com");
            await Assert.ThrowsAsync<ArgumentException>(() => requestRouter.RouteRequest(context, httpForwarder));

        }

        private (RequestRouter requestRouter, HttpContext context, IHttpForwarder httpForwarder) SetUp(string requestPath, string appId, string destination)
        {
            var routingServiceMock = new Mock<IRoutingService>();
            routingServiceMock.Setup(f => f.GetItem(It.IsAny<string>())).Returns(new RoutingEntry()
            {
                Name = appId,
                Destination = destination,
            });

            var httpForwarderMock = new Mock<IHttpForwarder>();


            var contextMock = new DefaultHttpContext();
            contextMock.Request.Path = requestPath;


            RequestRouter requestRouter = new RequestRouter(routingServiceMock.Object);
            return (requestRouter, contextMock, httpForwarderMock.Object);
        }
    }
}
