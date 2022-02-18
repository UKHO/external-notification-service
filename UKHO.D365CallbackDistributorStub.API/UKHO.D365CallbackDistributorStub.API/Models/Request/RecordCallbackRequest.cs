namespace UKHO.D365CallbackDistributorStub.API.Models.Request
{
    public class RecordCallbackRequest
    {
        public CallbackRequest CallbackRequest  { get; set; }

        public string SubscriptionId { get; set; }

        public Guid Guid { get; set; }
    }
}
