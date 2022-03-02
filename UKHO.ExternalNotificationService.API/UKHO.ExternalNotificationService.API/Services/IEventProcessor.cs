using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using UKHO.ExternalNotificationService.Common.Models.Request;

namespace UKHO.ExternalNotificationService.API.Services
{
    public interface IEventProcessor
    {
        string EventType { get; }

        Task<IActionResult> Process(CustomEventGridEvent customEventGridEvent, string correlationId, CancellationToken cancellationToken = default);
    }
}
