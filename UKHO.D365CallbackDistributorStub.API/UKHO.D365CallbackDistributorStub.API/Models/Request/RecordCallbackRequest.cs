namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class RecordCallbackRequest
    {
        public CallbackRequest? callBackRequest { get; set; }
        public string? subscriptionId { get; set; }
        public Guid guid { get; set; }
    }
}
