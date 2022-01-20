using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        public SubscriptionController()
        {

        }

        [HttpPost]
        [Route("/api/subscription")]
        public async Task<IActionResult> Post([FromBody] JObject jobj)
        {
            return Ok();
        }
    }
}
