using Azure.Storage.Queues.Models;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        private readonly IHandleDeadLetterService _handleDeadLetterService;

        public SubscriptionServiceJob(ISubscriptionServiceData subscriptionServiceData,
            ILogger<SubscriptionServiceJob> logger, IOptions<D365CallbackConfiguration> d365CallbackConfiguration,
            ICallbackService callbackService,
            IHandleDeadLetterService handleDeadLetterService)
        {
            _subscriptionServiceData = subscriptionServiceData;           
            _logger = logger;
            _d365CallbackConfiguration = d365CallbackConfiguration;           
            _callbackService =  callbackService;
            _handleDeadLetterService = handleDeadLetterService;
        }

        public async Task ProcessQueueMessage([QueueTrigger("%SubscriptionStorageConfiguration:QueueName%")] QueueMessage message)
        {            
            SubscriptionRequestMessage subscriptionMessage = message.Body.ToObjectFromJson<SubscriptionRequestMessage>();
            EventSubscription eventSubscription;
            _logger.LogInformation(EventIds.CreateSubscriptionRequestStart.ToEventId(),
                    "Subscription provisioning request started for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);

            SubscriptionRequestResult subscriptionRequestResult = new(subscriptionMessage);
            ExternalNotificationEntity externalNotificationEntity;

            await Agent.Tracer.CaptureTransaction("Subscription-Transaction", ApiConstants.TypeRequest, async () =>
            {
                var transaction = Agent.Tracer.CurrentTransaction;

                try
                {
                    if (subscriptionMessage.IsActive)
                    {

                        ISpan span = transaction.StartSpan("CreateSubscription", ApiConstants.TypeApp,
                            ApiConstants.SubTypeInternal);

                        bool created = false;
                        try
                        {
                            eventSubscription =
                                await _subscriptionServiceData.CreateOrUpdateSubscription(subscriptionMessage,
                                    CancellationToken.None);
                            subscriptionRequestResult.ProvisioningState = "Succeeded";

                            externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(
                                subscriptionRequestResult, subscriptionMessage.IsActive,
                                _d365CallbackConfiguration.Value.SucceededStatusCode);

                            _logger.LogInformation(EventIds.CreateSubscriptionRequestSuccess.ToEventId(),
                                "Subscription provisioning request Succeeded for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                                subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId,
                                subscriptionMessage.CorrelationId);

                            created = true;

                        }
                        catch (Exception e)
                        {
                            subscriptionRequestResult.ProvisioningState = "Failed";

                            //Webhook validation handshake failure error
                            if (e.Message.Contains("Webhook validation handshake failed"))
                            {
                                int startIndex = e.Message.IndexOf("Webhook validation handshake failed");
                                subscriptionRequestResult.ErrorMessage =
                                    e.Message.Substring(startIndex, e.Message.Length - startIndex);

                                externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(
                                    subscriptionRequestResult, subscriptionMessage.IsActive,
                                    _d365CallbackConfiguration.Value.FailedStatusCode);

                                _logger.LogError(EventIds.CreateSubscriptionRequestHandshakeFailureError.ToEventId(),
                                    "Subscription provisioning request failed with Webhook handshake failure error with Exception:{e} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                                    e.Message, subscriptionRequestResult.SubscriptionId,
                                    subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                            }
                            //other potential errors
                            else
                            {
                                subscriptionRequestResult.ErrorMessage = e.Message;
                                externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(
                                    subscriptionRequestResult, subscriptionMessage.IsActive,
                                    _d365CallbackConfiguration.Value.FailedStatusCode);

                                _logger.LogError(EventIds.CreateSubscriptionRequestOtherError.ToEventId(),
                                    "Subscription provisioning request failed with other error with Exception:{e} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                                    e.Message, subscriptionRequestResult.SubscriptionId,
                                    subscriptionMessage.D365CorrelationId, subscriptionMessage.CorrelationId);
                            }

                            span?.CaptureException(e);

                        }
                        finally
                        {
                            transaction?.SetLabel("SubscriptionCreated", created);
                            span?.End();
                        }
                    }
                    //delete the subscription if status is Inactive
                    else
                    {
                        ISpan span = transaction.StartSpan("DeleteSubscription", ApiConstants.TypeApp,
                            ApiConstants.SubTypeInternal);

                        bool deleted = false;

                        try
                        {
                            await _subscriptionServiceData.DeleteSubscription(subscriptionMessage,
                                CancellationToken.None);
                            subscriptionRequestResult.ProvisioningState = "Succeeded";

                            externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(
                                subscriptionRequestResult, subscriptionMessage.IsActive,
                                _d365CallbackConfiguration.Value.SucceededStatusCode);

                            _logger.LogInformation(EventIds.DeleteSubscriptionRequestSuccess.ToEventId(),
                                "Delete Event Grid Domain Subscription request Succeeded for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                                subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId,
                                subscriptionMessage.CorrelationId);

                            deleted = true;
                        }
                        catch (Exception ex)
                        {
                            subscriptionRequestResult.ProvisioningState = "Failed";
                            subscriptionRequestResult.ErrorMessage = ex.Message;

                            externalNotificationEntity = CommonHelper.GetExternalNotificationEntity(
                                subscriptionRequestResult, subscriptionMessage.IsActive,
                                _d365CallbackConfiguration.Value.FailedStatusCode);

                            _logger.LogError(EventIds.DeleteSubscriptionRequestError.ToEventId(),
                                "Delete Event Grid Domain Subscription request failed with error with Exception:{ex} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                                ex.Message, subscriptionRequestResult.SubscriptionId,
                                subscriptionMessage.D365CorrelationId,
                                subscriptionMessage.CorrelationId);

                            span?.CaptureException(ex);
                        }
                        finally
                        {
                            transaction?.SetLabel("SubscriptionDeleted", deleted);
                            span?.End();
                        }
                    }

                    //Callback to D365
                    _logger.LogInformation(EventIds.CallbackToD365Started.ToEventId(),
                        "Callback to D365 using Dataverse start with ResponseStatusCode:{ResponseStatusCode} and ResponseDetails:{externalNotificationEntity} for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                        externalNotificationEntity.ResponseStatusCode, externalNotificationEntity.ResponseDetails,
                        subscriptionRequestResult.SubscriptionId, subscriptionMessage.D365CorrelationId,
                        subscriptionMessage.CorrelationId);

                    string entityPath = $"ukho_externalnotifications({subscriptionMessage.SubscriptionId})";
                    await _callbackService.CallbackToD365UsingDataverse(entityPath, externalNotificationEntity,
                        subscriptionMessage);

                    _logger.LogInformation(EventIds.CreateSubscriptionRequestCompleted.ToEventId(),
                        "Subscription provisioning request Completed for SubscriptionId:{SubscriptionId} and _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}",
                        subscriptionMessage.SubscriptionId, subscriptionMessage.D365CorrelationId,
                        subscriptionMessage.CorrelationId);
                }
                catch (Exception e)
                {
                    transaction?.CaptureException(e);
                    throw;
                }
                finally
                {
                    transaction?.End();
                }

               

            });
        }

        public async Task ProcessDeadLetterMessage([BlobTrigger("%SubscriptionStorageConfiguration:StorageContainerName%/{filePath}")] Stream myBlob, string filePath)
        {
            string subscriptionId = GetSubscriptionId(filePath);
            string fileName = Path.GetFileName(filePath);

            SubscriptionRequestMessage subscriptionRequestMessage = new() { CorrelationId = Guid.NewGuid().ToString(), SubscriptionId = subscriptionId };

            _logger.LogInformation(EventIds.ENSDeadLetterContainerJobRequestStart.ToEventId(),
                    "External notification service - dead letter container webjob request started for SubscriptionId:{SubscriptionId}, FileName:{fileName}, _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionId, fileName, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);

            await _handleDeadLetterService.ProcessDeadLetter(filePath, subscriptionId, subscriptionRequestMessage, fileName);

            _logger.LogInformation(EventIds.ENSDeadLetterContainerJobRequestCompleted.ToEventId(),
                    "External notification service - dead letter container webjob request completed for SubscriptionId:{SubscriptionId}, FileName:{fileName}, _D365-Correlation-ID:{correlationId} and _X-Correlation-ID:{CorrelationId}", subscriptionId, fileName, subscriptionRequestMessage.D365CorrelationId, subscriptionRequestMessage.CorrelationId);
        }

        private static string GetSubscriptionId(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string[] splitFilePath = filePath.Split("/");
                if (splitFilePath.Length > 1)
                {
                    return splitFilePath[1];
                }
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
