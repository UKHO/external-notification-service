using Newtonsoft.Json;

namespace UKHO.ExternalNotificationService.SubscriptionService.D365Callback
{
    public class ExternalNotificationEntityWithStateCode : ExternalNotificationEntity
    {
        [JsonProperty("statecode")]
        public int ResponseStateCode { get; set; }
    }
}
