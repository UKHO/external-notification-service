
using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.SubscriptionService.D365Callback
{
    public class ExternalNotificationEntity
    {
        [JsonPropertyName("ResponseStatusCode")]
        public int ukho_lastresponse { get; set; }

        [JsonPropertyName("ResponseDetails")]
        public string ukho_responsedetails { get; set; }
    }
}
