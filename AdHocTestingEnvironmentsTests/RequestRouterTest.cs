using AdHocTestingEnvironments.Routing;
using System;
using Xunit;

namespace AdHocTestingEnvironmentsTests
{
    public class RequestRouterTest
    {
        [Fact]
        public void Test1()
        {
            RequestRouter requestRouter = new RequestRouter(null);

            var res = requestRouter.GetDestination("/endpoint/abc/");

        }
    }
}
