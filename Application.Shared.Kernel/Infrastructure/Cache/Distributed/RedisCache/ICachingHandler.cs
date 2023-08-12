using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Net;

namespace Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache
{

    public interface ICachingHandler
    {

        public Task<bool> RemoveAsync(string key, CancellationToken token = default);
        public long PingLocalCache();
        public Task<Dictionary<EndPoint, long>> Ping();
        public Task<List<RedisKey>> GetKeys(string filter);
        public Task<string> GetStringAsync(string key, CancellationToken token = default);
        public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions distributedCacheEntryOptions, CancellationToken token = default);
    }
    public interface IDistributedMainCache : IDistributedCache
    {

    }
    public interface IDistributedFailoverCache : IDistributedCache
    {

    }
    public class LocalCacheOptions : MemoryCacheOptions
    {
        public LocalCacheOptions()
        {

        }
    }
    public class RedisCacheOptionsMainCache : RedisCacheOptions
    {
        public RedisCacheOptionsMainCache()
        {

        }
    }
    public class RedisCacheOptionsFailoverCache : RedisCacheOptions
    {
        public RedisCacheOptionsFailoverCache()
        {

        }
    }

    public class RedisMainCache : Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache, IDistributedMainCache
    {
        public RedisMainCache(IOptions<RedisCacheOptionsMainCache> optionsAccessor) : base(optionsAccessor)
        {

        }
    }

    public class RedisFailoverCache : Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache, IDistributedFailoverCache
    {
        public RedisFailoverCache(IOptions<RedisCacheOptionsFailoverCache> optionsAccessor) : base(optionsAccessor)
        {

        }
    }
}
