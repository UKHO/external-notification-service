using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        ErrorRedactingResponseBody = 900003
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
