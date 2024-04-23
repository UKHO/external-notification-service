using System.Text.Json.Serialization;

namespace UKHO.ExternalNotificationService.Common.Models.Request
{
    public class D365Payload
    {
        [JsonPropertyName(name: "correlationId")]
        public string CorrelationId { get; set; }
        [JsonPropertyName(name: "operationCreatedOn")]
        public string OperationCreatedOn { get; set; }
        [JsonPropertyName(name: "postEntityImages")]
        public EntityImage[] PostEntityImages { get; set; }
        [JsonPropertyName(name: "inputParameters")]
        public InputParameter[] InputParameters { get; set; }
    }

    public class InputParameter
    {
        [JsonPropertyName(name: "value")]
        public InputParameterValue Value { get; set; }
    }

    public class InputParameterValue
    {
        [JsonPropertyName(name: "attributes")]
        public D365Attribute[] Attributes { get; set; }
        [JsonPropertyName(name: "formattedValues")]
        public FormattedValue[] FormattedValues { get; set; }
    }

    public class FormattedValue
    {
        [JsonPropertyName(name: "key")]
        public string Key { get; set; }
        [JsonPropertyName(name: "value")]
        public object Value { get; set; }
    }

    public class D365Attribute
    {
        [JsonPropertyName(name: "key")]
        public string Key { get; set; }

        [JsonPropertyName(name: "value")]
        public object Value { get; set; }
    }

    public class EntityImage
    {
        [JsonPropertyName(name: "key")]
        public string Key { get; set; }
        [JsonPropertyName(name: "value")]
        public EntityImageValue Value { get; set; }
    }

    public class EntityImageValue
    {
        [JsonPropertyName(name: "attributes")]
        public D365Attribute[] Attributes { get; set; }
        [JsonPropertyName(name: "formattedValues")]
        public FormattedValue[] FormattedValues { get; set; }
    }
}
