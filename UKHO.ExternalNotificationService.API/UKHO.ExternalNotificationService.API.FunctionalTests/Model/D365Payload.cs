
namespace UKHO.ExternalNotificationService.API.FunctionalTests.Model
{
    public class D365Payload
    {
        public string CorrelationId;
        public string OperationCreatedOn;
        public InputParameter[] InputParameters;
        public EntityImage[] PostEntityImages;
    }

    public class EntityImage
    {
        public string key;
        public EntityImageValue value;
    }

    public class EntityImageValue
    {
        public D365Attribute[] Attributes;
        public FormattedValue[] FormattedValues;
    }
    public class InputParameter
    {
        public InputParameterValue value;
    }
    public class InputParameterValue
    {
        public D365Attribute[] Attributes;
        public FormattedValue[] FormattedValues;
    }

    public class FormattedValue
    {
        public string key;
        public object value;
    }
    public class D365Attribute
    {
        public string key;
        public object value;
    }
}
