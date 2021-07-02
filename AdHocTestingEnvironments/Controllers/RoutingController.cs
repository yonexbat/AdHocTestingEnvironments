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

        private readonly IRoutingService _routingService;

        public RoutingController(IRoutingService routingService)
        {
            _routingService = routingService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<RoutingEntry> Get()
        {
            return _routingService.GetItems();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public RoutingEntry Get(string id)
        {
            return _routingService.GetItem(id);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public RoutingEntry Post([FromBody] RoutingEntry value)
        {
            return _routingService.AddItem(value);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public RoutingEntry Put(string id, [FromBody] RoutingEntry routingEntry)
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
            _routingService.DeleteItem(id);
        }
    }
}
