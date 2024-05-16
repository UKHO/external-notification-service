
namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IWebJobAccessKeyProvider
    {
        string? GetWebJobsAccessKey(string keyName);
    }
}
