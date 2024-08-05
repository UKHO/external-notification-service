
using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.SubscriptionService.D365Callback
{
    public class ExternalNotificationEntity
    {
        [JsonPropertyName("ukho_lastresponse")]
        public int ResponseStatusCode { get; set; }

        [JsonPropertyName("ukho_responsedetails")]
        public string ResponseDetails { get; set; }
    }
}
