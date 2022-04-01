using Newtonsoft.Json;


namespace UKHO.ExternalNotificationService.SubscriptionService.D365Callback
{
    public class ExternalNotificationEntity
    {
        [JsonProperty("ukho_lastresponse")]
        public int ResponseStatusCode { get; set; }

        [JsonProperty("ukho_responsedetails")]
        public string ResponseDetails { get; set; }

        [JsonProperty("statecode")]
        public int ResponseStateCode { get; set; }
    }
}
