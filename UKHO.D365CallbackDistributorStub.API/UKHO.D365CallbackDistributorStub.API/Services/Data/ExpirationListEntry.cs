using System.Diagnostics.CodeAnalysis;

namespace UKHO.D365CallbackDistributorStub.API.Services.Data
{
    [ExcludeFromCodeCoverage]
    internal class ExpirationListEntry<T> where T : class
    {
        private readonly T _item;
        private readonly DateTimeOffset _timeStamp;

        public ExpirationListEntry(T item)
        {
            _item = item;
            _timeStamp = DateTimeOffset.UtcNow;
        }

        public T Item => _item;

        public DateTimeOffset Timestamp => _timeStamp;
    }
}
