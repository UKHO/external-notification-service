using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.D365CallbackDistributorStub.API.Models.Request;
using UKHO.D365CallbackDistributorStub.API.Services.Data;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class CallbackService
    {
        private static readonly TimeSpan s_queueExpiryInterval = TimeSpan.FromDays(1);

        private static readonly ExpirationList<RecordCallbackRequest> s_recordCallbackRequestQueue;
        private static readonly ExpirationList<CommandCallbackRequest> s_commandCallbackRequestList;
        private const HttpStatusCode NoContent = HttpStatusCode.NoContent;
        private readonly ILogger<CallbackService> _logger;

        static CallbackService()
        {
            s_recordCallbackRequestQueue = new ExpirationList<RecordCallbackRequest>(s_queueExpiryInterval);
            s_commandCallbackRequestList = new ExpirationList<CommandCallbackRequest>(s_queueExpiryInterval);
        }

        public CallbackService(ILogger<CallbackService> logger)
        {
            _logger = logger;
        }

        public static bool SaveCallbackRequest(CallbackRequest callbackRequest, string subscriptionId, HttpStatusCode? httpStatusCode)
        {
            try
            {
                s_recordCallbackRequestQueue.Add(new RecordCallbackRequest
                {
                    CallBackRequest = callbackRequest,
                    Guid = Guid.NewGuid(),
                    SubscriptionId = subscriptionId,
                    TimeStamp = DateTime.UtcNow,
                    HttpStatusCode = httpStatusCode ?? NoContent
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<RecordCallbackRequest>? GetCallbackRequest(string? subscriptionId)
        {
            if (!string.IsNullOrEmpty(subscriptionId))
            {
                return s_recordCallbackRequestQueue.Where(a => a != null && a.SubscriptionId == subscriptionId).ToList();
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
                CommandCallbackRequest? commandCallbackRequest = s_commandCallbackRequestList.LastOrDefault(a => a.SubscriptionId == subscriptionId);

                if (commandCallbackRequest != null)
                {
                    if (httpStatusCode == null)
                    {
                        s_commandCallbackRequestList.Remove(commandCallbackRequest);
                    }
                    else
                    {
                        commandCallbackRequest.HttpStatusCode = (HttpStatusCode)httpStatusCode;
                    }
                }
                else
                {
                    if (httpStatusCode != null)
                    {
                        s_commandCallbackRequestList.Add(new CommandCallbackRequest
                        {
                            SubscriptionId = subscriptionId,
                            HttpStatusCode = (HttpStatusCode)httpStatusCode,
                        });
                    }
                    else
                    {
                        _logger.LogInformation("Request not found in memory for subscriptionId: {subscriptionId}", subscriptionId);
                        return true;
                    }
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
            return s_commandCallbackRequestList.LastOrDefault(a => a.SubscriptionId == subscriptionId);
        }
    }
}
