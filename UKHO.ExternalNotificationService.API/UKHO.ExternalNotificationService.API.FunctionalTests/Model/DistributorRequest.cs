﻿using System;
using System.Net;

namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class DistributorRequest
    {
        public CustomCloudEvent CloudEvent { get; set; }
        public string Subject { get; set; }
        public Guid Guid { get; set; }
        public DateTime TimeStamp { get; set; }
        public HttpStatusCode? statusCode { get; set; }
    }
}
