using System.Collections.Generic;
using Microsoft.Extensions.Options;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IOptions<EnsConfiguration> _ensConfiguration;

        public NotificationRepository(IOptions<EnsConfiguration> ensConfiguration)
        {
            _ensConfiguration = ensConfiguration;
        }

        public ICollection<NotificationType> GetAllNotificationTypes()
        {
            return _ensConfiguration.Value.NotificationTypes;
        }
    }
}
