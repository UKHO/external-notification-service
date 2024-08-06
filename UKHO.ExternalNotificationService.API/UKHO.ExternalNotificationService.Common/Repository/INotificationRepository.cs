using System.Collections.Generic;
using UKHO.ExternalNotificationService.Common.Configuration;

namespace UKHO.ExternalNotificationService.Common.Repository
{
    public interface INotificationRepository
    {
        ICollection<NotificationType>? GetAllNotificationTypes();
    }
}
