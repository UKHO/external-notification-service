
using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.SubscriptionService.D365Callback
{
    public class ExternalNotificationEntityWithStateCode : ExternalNotificationEntity
    {
        [JsonPropertyName("statecode")]
        public int ResponseStateCode { get; set; }
    }
}
