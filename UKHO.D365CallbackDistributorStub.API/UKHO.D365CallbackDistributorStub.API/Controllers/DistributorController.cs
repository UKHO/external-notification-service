using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;

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
            string? webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
            HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
            HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            return OkResponse();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post()
        {
            _logger.LogInformation("Distributor Webhook accessed");

            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            {
                string jsonContent = await reader.ReadToEndAsync();
                CustomCloudEvent? customCloudEvent = JsonSerializer.Deserialize<CustomCloudEvent>(jsonContent,JOptions);

                if (customCloudEvent != null)
                {
                    CommandDistributionRequest? commandDistributionRequest = _distributionService.SubjectInCommandDistributionList(customCloudEvent.Subject);
                    bool distributionRequestSaved = DistributionService.SaveDistributorRequest(customCloudEvent,
                                                                              commandDistributionRequest == null ? HttpStatusCode.OK : commandDistributionRequest.HttpStatusCode);

                    if (distributionRequestSaved)
                    {
                        _logger.LogInformation("Distributor webhook request stored in memory for Subject: {Subject}", customCloudEvent.Subject);
                        if (commandDistributionRequest != null)
                        {
                            _logger.LogInformation("Distribution request failed for Subject: {subjectId} with httpStatusCode as {httpStatusCode}", customCloudEvent.Subject, commandDistributionRequest.HttpStatusCode.ToString());
                            return GetEnsStubResponse(commandDistributionRequest.HttpStatusCode);
                        }
                        return OkResponse();
                    }
                    else
                    {
                        _logger.LogInformation("Distributor webhook request not stored in memory for Subject: {Subject}", customCloudEvent.Subject);
                        return BadRequestResponse();
                    }
                }
                else
                {
                    _logger.LogInformation("Distributor webhook request cannot be null");
                    return BadRequestResponse();
                }
            }
        }

        [HttpGet]
        public IActionResult Get(string? subject)
        {
            _logger.LogInformation("GET distribution request accessed for Subject: {Subject}", subject);
            List<DistributorRequest>? distributorRequest = _distributionService.GetDistributorRequest(subject);

            if (distributorRequest == null || distributorRequest.Count == 0)
            {
                _logger.LogInformation("Distribution Request not found for Subject : {Subject}", subject);
                return NotFoundResponse();
            }
            _logger.LogInformation("Distribution Request found and return for Subject : {Subject}", subject);
            return Ok(distributorRequest);
        }

        [HttpPost("command-to-return-status/{subject}/{httpStatusCode?}")]
        public IActionResult CommandToReturnStatus(string subject, HttpStatusCode? httpStatusCode)
        {
            _logger.LogInformation("Command for distributor webhook accessed for subject: {subject}", subject);

            bool commandDistributorRequestSaved = _distributionService.SaveCommandDistributorRequest(subject, httpStatusCode);
            if (commandDistributorRequestSaved)
            {
                _logger.LogInformation("Command for distributor webhook request stored in memory for Subject: {Subject}", subject);
                return OkResponse();
            }
            _logger.LogInformation("Command distributor webhook request not stored in memory for Subject: {Subject}", subject);
            return BadRequestResponse();
        }
    }
}
