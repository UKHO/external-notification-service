using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UKHO.D365CallbackDistributorStub.API.Models.Request;
using UKHO.D365CallbackDistributorStub.API.Services;

namespace UKHO.D365CallbackDistributorStub.API.Controllers
{
    [Route("/webhook/notification")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class DistributorController : BaseController<DistributorController>
    {
        private readonly ILogger<DistributorController> _logger;
        private readonly IDistributionWebhookService _distributionWebhookService;
        private readonly DistributionService _distributionService;

        public DistributorController(ILogger<DistributorController> logger, IDistributionWebhookService distributionWebhookService,
            DistributionService distributionService)
        {
            _logger = logger;
            _distributionWebhookService = distributionWebhookService;
            _distributionService = distributionService;
        }


        [HttpOptions]
        public IActionResult Options()
        {
            _logger.LogInformation("Distributor option accessed.");
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }
            return GetWebhookResponse();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            _logger.LogInformation("Distributor Webhook accessed");
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();
                CustomCloudEvent? customCloudEvent = JsonConvert.DeserializeObject<CustomCloudEvent>(jsonContent);
                bool isDistributorRequestSaved = DistributionService.SaveDistributorRequest(customCloudEvent);
                if (isDistributorRequestSaved)
                {
                    return NoContent();
                }
                else
                {
                    //return BuildInternalServerErrorResponse();
                }
            }
            return NoContent();
        }

        [HttpGet]
        public IActionResult Get(string cloudEventId)
        {
            DistributorRequest? distributorRequest = _distributionService.GetDistributorRequest(cloudEventId);
            if (distributorRequest == null)
            {
                _logger.LogInformation("Distribution Request not found for CloudEventId : {cloudEventId}", cloudEventId);
                return NotFound();
            }
            return Ok(distributorRequest);
        }
    }
}
