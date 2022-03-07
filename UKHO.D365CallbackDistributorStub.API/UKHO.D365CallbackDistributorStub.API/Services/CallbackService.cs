using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class CallbackService 
    {
        private static readonly Queue<RecordCallbackRequest> s_recordCallbackRequestQueue = new();
        private static readonly List<CommandCallbackRequest> s_commandCallbackRequestList = new();
        private const HttpStatusCode _noContent = HttpStatusCode.NoContent;
        private readonly ILogger<CallbackService> _logger;

        public CallbackService(ILogger<CallbackService> logger)
        {
            _logger = logger;
        }

        public static bool SaveCallbackRequest(CallbackRequest callbackRequest, string subscriptionId, HttpStatusCode? httpStatusCode)
        {
            try
            {
                s_recordCallbackRequestQueue.Enqueue(new RecordCallbackRequest
                {
                    CallBackRequest = callbackRequest,
                    Guid = Guid.NewGuid(),
                    SubscriptionId = subscriptionId,
                    TimeStamp = CommonService.ToRfc3339String(DateTime.UtcNow),
                    HttpStatusCode = httpStatusCode ?? _noContent
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

        public bool SaveCommandCallbackRequest(string subscriptionId, HttpStatusCode? httpStatusCode)
        {
            try
            {
                CommandCallbackRequest? commandCallbackRequest = s_commandCallbackRequestList.FirstOrDefault(a => a.SubscriptionId == subscriptionId);

                if (commandCallbackRequest != null)
                {
                    if (httpStatusCode == null)
                    {
                        s_commandCallbackRequestList.Remove(commandCallbackRequest);
                    }
                    else
                    {
                        commandCallbackRequest.HttpStatusCode = httpStatusCode ?? _noContent;
                    }
                }
                else
                {
                    if (httpStatusCode != null)
                    {
                        s_commandCallbackRequestList.Add(new CommandCallbackRequest
                        {
                            SubscriptionId = subscriptionId,
                            HttpStatusCode = httpStatusCode ?? _noContent,
                        });
                    }
                    else
                    {
                        _logger.LogInformation("Request not found in memory for subscriptionId: {subscriptionId}", subscriptionId);
                        return true;
                    }
                }

                if (s_commandCallbackRequestList.Count >= 50)
                {
                    s_commandCallbackRequestList.RemoveAt(0);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public CommandCallbackRequest? SubscriptionInCommandCallbackList(string subscriptionId)
        {
           return s_commandCallbackRequestList.Where(a => a.SubscriptionId == subscriptionId).FirstOrDefault();
        }
    }
}
