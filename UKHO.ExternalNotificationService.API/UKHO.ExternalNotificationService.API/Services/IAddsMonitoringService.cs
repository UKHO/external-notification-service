using UKHO.ExternalNotificationService.Common.Models.Monitoring;

namespace UKHO.ExternalNotificationService.API.Services;

public interface IAddsMonitoringService
{
    Task StopProcessAsync(AddsData addsData, string correlationId, CancellationToken cancellationToken);
}
