using System;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Data.Format.Json;

namespace Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache
{
    public class CachingHandler : ICachingHandler, IDisposable
    {
        #region Private
        private readonly ILogger _logger;
        private readonly bool _useLocalCache = true;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IMemoryCache _localCache;
        /*private readonly IDistributedMainCache _mainCache;
        private readonly IDistributedFailoverCache _failoverCache;*/
        private readonly ISingletonJsonHandler _jsonHandler;
        private readonly IAppconfig _appconfig;

        #endregion

        #region Public
        #endregion
        #region Ctor & Dtor
        public CachingHandler(ILogger<CachingHandler> logger, IConnectionMultiplexer connectionMultiplexer, IAppconfig appconfig, ISingletonJsonHandler jsonHandler, IMemoryCache localCache/*, IDistributedMainCache mainDistributedCache,IDistributedFailoverCache failoverDistributedCache*/)
        {
            _localCache = localCache;
            /*_mainCache = mainDistributedCache;
            _failoverCache = failoverDistributedCache;*/
            _jsonHandler = jsonHandler;
            _connectionMultiplexer = connectionMultiplexer;
            _useLocalCache = true;
            _appconfig = appconfig;
            _logger = logger;

        }
        public CachingHandler(ILogger<CachingHandler> logger, ISingletonJsonHandler jsonHandler, IAppconfig appconfig, IMemoryCache localCache/*, IDistributedMainCache mainDistributedCache,IDistributedFailoverCache failoverDistributedCache*/)
        {
            _localCache = localCache;
            /*_mainCache = mainDistributedCache;
            _failoverCache = failoverDistributedCache;*/
            _jsonHandler = jsonHandler;
            _useLocalCache = true;
            _appconfig = appconfig;
            _logger = logger;

        }
        ~CachingHandler()
        {
            Dispose();
        }
        #endregion
        #region Methods

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<IServer> Nodes
        {
            get
            {
                List<IServer> servers = new List<IServer>();
                if (_connectionMultiplexer != null)
                {
                    var endpoints = _connectionMultiplexer.GetEndPoints();
                    if (endpoints.Length != 0)
                    {
                        endpoints.ToList().ForEach(e =>
                        {

                            servers.Add(_connectionMultiplexer.GetServer(e));
                        });
                    }
                }
                return servers;
            }
        }
        public long PingLocalCache()
        {
            long response = GeneralDefs.NotFoundResponseValue;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                _localCache.Get("");
                stopwatch.Stop();
                response = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response = GeneralDefs.NotFoundResponseValue;
            }
            return response;
        }
        public async Task<Dictionary<EndPoint, long>> Ping()
        {
            try
            {
                Dictionary<EndPoint, long> response = new Dictionary<EndPoint, long>();
                EndPoint[] endPoints = _appconfig.AppServiceConfiguration?.CacheConfigurationModel?.Hosts?.Select(x => x.EndPoint)?.ToArray();
                for (int i = 0; i < endPoints.Length; i++)
                {
                    long responseMs = GeneralDefs.NotFoundResponseValue;
                    response.TryAdd(endPoints[i], responseMs);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    try
                    {
                        if (_connectionMultiplexer == null)
                            continue;

                        bool conn = _connectionMultiplexer.GetServer(endPoints[i]).IsConnected;
                        stopwatch.Stop();
                        if (conn)
                            responseMs = stopwatch.ElapsedMilliseconds;
                        else
                            throw new Exception();
                    }
                    catch (Exception ex)
                    {
                        responseMs = GeneralDefs.NotFoundResponseValue;
                    }
                    response[endPoints[i]] = responseMs;
                }
                return response;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
            }
            return null;

            /*try
            {
                await _mainCache.GetAsync("");
                stopwatch.Stop();
                response[1] = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                response[1] = GeneralDefs.NotFoundResponseValue;
            }

            stopwatch.Restart();
            try
            {
                await _failoverCache.GetAsync("");
                response[2] = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                response[2] = GeneralDefs.NotFoundResponseValue;
            }
            stopwatch.Stop();*/
        }

        public void ReInitConnectionMultiplexer(IServiceProvider serviceProvider)
        {

            if (_connectionMultiplexer == null)
            {

                var cacheConfig = _appconfig.AppServiceConfiguration.CacheConfigurationModel;
                var serviceCollection = serviceProvider.GetService<IServiceCollection>();

                if (serviceCollection != null)
                {
                    serviceCollection.UseServerSideCache(cacheConfig);
                    var sP = serviceCollection.BuildServiceProvider();
                }
            }
        }

        public async Task<string> GetStringAsync(string key, CancellationToken token = default)
        {
            try
            {
                if (_localCache.TryGetValue(key, out object value) && _useLocalCache && value != null)
                {
                    return value.ToString();
                }
                else
                {
                    if (_connectionMultiplexer == null)
                        return null;
                    string resultFromMain = null;
                    try
                    {
                        resultFromMain = await _connectionMultiplexer.GetDatabase().StringGetAsync(key);
                        if (resultFromMain != null)
                        {
                            _localCache.Set(key, resultFromMain);
                        }
                        return resultFromMain;
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(ex.Message, ex);
                    }
                    /*string resultFromMain = null;
                    try
                    {
                        resultFromMain = await _mainCache.GetStringAsync(key, token);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (resultFromMain != null)
                    {
                        return resultFromMain;
                    }
                    else
                    {
                        string resultFromFailover = null;
                        try
                        {
                            resultFromFailover = await _failoverCache.GetStringAsync(key, token);
                        }
                        catch (Exception ex)
                        {

                        }
                        if (resultFromFailover != null)
                        {
                            return resultFromFailover;
                        }

                    }*/
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
            }
            return null;
        }


        public async Task<bool> RemoveAsync(string key, CancellationToken token = default)
        {
            try
            {
                if (_useLocalCache)
                    _localCache.Remove(key);

                if (_connectionMultiplexer == null)
                    return false;
                try
                {
                    bool response = await _connectionMultiplexer.GetDatabase().KeyDeleteAsync(key);
                    return response;
                }
                catch (Exception ex)
                {
                    return false;
                }
                /*_mainCache.RemoveAsync(key,token);
                _failoverCache.RemoveAsync(key, token);*/
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
            }
            return false;
        }


        public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            try
            {
                if (_useLocalCache)
                    _localCache.Set(key, value, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow });

                if (_connectionMultiplexer == null)
                    return Task.CompletedTask;

                try
                {
                    try
                    {
                        _connectionMultiplexer.GetDatabase().StringSetAsync(key, value, expiry: options.AbsoluteExpirationRelativeToNow);

                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(ex.Message, ex);
                    }
                    /*_mainCache.SetStringAsync(key, value, options, token);
                    
                    try
                    {
                        _failoverCache.SetStringAsync(key, value, options, token);

                    }
                    catch (Exception ex)
                    {

                    }*/
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return Task.CompletedTask;
        }

        public async Task<List<RedisKey>> GetKeys(string filter)
        {
            List<RedisKey> response = new List<RedisKey>();
            foreach (IServer server in Nodes)
            {
                IAsyncEnumerable<RedisKey> keys = server.KeysAsync(pattern: filter, flags: CommandFlags.PreferMaster);
                await foreach (var item in keys.WithCancellation(new CancellationToken()).ConfigureAwait(false))
                {
                    if (!response.Contains(item))
                        response.Add(item);
                }
            }
            return response;
        }

        #endregion
    }

}
