using System;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        public PlatformController()
        {

        }

        [HttpPost]
        public ActionResult TestInboundConnection(){
            Console.WriteLine("--> inbound POST # Command Service");

            return Ok("Inbound test of from Platforms Controller");
        }
    }
}