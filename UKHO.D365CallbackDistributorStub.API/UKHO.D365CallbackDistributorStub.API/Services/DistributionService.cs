using System.Diagnostics.CodeAnalysis;
using UKHO.D365CallbackDistributorStub.API.Models.Request;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    [ExcludeFromCodeCoverage]
    public class DistributionService
    {
        private static readonly Queue<DistributorRequest> s_recordDistributorRequestQueue = new();
        public static bool SaveDistributorRequest(CustomCloudEvent cloudEvent)
        {
            try
            {
                s_recordDistributorRequestQueue.Enqueue(new DistributorRequest
                {
                    CloudEvent = cloudEvent,
                    Subject = cloudEvent.Subject,
                    Guid = new Guid(),
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
            if(!string.IsNullOrEmpty(subject))
            {
                return s_recordDistributorRequestQueue.Where(a => a.Subject == subject).ToList();
            }
            else
            {
                return s_recordDistributorRequestQueue.ToList();
            }
        }
    }
}
