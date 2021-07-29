using AdHocTestingEnvironments.Model.Common;
using AdHocTestingEnvironments.Model.Environment;
using AdHocTestingEnvironments.Model.Kubernetes;
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
    public class AdHocTestingEnvironmentsController : ControllerBase
    {

        private readonly IEnvironmentInstanceService _environmentInstanceService;
        private readonly IEnvironmentService _environmentService;

        public AdHocTestingEnvironmentsController(
            IEnvironmentInstanceService environmentInstanceService,
            IEnvironmentService environmentService)
        {
            _environmentInstanceService = environmentInstanceService;
            _environmentService = environmentService;
        }

        // GET: api/<AdHocEnvironmentController>
        [HttpGet]
        public async Task<IEnumerable<EnvironmentInstance>> Get()
        {
            return await _environmentInstanceService.ListEnvironmentInstances();
        }

        [HttpGet(nameof(ListEnvironments))]
        public async Task<IEnumerable<Application>> ListEnvironments()
        {
            return await _environmentService.ListEnvironmetns();
        }


        // POST api/<AdHocEnvironmentController>
        [HttpPost]
        public async Task<StartResult> Post([FromBody] StartRequest dto)
        {
            var instanceName = await _environmentInstanceService.StartEnvironmentInstance(dto);
            return new StartResult() { Status = "Ok", InstanceName = instanceName, };
        }


        // DELETE api/<AdHocEnvironmentController>/5
        [HttpDelete("{id}")]
        public async Task<Result> Delete(string id)
        {
            await _environmentInstanceService.StopEnvironmentInstance(id);
            return new Result() { Status = "Ok", };
        }
    }
}
