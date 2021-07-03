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
    public class AdHocTestingEnvironmentsController : ControllerBase
    {

        private readonly IKubernetesClient _kubernetesClient;       

        public AdHocTestingEnvironmentsController(IKubernetesClient kubernetesClient)
        {
            _kubernetesClient = kubernetesClient;
        }

        // GET: api/<AdHocTestingEnvironmentsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AdHocTestingEnvironmentsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AdHocTestingEnvironmentsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AdHocTestingEnvironmentsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AdHocTestingEnvironmentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
