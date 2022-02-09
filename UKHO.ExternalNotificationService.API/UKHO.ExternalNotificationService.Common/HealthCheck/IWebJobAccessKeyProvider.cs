
namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public interface IWebJobAccessKeyProvider
    {
        public string GetWebJobsAccessKey(string keyName);
    }
}
