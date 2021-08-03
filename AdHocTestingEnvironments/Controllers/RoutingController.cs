using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdHocTestingEnvironments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutingController : ControllerBase
    {

        private readonly IEndpointResolverService _routingService;

        public RoutingController(IEndpointResolverService routingService)
        {
            _routingService = routingService;
        }


        [HttpGet]
        public async Task<IEnumerable<EndpointEntry>> Get()
        {
            return await _routingService.GetCustomItem();
        }


        [HttpGet("{id}")]
        public async Task<EndpointEntry> Get(string id)
        {
            return await _routingService.GetItem(id);
        }

        [HttpPost]
        public async Task<EndpointEntry> Post([FromBody] EndpointEntry value)
        {
            return await _routingService.AddCustomItem(value);
        }

        [HttpPut("{id}")]
        public async Task<EndpointEntry> Put(string id, [FromBody] EndpointEntry routingEntry)
        {
            var item = await _routingService.GetItem(id);
            item.Destination = routingEntry.Destination;
            item.Name = routingEntry.Name;
            return item;
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _routingService.DeleteCustomItem(id);
        }
    }
}
