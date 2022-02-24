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
        private readonly DistributionService _distributionService;
        public DistributorController(ILogger<DistributorController> logger, DistributionService distributionService)
        {
            _logger = logger;
            _distributionService = distributionService;
        }

        [HttpOptions]
        public IActionResult Options()
        {
            _logger.LogInformation("Distributor option accessed.");
            using (StreamReader? reader = new(Request.Body, Encoding.UTF8))
            {
                string? webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }
            return GetOkResponse();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            _logger.LogInformation("Distributor Webhook accessed");
            using (StreamReader? reader = new(Request.Body, Encoding.UTF8))
            {
                string jsonContent = await reader.ReadToEndAsync();
                CustomCloudEvent? customCloudEvent = JsonConvert.DeserializeObject<CustomCloudEvent>(jsonContent);
                if (customCloudEvent != null)
                {
                    bool isDistributorRequestSaved = DistributionService.SaveDistributorRequest(customCloudEvent);
                    if (isDistributorRequestSaved)
                    {
                        _logger.LogInformation("Distributor webhook request stored in memory for Id: {Id}", customCloudEvent.Id);
                        return GetNotContentResponse();
                    }
                    else
                    {
                        _logger.LogInformation("Distributor webhook request not stored in memory for Id: {Id}", customCloudEvent.Id);
                        return GetBadRequestResponse();
                    }
                }
                else
                {
                    _logger.LogInformation("Distributor webhook request cannot be null");
                    return GetBadRequestResponse();
                }
            }
        }

        [HttpGet]
        public IActionResult Get(string cloudEventId)
        {
            _logger.LogInformation("GET distribution request accessed for Id: {Id}", cloudEventId);
            DistributorRequest? distributorRequest = _distributionService.GetDistributorRequest(cloudEventId);
            if (distributorRequest == null)
            {
                _logger.LogInformation("Distribution Request not found for CloudEventId : {cloudEventId}", cloudEventId);
                return GetNotFoundResponse();
            }
            _logger.LogInformation("Distribution Request found and return for CloudEventId : {cloudEventId}", cloudEventId);
            return Ok(distributorRequest);
        }
    }
}
