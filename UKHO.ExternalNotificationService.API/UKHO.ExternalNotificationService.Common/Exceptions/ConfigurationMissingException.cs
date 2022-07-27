using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace UKHO.ExternalNotificationService.Common.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ConfigurationMissingException : ConfigurationErrorsException
    {
        public ConfigurationMissingException()
        { }

        public ConfigurationMissingException(string message)
            : base(message)
        { }
    }
}
