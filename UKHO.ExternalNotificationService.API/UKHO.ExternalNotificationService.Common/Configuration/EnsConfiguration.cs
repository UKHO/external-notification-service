using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.Common.Configuration
{
    public class EnsConfiguration
    {
        public ICollection<NotificationType> NotificationTypes { get; set; }
    }
    public class NotificationType
    {
        public string Name { get; set; }

        /// <summary>
        /// Topic name can only contain A-Z, a-z, 0-9, and the '-'
        /// </summary>
        public string TopicName { get; set; }
    }
}
