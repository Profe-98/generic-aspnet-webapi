using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Net;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Cache.Distributed.RedisCache
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
