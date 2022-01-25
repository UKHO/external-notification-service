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
        /// 900007 - The requested resource responded with Accepted.
        /// </summary>
        Accepted = 900008
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
