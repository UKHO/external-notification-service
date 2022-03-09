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
        private const HttpStatusCode Ok = HttpStatusCode.OK;
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
                    Guid = Guid.NewGuid(),
                    TimeStamp =DateTime.UtcNow,
                    StatusCode = httpStatusCode ?? Ok
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

        public bool SaveCommandDistributorRequest(string subject, HttpStatusCode? httpStatusCode)
        {
            try
            {
                CommandDistributionRequest? commandDistributorRequest = s_CommandDistributionList.LastOrDefault(a => a.Subject == subject);
                if (commandDistributorRequest != null)
                {
                    if (httpStatusCode == null)
                    {
                        s_CommandDistributionList.Remove(commandDistributorRequest);
                    }
                    else
                    {
                        commandDistributorRequest.HttpStatusCode = (HttpStatusCode)httpStatusCode;
                    }
                }
                else
                {
                    if (httpStatusCode != null)
                    {
                        s_CommandDistributionList.Add(new CommandDistributionRequest
                        {
                            Subject = subject,
                            HttpStatusCode = (HttpStatusCode)httpStatusCode
                        });
                    }
                    else
                    {
                        _logger.LogInformation("Request not found in memory for subject: {subject}", subject);
                        return true;
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

        public CommandDistributionRequest? SubjectInCommandDistributionList(string? subject)
        {
            return s_CommandDistributionList.LastOrDefault(a => a.Subject == subject);
        }
    }
}
