//using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class CallbackRequest
    {
        [JsonPropertyName("ukho_lastresponse")]
        public int? ResponseStatusCode { get; set; }
        [JsonPropertyName("ukho_responsedetails")]
        public string? ResponseDetails { get; set; }
    }
}
