using Microsoft.Extensions.Logging;

namespace UKHO.ExternalNotificationService.Common.Logging
{
    public enum EventIds
    {
        /// <summary>
        /// 900001 - An unhandled exception occurred while processing the request.
        /// </summary>
        UnhandledControllerException = 900001,
        /// <summary>
        /// 900002 - Request/response information is logged successfully.
        /// </summary>
        LogRequest = 900002,
        /// <summary>
        /// 900003 - Error while redacting for response body for specific property.
        /// </summary>
        ErrorRedactingResponseBody = 900003,
        /// <summary>
        /// 900004 - Request sent to the server is incorrect or corrupt.
        /// </summary>
        BadRequest = 900004,
        /// <summary>
        /// 900005 - Server encountered an unexpected condition that prevented it from fulfilling the request.
        /// </summary>
        InternalServerError = 900005,
        /// <summary>
        /// 900006 - The requested resource has not been modified since the last time you accessed it.
        /// </summary>
        NotModified = 900006,
        /// <summary>
        /// 900007 - The requested resource responded with OK.
        /// </summary>
        OK = 900007,
        /// <summary>
        /// 900008 - The requested resource responded with Accepted.
        /// </summary>
        Accepted = 900008,
        /// <summary>
        /// 900009 - Event hub for external notification service is healthy.
        /// </summary>
        EventHubLoggingIsHealthy = 900009,
        /// <summary>
        /// 900010 - Event hub for external notification service is unhealthy.
        /// </summary>
        EventHubLoggingIsUnhealthy = 900010,
        /// <summary>
        /// 900011 -  Event data for external notification service event hub health check.
        /// </summary>
        EventHubLoggingEventDataForHealthCheck = 900011,
        /// <summary>
        /// 900012 - Request for external notification service post endpoint is started.
        /// </summary>
        ENSSubscriptionRequestStart = 900012,
        /// <summary>
        /// 900013 - Data truncation D365 Http Payload Size Exceeded.
        /// </summary>
        D365PayloadSizeExceededError = 900013,
        /// <summary>
        /// 900014 -  Event data for adding message in queue.
        /// </summary>
        AddedMessageInQueue = 900014,
        /// <summary>
        /// 900015 -  Create subscription web job is started.
        /// </summary>
        CreateSubscriptionRequestStart = 900015,
        /// <summary>
        /// 900016 -  Create subscription web job is Completed.
        /// </summary>
        CreateSubscriptionRequestCompleted = 900016,
        /// <summary>
        /// 900017 -  Create subscription service is started.
        /// </summary>
        CreateSubscriptionServiceStart = 900017,
        /// <summary>
        /// 900018 -  Create subscription service is Completed.
        /// </summary>
        CreateSubscriptionServiceCompleted = 900018,
        /// <summary>
        /// 900019 -  Create or updated azure event domain topic is started.
        /// </summary>
        CreateOrUpdateAzureEventDomainTopicStart = 900019,
        /// <summary>
        /// 900020 -  Create or updated azure event domain topic is Completed.
        /// </summary>
        CreateOrUpdateAzureEventDomainTopicCompleted = 900020,
        /// <summary>
        /// 900021 -  Create subscription request error in case of exception.
        /// </summary>
        CreateSubscriptionRequestError = 900021,
        /// <summary>
        /// 900022 - Azure blob storage for external notification service is healthy.
        /// </summary>
        AzureBlobStorageIsHealthy = 900022,
        /// <summary>
        /// 900023 - Azure blob storage for external notification service is unhealthy.
        /// </summary>
        AzureBlobStorageIsUnhealthy = 900023,
        /// <summary>
        /// 900024 - Azure message queue for external notification service is healthy.
        /// </summary>
        AzureMessageQueueIsHealthy = 900024,
        /// <summary>
        /// 900025 - Azure message queue for external notification service is unhealthy.
        /// </summary>
        AzureMessageQueueIsUnhealthy = 900025,
        /// <summary>
        /// 900026 -  Azure webjob for external notification service is healthy.
        /// </summary>
        AzureWebJobIsHealthy = 900026,
        /// <summary>
        /// 900027 -  Azure webjob for external notification service is unhealthy.
        /// </summary>
        AzureWebJobIsUnhealthy = 900027,
        /// <summary>
        /// 900028 -  Request for external notification service webhook post endpoint is started.
        /// </summary>
        ENSWebhookRequestStart = 900028,
        /// <summary>
        /// 900029 -  Requested for external notification service webhook options endpoint.
        /// </summary>
        ENSWebhookOptionsEndPointRequested= 900029,
        /// <summary>
        /// 900030 - Callbcak to D365 API using dataverse event started
        /// </summary>
        CallbackToD365Started = 900030,
        /// <summary>
        /// 900031 - Before calling HTTP client request to  Callback to D365 and after token authorization
        /// </summary>
        BeforeCallbackToD365 = 900031,
        /// <summary>
        /// 900032 - Log an error if call back to D365 API returns statusCode other than 204
        /// </summary>
        ErrorInCallbackToD365HttpClient = 900032,
        /// <summary>
        /// 900033 - Http Client request completed for Callback to D365
        /// </summary>
        CallbackToD365Completed = 900033,
        /// <summary>
        /// 900032 - Request for retrying D365 Callback Http endpoint
        /// </summary>
        RetryHttpClientD365CallbackRequest = 900034,
        /// <summary>
        /// 900035 - Authorization failed with AD Authentication Token
        /// </summary>
        ADAuthenticationFailed = 900035
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
