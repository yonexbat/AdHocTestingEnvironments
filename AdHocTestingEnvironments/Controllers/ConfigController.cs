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
    public class ConfigController : ControllerBase
    {

        IGitClientService _gitClientService;

        public ConfigController(IGitClientService gitClientService)
        {
            _gitClientService = gitClientService;
        }


        /// <summary>
        /// Intentional abuse of get.
        /// Hack.
        /// https://localhost:5001/api/Config?a=abc&b=def
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get([FromQuery]string a, [FromQuery] string b)
        {
            _gitClientService.ChangeUserNameAndPassword(a, b);
            return "Ok";
        }
       
    }
}
