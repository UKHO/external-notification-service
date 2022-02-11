using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ExternalNotificationService.API.Extensions;
using UKHO.ExternalNotificationService.API.Services;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;
using UKHO.ExternalNotificationService.Common.Helper;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.Request;
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
        private List<Error> _errors;

        public SubscriptionController(IHttpContextAccessor contextAccessor, ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService, IAzureMessageQueueHelper azureMessageQueueHelper, IOptions<SubscriptionStorageConfiguration> ensStorageConfiguration, ISubscriptionStorageService subscriptionStorageService) : base(contextAccessor, logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
            _azureMessageQueueHelper = azureMessageQueueHelper;
            _ensStorageConfiguration = ensStorageConfiguration;
            _subscriptionStorageService = subscriptionStorageService;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] D365Payload d365Payload)
        {
            _logger.LogInformation(EventIds.ENSSubscriptionRequestStart.ToEventId(), "Subscription request for D365Payload:{d365Payload} with _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(d365Payload), GetCurrentCorrelationId());

            if (d365Payload == null)
            {
                var error = new List<Error>
                {
                    new Error()
                    {
                        Source = "requestBody",
                        Description = "Either body is null or malformed."
                    }
                };
                return BuildBadRequestErrorResponse(error);
            }

            ValidationResult validationD365PayloadResult = await _subscriptionService.ValidateD365PayloadRequest(d365Payload);

            if (!validationD365PayloadResult.IsValid && validationD365PayloadResult.HasBadRequestErrors(out _errors))
            {
                return BuildBadRequestErrorResponse(_errors);
            }

            SubscriptionRequest subscription = _subscriptionService.ConvertToSubscriptionRequestModel(d365Payload);
            var subscriptionMessage = _subscriptionService.GetSubscriptionRequestMessage(subscription);

            _logger.LogInformation(EventIds.LogRequest.ToEventId(), "Subscription request Accepted for D365Payload:{JsonConvert.SerializeObject(objPayload)} with _X-Correlation-ID:{correlationId}", JsonConvert.SerializeObject(d365Payload), GetCurrentCorrelationId());
            string storageAccountConnectionString = _subscriptionStorageService.GetStorageAccountConnectionString(_ensStorageConfiguration.Value.StorageAccountName, _ensStorageConfiguration.Value.StorageAccountKey);
            await _azureMessageQueueHelper.AddQueueMessage(storageAccountConnectionString, _ensStorageConfiguration.Value.QueueName, subscriptionMessage, GetCurrentCorrelationId());
            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted for SubscriptionId:{subscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", subscription.SubscriptionId, subscription.D365CorrelationId, GetCurrentCorrelationId());
            _logger.LogInformation(EventIds.Accepted.ToEventId(), "Subscription request Accepted for SubscriptionId:{subscriptionId} with _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{correlationId}", subscription.SubscriptionId, subscription.D365CorrelationId, GetCurrentCorrelationId());

            return GetEnsResponse(new ExternalNotificationServiceResponse { HttpStatusCode = HttpStatusCode.Accepted });
        }

    }
}
