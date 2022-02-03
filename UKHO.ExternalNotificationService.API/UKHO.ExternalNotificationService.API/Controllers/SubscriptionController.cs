using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using UKHO.ExternalNotificationService.API.Helper;
using UKHO.ExternalNotificationService.API.Models;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAzureMessageQueueHelper _azureMessageQueueHelper;
        private readonly IOptions<EnsSubscriptionStorageConfiguration> _ensStorageConfiguration;

        public SubscriptionController(IHttpContextAccessor contextAccessor, ISubscriptionService subscriptionService, IAzureMessageQueueHelper azureMessageQueueHelper, IOptions<EnsSubscriptionStorageConfiguration> ensStorageConfiguration, ILogger<SubscriptionController> logger) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _azureMessageQueueHelper = azureMessageQueueHelper;
            _ensStorageConfiguration = ensStorageConfiguration;
        }

        [HttpPost]
        public async virtual Task<IActionResult> Post([FromBody] JObject jobj)
        {
            SubscriptionRequest subscription = ConvertToSubscriptionRequestModel(jobj);
            var subscriptionMessage = _subscriptionService.GetSubscriptionRequestMessage(subscription);
            string storageAccountConnectionString =
                 _subscriptionService.GetStorageAccountConnectionString(_ensStorageConfiguration.Value.StorageAccountName, _ensStorageConfiguration.Value.StorageAccountKey);
            await _azureMessageQueueHelper.AddQueueMessage(storageAccountConnectionString, _ensStorageConfiguration.Value.QueueName, subscriptionMessage);

            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "Subscription request Accepted", jobj);
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.OK });
        }
        private SubscriptionRequest ConvertToSubscriptionRequestModel(JObject jobj)
        {
            return new SubscriptionRequest()
            {
                Id = "12",
                IsActive = true,
                WebhookUrl = Convert.ToString(""),
                NotificationType = Convert.ToString(""),
                CorrelationId = ""
            };
        }      
    }
}
