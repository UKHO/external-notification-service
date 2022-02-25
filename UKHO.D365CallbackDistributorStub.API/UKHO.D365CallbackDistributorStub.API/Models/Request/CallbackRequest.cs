using Newtonsoft.Json;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class CallbackRequest
    {
        [JsonProperty("ukho_lastresponse")]
        public int? ResponseStatusCode { get; set; }
        [JsonProperty("ukho_responsedetails")]
        public string? ResponseDetails { get; set; }
    }
}
