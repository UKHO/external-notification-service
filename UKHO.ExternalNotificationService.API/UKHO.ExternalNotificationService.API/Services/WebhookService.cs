using System.Collections.Generic;
using System.Linq;

namespace UKHO.ExternalNotificationService.API.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IEnumerable<IEventProcessor> _eventProcessors;

        public WebhookService(IEnumerable<IEventProcessor> eventProcessors)
        {
            _eventProcessors = eventProcessors;
        }

        public IEventProcessor GetProcessor(string eventType)
        {
            return _eventProcessors.FirstOrDefault(x => x.EventType.Equals(eventType));
        }
    }
}
