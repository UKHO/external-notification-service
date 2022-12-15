using System.Collections;
using System.Collections.Immutable;

namespace UKHO.D365CallbackDistributorStub.API.Services.Data
{

    internal class ExpirationList<T> : IEnumerable<T> where T : class
    {
        private readonly TimeSpan _expirationInterval;
        private readonly ReaderWriterLockSlim _lock;

        private ImmutableList<ExpirationListEntry<T>> _list;

        /// <summary>
        /// Creates an instance of a <see cref="ExpirationList{T}"/>
        /// </summary>
        /// <param name="expirationInterval">The </param>
        /// <exception cref="ArgumentException"></exception>
        public ExpirationList(TimeSpan expirationInterval)
        {
            if (expirationInterval.TotalMilliseconds <= 0)
            {
                throw new ArgumentException($"Expiration interval {expirationInterval} is <= 0");
            }

            _expirationInterval = expirationInterval;

            _list = ImmutableList<ExpirationListEntry<T>>.Empty;
            _lock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Add an instance to the list
        /// </summary>
        /// <param name="instance"></param>
        public void Add(T instance)
        {
            PurgeList();

            try
            {
                _lock.EnterWriteLock();
                _list = _list.Add(new ExpirationListEntry<T>(instance));
            }

            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Remove an instance from the list
        /// </summary>
        /// <param name="instance"></param>
        public void Remove(T instance)
        {
            PurgeList();

            try
            {
                _lock.EnterUpgradeableReadLock();

                var existingItem = _list.SingleOrDefault(x => x.Item.Equals(instance));

                if (existingItem != null)
                {
                    try
                    {
                        _lock.EnterWriteLock();
                        _list = _list.Remove(existingItem);
                    }

                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }

            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerator{T}"/> over the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            PurgeList();

            try
            {
                _lock.EnterReadLock();

                var list = _list.Select(x => x.Item).ToImmutableList();

                return list.GetEnumerator();
            }

            finally
            {
                _lock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void PurgeList()
        {
            try
            {
                _lock.EnterWriteLock();

                foreach (var item in _list.Where(item => DateTimeOffset.UtcNow - item.Timestamp > _expirationInterval))
                {
                    _list = _list.Remove(item);
                }
            }

            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
