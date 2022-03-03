using System.Globalization;

namespace UKHO.D365CallbackDistributorStub.API.Services
{
    public static class CommonService
    {
        public static string ToRfc3339String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
