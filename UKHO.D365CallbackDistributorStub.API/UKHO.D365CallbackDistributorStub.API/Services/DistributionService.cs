using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class DistributionService
    {
        private static readonly Queue<DistributorRequest> s_recordDistributorRequestQueue = new();
        private static readonly List<CommandDistributionRequest> s_CommandDistributionList = new();
        private const HttpStatusCode _ok = HttpStatusCode.OK;
        private readonly ILogger<DistributionService> _logger;

        public DistributionService(ILogger<DistributionService> logger)
        {
            _logger = logger;
        }

        public static bool SaveDistributorRequest(CustomCloudEvent cloudEvent, HttpStatusCode? httpStatusCode)
        {
            try
            {
                s_recordDistributorRequestQueue.Enqueue(new DistributorRequest
                {
                    CloudEvent = cloudEvent,
                    Subject = cloudEvent.Subject,
                    Guid = new Guid(),
                    TimeStamp = CommonService.ToRfc3339String(DateTime.UtcNow),
                    StatusCode = httpStatusCode ?? _ok
                });

                if (s_recordDistributorRequestQueue.Count >= 50)
                {
                    s_recordDistributorRequestQueue.Dequeue();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<DistributorRequest>? GetDistributorRequest(string? subject)
        {
            if (!string.IsNullOrEmpty(subject))
            {
                return s_recordDistributorRequestQueue.Where(a => a.Subject == subject).ToList();
            }
            else
            {
                return s_recordDistributorRequestQueue.ToList();
            }
        }

        public bool SaveDistributorRequestForCommand(CustomCloudEvent cloudEvent, HttpStatusCode? statusCode)
        {
            try
            {
                CommandDistributionRequest? commanddistributorRequest = s_CommandDistributionList.FirstOrDefault(a => a.Subject == cloudEvent.Subject);
                if (commanddistributorRequest != null)
                {
                    if (statusCode == null)
                    {
                        s_CommandDistributionList.Remove(commanddistributorRequest);
                    }
                    else
                    {
                        commanddistributorRequest.HttpStatusCode = statusCode ?? _ok;
                    }
                }
                else
                {
                    if (statusCode != null)
                    {
                        s_CommandDistributionList.Add(new CommandDistributionRequest
                        {
                            Subject = cloudEvent.Subject,
                            HttpStatusCode = statusCode ?? _ok
                        });
                    }
                    else
                    {
                        _logger.LogInformation("Request not found in memory for subject: {subject}", cloudEvent.Subject);
                        return false;
                    }
                }

                if (s_CommandDistributionList.Count >= 50)
                {
                    s_CommandDistributionList.RemoveAt(0);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CommandDistributionRequest? SubscriptionInCommandDistributionList(string? subject)
        {
            return s_CommandDistributionList.Where(a => a.Subject == subject).FirstOrDefault();
        }
    }
}
