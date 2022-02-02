using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
