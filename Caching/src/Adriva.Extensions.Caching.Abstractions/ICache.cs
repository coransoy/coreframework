﻿using System;
using System.Threading.Tasks;

namespace Adriva.Extensions.Caching.Abstractions
{
    public delegate void EvictionCallback(string key, object value, ICache cache);

    /// <summary>
    /// Represents a cache type.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Gets or creates a new cache entry in the cache asynchronously.
        /// </summary>
        /// <param name="key">The identifier of the cache item.</param>
        /// <param name="factory">A factory method that creates the cache data if it already does not exist in the cache.</param>
        /// <param name="evictionCallback">A callback function that will be called when the cached item is evicted.</param>
        /// <param name="dependencyMonikers">List of dependency keys, so that if any of the dependencies expire the cached item will be removed from the cache.</param>
        /// <typeparam name="T">Type of the data that will be cached.</typeparam>
        /// <returns>A task that represents the asynchronous cache operation. The value of the TResult parameter contains the data retrieved from the cache if exists, or the result of the factory method.</returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<ICacheItem, Task<T>> factory, EvictionCallback evictionCallback = null, params string[] dependencyMonikers);

        /// <summary>
        /// Notifies the given dependency item of a change so that all cache dependencies are expired in the cache.
        /// </summary>
        /// <param name="dependencyMoniker">The identifier of the dependency.</param>
        void NotifyChanged(string dependencyMoniker);

        /// <summary>
        /// Notifies the given dependency item of a change so that all cached items depending on this moniker are evicted.
        /// </summary>
        /// <param name="dependencyMoniker">The identifier of the dependency.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task NotifyChangedAsync(string dependencyMoniker);
    }
}
