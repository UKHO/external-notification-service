using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    public class CallbackService
    {
        static readonly Queue<RecordCallbackRequest> s_recordCallbackRequestQueue = new();

        public static bool SaveCallbackRequest(CallbackRequest callbackRequest, string subscriptionId)
        {
            try
            {
                s_recordCallbackRequestQueue.Enqueue(new RecordCallbackRequest
                {
                    CallbackRequest = callbackRequest,
                    Guid = Guid.NewGuid(),
                    SubscriptionId = subscriptionId

                });

                if (s_recordCallbackRequestQueue.Count >= 50)
                {
                    s_recordCallbackRequestQueue.Dequeue();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public  RecordCallbackRequest? GetCallbackRequest(string subscriptionId)
        {
            return s_recordCallbackRequestQueue.FirstOrDefault(a => a.SubscriptionId == subscriptionId);
        }
    }
}
