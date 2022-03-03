using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class CallbackService
    {
        private static readonly Queue<RecordCallbackRequest> s_recordCallbackRequestQueue = new();
        private static readonly List<FailCallbackRequest> s_failCallbackRequestList = new();
        public static bool SaveCallbackRequest(CallbackRequest callbackRequest, string subscriptionId)
        {
            try
            {
                s_recordCallbackRequestQueue.Enqueue(new RecordCallbackRequest
                {
                    CallBackRequest = callbackRequest,
                    Guid = Guid.NewGuid(),
                    SubscriptionId = subscriptionId,
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo),
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
                return s_recordCallbackRequestQueue.Where(a => a.SubscriptionId == subscriptionId).ToList();
            }
            else
            {
                return s_recordCallbackRequestQueue.ToList();
            }

        }

        public bool SaveFailCallbackRequest(string subscriptionId, HttpStatusCode? httpStatusCode)
        {
            try
            {
                FailCallbackRequest? failCallbackRequest = s_failCallbackRequestList.FirstOrDefault(a => a.SubscriptionId == subscriptionId);

                if (failCallbackRequest != null)
                {
                    if (httpStatusCode == null)
                    {
                        s_failCallbackRequestList.Remove(failCallbackRequest);
                    }
                    else
                    {
                        failCallbackRequest.httpStatusCode = httpStatusCode ?? new HttpStatusCode();
                    }
                }
                else
                {
                    if (httpStatusCode != null)
                    {
                        s_failCallbackRequestList.Add(new FailCallbackRequest
                        {
                            SubscriptionId = subscriptionId,
                            httpStatusCode = httpStatusCode ?? new HttpStatusCode(),
                        });
                    }
                    else
                    {
                        return false;
                    }
                }

                if (s_failCallbackRequestList.Count >= 50)
                {
                    s_failCallbackRequestList.RemoveAt(0);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public FailCallbackRequest? SubscriptionInFailCallbackList(string subscriptionId)
        {
           return s_failCallbackRequestList.Where(a => a.SubscriptionId == subscriptionId).FirstOrDefault();
        }
    }
}
