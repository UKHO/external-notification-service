
using System;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class EnsCallbackResponseModel
    {
        public CallBackRequest callBackRequest { get; set; }
        public string subscriptionId { get; set; }
        public string guid { get; set; }
        public DateTime timeStamp { get; set; }
        public int httpStatusCode { get; set; }

    }
    public class CallBackRequest
    {
        public int ukho_lastresponse { get; set; }
        public string ukho_responsedetails { get; set; }
    }
}
