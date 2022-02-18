
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistributorController : BaseController<DistributorController>
    {
        [HttpOptions]
        [Route("/webhook/newEventspublished")]
        public IActionResult Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }
            return GetWebhookResponse();
        }

        [HttpPost]
        [Route("/webhook/newEventspublished")]
        public virtual async Task<IActionResult> Post()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();
                return GetWebhookResponse();
            }
        }
    }
}
