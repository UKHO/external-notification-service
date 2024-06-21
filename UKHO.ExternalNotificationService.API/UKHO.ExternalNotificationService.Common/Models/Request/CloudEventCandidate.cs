using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class CloudEventCandidate<T> where T : class
    {
        public string? DataContentType { get; set; }
        public string? Subject { get; set; }
        public string? DataSchema { get; set; }
        public T? Data { get; set; }
    }

}
