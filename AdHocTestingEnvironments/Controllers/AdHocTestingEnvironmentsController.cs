using AdHocTestingEnvironments.Model.Environment;
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

        private readonly IEnvironmentService _environmentService;

        public AdHocTestingEnvironmentsController(IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
        }

        // GET: api/<AdHocEnvironmentController>
        [HttpGet]
        public async Task<IEnumerable<AdHocTestingEnvironmentInstanceViewModel>> Get()
        {
            return await _environmentService.ListEnvironmentInstances();
        }

        [HttpGet]
        public async Task<IEnumerable<TestingEnvironemntViewModel>> ListEnvironments()
        {
            return await _environmentService.ListEnvironmetns();
        }


        // POST api/<AdHocEnvironmentController>
        [HttpPost]
        public async Task<string> Post([FromBody] string value)
        {
            return await _environmentService.StartEnvironmentInstance(value);
        }


        // DELETE api/<AdHocEnvironmentController>/5
        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _environmentService.StopEnvironmentInstance(id);
        }
    }
}
