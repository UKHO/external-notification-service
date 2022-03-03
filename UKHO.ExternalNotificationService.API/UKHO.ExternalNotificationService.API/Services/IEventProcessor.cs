using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;
using UKHO.ExternalNotificationService.Common.Models.Response;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IEventProcessor
    {
        string EventType { get; }

        Task<ExternalNotificationServiceProcessResponse> Process(CustomEventGridEvent customEventGridEvent, string correlationId, CancellationToken cancellationToken = default);
    }
}
