
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
        /// 900014 -  Request for enterprise event service webhook post endpoint is started.
        /// </summary>
        EESWebhookRequestStart = 900014

    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
