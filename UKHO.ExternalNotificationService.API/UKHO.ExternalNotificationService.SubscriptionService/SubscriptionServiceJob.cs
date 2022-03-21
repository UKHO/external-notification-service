﻿using Azure.Storage.Queues.Models;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Logging;
using UKHO.ExternalNotificationService.Common.Models.AzureEventGridDomain;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.SubscriptionService.Configuration;
using UKHO.ExternalNotificationService.SubscriptionService.D365Callback;
using UKHO.ExternalNotificationService.SubscriptionService.Helpers;
using UKHO.ExternalNotificationService.SubscriptionService.Services;

namespace UKHO.ExternalNotificationService.SubscriptionService
{
    [ExcludeFromCodeCoverage]
    public class SubscriptionServiceJob
    {
        private readonly ISubscriptionServiceData _subscriptionServiceData;
        private readonly ILogger<SubscriptionServiceJob> _logger;        
        private readonly IOptions<D365CallbackConfiguration> _d365CallbackConfiguration;
        private readonly ICallbackService _callbackService;        

        public SubscriptionServiceJob(ISubscriptionServiceData subscriptionServiceData,
            ILogger<SubscriptionServiceJob> logger, IOptions<D365CallbackConfiguration> d365CallbackConfiguration,
            ICallbackService callbackService)
        {
            _subscriptionServiceData = subscriptionServiceData;           
            _logger = logger;
            _d365CallbackConfiguration = d365CallbackConfiguration;           
            _callbackService =  callbackService;
        }

        public async Task ProcessQueueMessage([QueueTrigger("%SubscriptionStorageConfiguration:QueueName%")] QueueMessage message)
        {            
            SubscriptionRequestMessage subscriptionMessage = message.Body.ToObjectFromJson<SubscriptionRequestMessage>();
            EventSubscription eventSubscription;
            _logger.LogInformation(EventIds.CreateSubscriptionRequestStart.ToEventId(),
                    "Subscription provisioning request started for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

            SubscriptionRequestResult subscriptionRequestResult = new(subscriptionMessage);
            ExternalNotificationEntity externalNotificationEntity;
            if (subscriptionMessage.IsActive)
            {
                try
                {
                    eventSubscription = await _subscriptionServiceData.CreateOrUpdateSubscription(subscriptionMessage, CancellationToken.None);
                    subscriptionRequestResult.ProvisioningState = "Succeeded";

                    externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, subscriptionMessage.IsActive, _d365CallbackConfiguration.Value.SucceededStatusCode);

                    _logger.LogError(EventIds.CreateSubscriptionRequestSuccess.ToEventId(),
                 "Subscription provisioning request Succeeded for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                }
                catch (Exception e)
                {
                    subscriptionRequestResult.ProvisioningState = "Failed";

                    //Webhook validation handshake failure error
                    if (e.Message.Contains("Webhook validation handshake failed"))
                    {
                        int startIndex = e.Message.IndexOf("Webhook validation handshake failed");
                        subscriptionRequestResult.ErrorMessage = e.Message.Substring(startIndex, e.Message.Length - startIndex);

                        externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, subscriptionMessage.IsActive, _d365CallbackConfiguration.Value.FailedStatusCode);

                        _logger.LogError(EventIds.CreateSubscriptionRequestHandshakeFailureError.ToEventId(),
                  "Subscription provisioning request failed with Webhook handshake failure error with Exception:{e} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", e.Message, subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                    }
                    //other potential errors
                    else
                    {
                        subscriptionRequestResult.ErrorMessage = e.Message;
                        externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, subscriptionMessage.IsActive, _d365CallbackConfiguration.Value.FailedStatusCode);

                        _logger.LogError(EventIds.CreateSubscriptionRequestOtherError.ToEventId(),
                  "Subscription provisioning request failed with other error with Exception:{e} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", e.Message, subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                    }
                }
            }
            //delete the subscription if status is Inactive
            else
            {
                try
                {
                    await _subscriptionServiceData.DeleteSubscription(subscriptionMessage, CancellationToken.None);
                    subscriptionRequestResult.ProvisioningState = "Succeeded";

                    externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, subscriptionMessage.IsActive, _d365CallbackConfiguration.Value.SucceededStatusCode);

                    _logger.LogError(EventIds.DeleteSubscriptionRequestSuccess.ToEventId(),
                 "Delete Event Grid Subscription provisioning request Succeeded for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                }
                catch (Exception ex)
                {
                    subscriptionRequestResult.ProvisioningState = "Failed";
                    subscriptionRequestResult.ErrorMessage = ex.Message;

                    externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(subscriptionRequestResult, subscriptionMessage.IsActive, _d365CallbackConfiguration.Value.FailedStatusCode);

                    _logger.LogError(EventIds.DeleteSubscriptionRequestError.ToEventId(),
                  "Delete Event Grid Subscription provisioning request failed with error with Exception:{ex} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", ex.Message, subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                }
            }
            //Callback to D365
            _logger.LogInformation(EventIds.CallbackToD365Started.ToEventId(),
              "Callback to D365 using Dataverse start with ResponseStatusCode:{ResponseStatusCode} and ResponseDetails:{externalNotificationEntity} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", externalNotificationEntity.ResponseStatusCode, externalNotificationEntity.ResponseDetails, subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

                string entityPath = $"ukho_externalnotifications({subscriptionMessage.SubscriptionId})";
                await _callbackService.CallbackToD365UsingDataverse(entityPath, externalNotificationEntity, subscriptionMessage);            
            
            _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
                    "Subscription provisioning request Completed for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
        }
    }
}
