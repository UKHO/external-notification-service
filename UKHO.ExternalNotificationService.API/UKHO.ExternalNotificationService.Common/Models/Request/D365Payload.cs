
namespace UKHO.ExternalNotificationService.Common.Models.Request
{
	public class D365Payload
	{
		public string CorrelationId { get; set; }
		public string OperationCreatedOn { get; set; }
		public InputParameter[] InputParameters { get; set; }
		public EntityImage[] PostEntityImages { get; set; }
	}

	public class EntityImage
	{
		public string key { get; set; }
		public EntityImageValue value { get; set; }
	}

	public class EntityImageValue
	{
		public D365Attribute[] Attributes { get; set; }
		public FormattedValue[] FormattedValues { get; set; }
	}
	public class InputParameter
	{
		public InputParameterValue value { get; set; }
	}
	public class InputParameterValue
	{
		public D365Attribute[] Attributes { get; set; }
		public FormattedValue[] FormattedValues { get; set; }
	}

	public class FormattedValue
	{
		public string key { get; set; }
		public object value { get; set; }
	}
	public class D365Attribute
	{
        public string key { get; set; }
        public object value { get; set; }
	}
}
