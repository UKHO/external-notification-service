
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace UKHO.ExternalNotificationService.Common.HealthCheck
{
    public class WebJobAccessKeyProvider : IWebJobAccessKeyProvider
    {
        private readonly IDictionary<string, string?> _webJobsAccessKey;
        public WebJobAccessKeyProvider(IConfiguration config)
        {
            _webJobsAccessKey = config.AsEnumerable()
                                .ToList()
                                .FindAll(kv => kv.Key != null)
                                .ToDictionary(x => x.Key, x => x.Value);
        }

        public string? GetWebJobsAccessKey(string keyName)
        {
            if (_webJobsAccessKey.TryGetValue(keyName, out string? accessKey))
            {
                return accessKey;
            }
            return null;
        }
    }
}
