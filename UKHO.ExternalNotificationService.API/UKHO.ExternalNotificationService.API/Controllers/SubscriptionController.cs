using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using UKHO.ExternalNotificationService.API.Services;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helper;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Response;
using UKHO.ExternalNotificationService.Common.Storage;

namespace UKHO.ExternalNotificationService.API.Controllers
{
    [ApiController]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ILogger<SubscriptionController> _logger;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IAzureMessageQueueHelper _azureMessageQueueHelper;
        private readonly IOptions<SubscriptionStorageConfiguration> _ensStorageConfiguration;
        private readonly ISubscriptionStorageService _subscriptionStorageService;

        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService, IAzureMessageQueueHelper azureMessageQueueHelper, IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration, ISubscriptionStorageService subscriptionStorageService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _azureMessageQueueHelper = azureMessageQueueHelper;
            _ensStorageConfiguration = ensStorageConfiguration;
            _subscriptionStorageService = subscriptionStorageService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] D365Payload objPayload)
        {
            SubscriptionRequest subscription = ConvertToSubscriptionRequestModel(objPayload);
            var subscriptionMessage = _subscriptionService.GetSubscriptionRequestMessage(subscription);

            string storageAccountConnectionString = _subscriptionStorageService.GetStorageAccountConnectionString(_ensStorageConfiguration.Value.StorageAccountName, _ensStorageConfiguration.Value.StorageAccountKey);
            await _azureMessageQueueHelper.AddQueueMessage(storageAccountConnectionString, _ensStorageConfiguration.Value.QueueName, subscriptionMessage, GetCurrentCorrelationId());
            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted for D365Payload:{JsonConvert.SerializeObject(objPayload)} with _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(objPayload), GetCurrentCorrelationId());
            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }

        private static SubscriptionRequest ConvertToSubscriptionRequestModel(D365Payload objPayload)
        {
            return new SubscriptionRequest()
            {
                Id = "12",
                IsActive = true,
                WebhookUrl = Convert.ToString(""),
                NotificationType = Convert.ToString(""),
                CorrelationId = objPayload.CorrelationId
            };
        }
    }
}
