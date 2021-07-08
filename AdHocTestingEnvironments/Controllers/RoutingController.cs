using AdHocTestingEnvironments.Model;
using AdHocTestingEnvironments.Services;
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

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<EndpointEntry> Get()
        {
            return _routingService.GetCustomItem();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public EndpointEntry Get(string id)
        {
            return _routingService.GetItem(id);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public EndpointEntry Post([FromBody] EndpointEntry value)
        {
            return _routingService.AddCustomItem(value);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public EndpointEntry Put(string id, [FromBody] EndpointEntry routingEntry)
        {
            var item = _routingService.GetItem(id);
            item.Destination = routingEntry.Destination;
            item.Name = routingEntry.Name;
            return item;
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _routingService.DeleteCustomItem(id);
        }
    }
}
