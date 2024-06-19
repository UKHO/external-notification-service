using System.Diagnostics.CodeAnalysis;
using System.Net;
using UKHO.D365CallbackDistributorStub.API.Models.Request;
using UKHO.D365CallbackDistributorStub.API.Services.Data;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class DistributionService
    {
        private static readonly TimeSpan s_queueExpiryInterval = TimeSpan.FromDays(1);

        private static readonly ExpirationList<DistributorRequest> s_recordDistributorRequestQueue;
        private static readonly ExpirationList<CommandDistributionRequest> s_commandDistributionList;

        private const HttpStatusCode Ok = HttpStatusCode.OK;
        private readonly ILogger<DistributionService> _logger;

        static DistributionService()
        {
            s_recordDistributorRequestQueue = new ExpirationList<DistributorRequest>(s_queueExpiryInterval);
            s_commandDistributionList = new ExpirationList<CommandDistributionRequest>(s_queueExpiryInterval);
        }

        public DistributionService(ILogger<DistributionService> logger)
        {
            _logger = logger;
        }

        public static bool SaveDistributorRequest(CustomCloudEvent cloudEvent, HttpStatusCode? httpStatusCode)
        {
            try
            {
                s_recordDistributorRequestQueue.Add(new DistributorRequest
                {
                    CloudEvent = cloudEvent,
                    Subject = cloudEvent.Subject,
                    Guid = Guid.NewGuid(),
                    TimeStamp = DateTime.UtcNow,
                    StatusCode = httpStatusCode ?? Ok
                });

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
                return s_recordDistributorRequestQueue.Where(a => a != null && a.Subject == subject).ToList();
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
                CommandDistributionRequest? commandDistributorRequest = s_commandDistributionList.LastOrDefault(a => a.Subject == subject);
                if (commandDistributorRequest != null)
                {
                    if (httpStatusCode == null)
                    {
                        s_commandDistributionList.Remove(commandDistributorRequest);
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
                        s_commandDistributionList.Add(new CommandDistributionRequest
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

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CommandDistributionRequest? SubjectInCommandDistributionList(string? subject)
        {
            return s_commandDistributionList.LastOrDefault(a => a.Subject == subject);
        }

        public void ClearDistributorQueue()
        {
            var distQueue = s_recordDistributorRequestQueue.ToList();

            if (distQueue.Count > 0)
            {
                foreach (var dist in distQueue)
                {
                    s_recordDistributorRequestQueue.Remove(dist);
                }
            }
        }
    }
}

