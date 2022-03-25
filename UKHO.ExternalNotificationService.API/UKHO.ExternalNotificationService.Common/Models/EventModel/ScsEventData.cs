using System;

namespace UKHO.ExternalNotificationService.Common.Models.EventModel
{
    public class ScsEventData
    {
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public int EditionNumber { get; set; }
        public int UpdateNumber { get; set; }
        public ProductBoundingBox BoundingBox { get; set; }
        public ProductUpdateStatus Status { get; set; }
        public bool IsPermitUpdateRequired { get; set; }
        public long FileSize { get; set; }
    }

    public class ProductBoundingBox
    {
        public double NorthLimit { get; set; }
        public double SouthLimit { get; set; }
        public double EastLimit { get; set; }
        public double WestLimit { get; set; }
    }

    public class ProductUpdateStatus
    {
        public DateTime? StatusDate { get; set; }
        public bool IsNewCell { get; set; }
        public string StatusName { get; set; }
    }
}
