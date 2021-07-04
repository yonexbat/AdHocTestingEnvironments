using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace AdHocTestingEnvironments.Services
{
    public interface IRequestRouterService
    {
        Task RouteRequest(HttpContext httpContext, IHttpForwarder forwarder);
    }
}
