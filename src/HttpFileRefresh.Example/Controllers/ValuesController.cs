using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HttpFileRefresh.Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration Config {get; set;}

        public ValuesController(IConfiguration config) 
        {
            Config = config;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Config["ConnectionStrings:DPD1476A"];
        }
    }
}
