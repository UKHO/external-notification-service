using System.Diagnostics.CodeAnalysis;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class CallbackService
    {
        static readonly Queue<RecordCallbackRequest> s_recordCallbackRequestQueue = new();
        public static bool SaveCallbackRequest(CallbackRequest callbackRequest, string subscriptionId)
        {
            try
            {
                s_recordCallbackRequestQueue.Enqueue(new RecordCallbackRequest
                {
                    callBackRequest = callbackRequest,
                    guid = Guid.NewGuid(),
                    subscriptionId = subscriptionId
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

        public  List<RecordCallbackRequest>? GetCallbackRequest(string? subscriptionId)
        {
            if(!string.IsNullOrEmpty(subscriptionId))
            {
                return s_recordCallbackRequestQueue.Where(a => a.subscriptionId == subscriptionId).ToList();
            }
            else
            {
                return s_recordCallbackRequestQueue.ToList();
            }

        }
    }
}
