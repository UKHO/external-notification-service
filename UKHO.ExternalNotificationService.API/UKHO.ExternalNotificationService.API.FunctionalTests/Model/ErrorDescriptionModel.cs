using System.Collections.Generic;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class ErrorDescriptionModel
    {
        public string CorrelationId { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Source { get; set; }
        public string Description { get; set; }
    }
}
