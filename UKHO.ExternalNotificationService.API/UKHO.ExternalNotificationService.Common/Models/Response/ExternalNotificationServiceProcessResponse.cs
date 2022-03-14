using System.Collections.Generic;
using System.Net;

namespace UKHO.ExternalNotificationService.Common.Models.Response
{
    public class ExternalNotificationServiceProcessResponse
    {
        public string BusinessUnit { get; set; }
        public List<Error> Errors { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
