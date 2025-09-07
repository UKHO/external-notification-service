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
        /// 900021 -  Create Subscription Provisioning request succeeded for webhook event
        /// </summary>
        CreateSubscriptionRequestSuccess = 900021,
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
        ENSWebhookOptionsEndPointRequested = 900029,
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
        /// 900034 - Request for retrying D365 Callback Http endpoint
        /// </summary>
        RetryHttpClientD365CallbackRequest = 900034,
        /// <summary>
        /// 900035 - Authorization failed with AD Authentication Token
        /// </summary>
        ADAuthenticationFailed = 900035,
        /// <summary>
        /// 900036 - Create subscription request error in case of other exception in Webhook event
        /// </summary>
        CreateSubscriptionRequestOtherError = 900036,
        /// <summary>
        /// 900037 - Create subscription request error in case of handshake failure exception in Webhook event
        /// </summary>
        CreateSubscriptionRequestHandshakeFailureError = 900037,
        /// <summary>
        /// 900038 -  Request for external notification service webhook post endpoint is Completed.
        /// </summary>
        ENSWebhookRequestCompleted = 900038,
        /// <summary>
        /// 900039 -  Request event type for external notification service webhook not matched with processor.
        /// </summary>
        ENSWebhookRequestTypeNotMatch = 900039,
        /// <summary>
        /// 900040 -  Request invalid event payload for external notification service webhook post endpoint.
        /// </summary>
        ENSWebhookRequestInvalidEventPayload = 900040,
        /// <summary>
        /// 900041 -  Request null event payload for external notification service webhook post endpoint.
        /// </summary>
        ENSWebhookRequestWithNullEventPayload = 900041,
        /// <summary>
        /// 900042 -  File share service event data mapping started.
        /// </summary>
        FssEventDataMappingStart = 900042,
        /// <summary>
        /// 900043 -  File share service event data mapping Completed.
        /// </summary>
        FssEventDataMappingCompleted = 900043,
        /// <summary>
        /// 900044 -  File share service event data discarded for business unit.
        /// </summary>
        FssEventDataDiscardedForBusinessUnit = 900044,
        /// <summary>
        /// 900045 -  External notification service event publish started.
        /// </summary>
        ENSEventPublishStart = 900045,
        /// <summary>
        /// 900046 -  External notification service event publish Completed.
        /// </summary>
        ENSEventPublishCompleted = 900046,
        /// <summary>
        /// 900047 -  External notification service event publish is failed.
        /// </summary>
        ENSEventNotPublished = 900047,
        /// <summary>
        /// 900048 - Delete Event Subscription Provisioning request succeeded for webhook event
        /// </summary>
        DeleteSubscriptionRequestSuccess = 900048,
        /// <summary>
        /// 900049 -  Delete Event Subscription Provisioning request error in case of exception for webhook event
        /// </summary>
        DeleteSubscriptionRequestError = 900049,
        /// <summary>
        /// 900050 - Delete Subscription Service event started
        /// </summary>
        DeleteSubscriptionServiceEventStart = 900050,
        /// <summary>
        /// 900051 - Delete Subscription Service event completed
        /// </summary>
        DeleteSubscriptionServiceEventCompleted = 900051,
        /// <summary>
        /// 900052 -  Delete Azure Event Domain Subscription Start
        /// </summary>
        DeleteAzureEventDomainSubscriptionStart = 900052,
        /// <summary>
        /// 900053 -  Delete Azure Event Domain Subscription Completed
        /// </summary>
        DeleteAzureEventDomainSubscriptionCompleted = 900053,
        /// <summary>
        /// 900054 -  Sales catalogue service event data mapping started.
        /// </summary>
        ScsEventDataMappingStart = 900054,
        /// <summary>
        /// 900055 -  Sales catalogue service event data mapping Completed.
        /// </summary>
        ScsEventDataMappingCompleted = 900055,
        /// <summary>
        /// 900056 -  External notification service - dead letter container webjob request started.
        /// </summary>
        ENSDeadLetterContainerJobRequestStart = 900056,
        /// <summary>
        /// 900057 -  External notification service - dead letter container webjob request completed.
        /// </summary>
        ENSDeadLetterContainerJobRequestCompleted = 900057,
        /// <summary>
        /// 900058 -  Callback to D365 started for dead letter processing to mark subscription as inactive.
        /// </summary>
        CallbackToD365ForDeadLetterProcessingStarted = 900058,
        /// <summary>
        /// 900059 - Log an error if call back to D365 API for dead letter processing returns statusCode other than 204
        /// </summary>
        ErrorInDeadLetterCallbackToD365HttpClient = 900059,
        /// <summary>
        /// 900060 - Callback to D365 succeeded for dead letter processing to mark subscription as inactive.
        /// </summary>
        CallbackToD365ForDeadLetterProcessingSucceeded = 900060,
        /// <summary>
        /// 900061 - Process to mark subscription as inactive due to failed notification delivery started.
        /// </summary>
        ENSSubscriptionMarkedAsInactiveStart = 900061,
        /// <summary>
        /// 900062 - Process to mark subscription as inactive due to failed notification delivery completed.
        /// </summary>
        ENSSubscriptionMarkedAsInactiveCompleted = 900062,
        /// <summary>
        /// 900063 - Process to copy dead letter container blob to destination container blob started.
        /// </summary>
        ENSCopyDeadLetterContainerBlobStarted = 900063,
        /// <summary>
        /// 900064 - Process to copy dead letter container blob to destination container blob completed.
        /// </summary>
        ENSCopyDeadLetterContainerBlobCompleted = 900064,
        /// <summary>
        /// 900065 -  File share service event data mapping configuration error.
        /// </summary>
        FSSEventDataMappingConfigurationError = 900065,
        /// <summary>
        /// 900066 -  Elasticsearch Client is not configured.
        /// </summary>
        ElasticsearchClientNotConfigured = 900066,
        /// <summary>
        /// 900067 -  Stop ADDS Elastic Monitoring Process Start.
        /// </summary>
        StopAddsElasticMonitoringProcessStart = 900067,
        /// <summary>
        /// 900068 -  Stop ADDS Elastic Monitoring Process completed.
        /// </summary>
        StopAddsElasticMonitoringProcessCompleted = 900068,
        /// <summary>
        /// 900069 -  Stop ADDS Elastic Monitoring Process error.
        /// </summary>
        StopAddsElasticMonitoringProcessError = 900069,
        /// <summary>
        /// 900070 -  Sales catalogue service s100 event data mapping started.
        /// </summary>
        ScsS100EventDataMappingStart = 900070,
        /// <summary>
        /// 900071 -  Sales catalogue service s100 event data mapping Completed.
        /// </summary>
        ScsS100EventDataMappingCompleted = 900071,

    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
