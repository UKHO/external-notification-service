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
        /// 805001 - Event hub for exchange set service is healthy.
        /// </summary>
        EventHubLoggingIsHealthy = 805001,
        /// <summary>
        /// 805002 - Event hub for exchange set service is unhealthy.
        /// </summary>
        EventHubLoggingIsUnhealthy = 805002,
        /// <summary>
        /// 805003 -  Event data for exchange set service event hub health check.
        /// </summary>
        EventHubLoggingEventDataForHealthCheck = 805003
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
