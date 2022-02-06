using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class D365Payload
    {
        public string CorrelationId { get; set; }
        public string OperationCreatedOn { get; set; }
        public EntityImage[] PostEntityImages { get; set; }
        public InputParameter[] InputParameters { get; set; }
    }

    public class InputParameter
    {
        [JsonPropertyName("value")]
        public InputParameterValue Value { get; set; }
    }

    public class InputParameterValue
    {
        public D365Attribute[] Attributes { get; set; }
        public FormattedValue[] FormattedValues { get; set; }
    }

    public class FormattedValue
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    public class D365Attribute
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }
    }

    public class EntityImage
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        public EntityImageValue ImageValue { get; set; }
    }

    public class EntityImageValue
    {
        public D365Attribute[] Attributes { get; set; }
        public FormattedValue[] FormattedValues { get; set; }
    }
}
