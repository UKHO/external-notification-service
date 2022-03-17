
using System;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class EnsCallbackResponseModel
    {
        public CallBackRequest CallBackRequest { get; set; }
        public string SubscriptionId { get; set; }
        public string Guid { get; set; }
        public DateTime TimeStamp { get; set; }
        public int HttpStatusCode { get; set; }

    }
    public class CallBackRequest
    {
        public int Ukho_lastresponse { get; set; }
        public string Ukho_responsedetails { get; set; }
    }
}
