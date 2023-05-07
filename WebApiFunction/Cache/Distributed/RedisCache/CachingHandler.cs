using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Data;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using StackExchange.Redis.Profiling;
using StackExchange.Redis;
using WebApiFunction.Cache.Distributed.RedisCache;
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
    public class CachingHandler : ICachingHandler, IDisposable
    {
        #region Private
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
        public CachingHandler(IConnectionMultiplexer connectionMultiplexer, IAppconfig appconfig, ISingletonJsonHandler jsonHandler, IMemoryCache localCache/*, IDistributedMainCache mainDistributedCache,IDistributedFailoverCache failoverDistributedCache*/)
        {
            _localCache = localCache;
            /*_mainCache = mainDistributedCache;
            _failoverCache = failoverDistributedCache;*/
            _jsonHandler = jsonHandler;
            _connectionMultiplexer = connectionMultiplexer;
            _useLocalCache = true;
            _appconfig = appconfig;

        }
        public CachingHandler(ISingletonJsonHandler jsonHandler, IAppconfig appconfig, IMemoryCache localCache/*, IDistributedMainCache mainDistributedCache,IDistributedFailoverCache failoverDistributedCache*/)
        {
            _localCache = localCache;
            /*_mainCache = mainDistributedCache;
            _failoverCache = failoverDistributedCache;*/
            _jsonHandler = jsonHandler;
            _useLocalCache = true;
            _appconfig = appconfig;

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
                response = GeneralDefs.NotFoundResponseValue;
            }
            return response;
        }
        public async Task<Dictionary<EndPoint, long>> Ping()
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
            return response;
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
                if (_localCache.TryGetValue(key, out object value) && _useLocalCache)
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

                }
            }
            catch (Exception ex)
            {
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

    public static class CachingHandlerExtensions
    {
        public static IConnectionMultiplexer CreateConnectionMultiplexer(CacheConfigurationModel cacheConfigurationModel)
        {
            if (cacheConfigurationModel == null)
                return null;
            var hosts = cacheConfigurationModel?.Hosts?.ToList();
            EndPoint[] endPoints = hosts.Select<CacheHostConfigurationModel, EndPoint>(x => x.EndPoint).ToArray();
            string clusterPassword = hosts.First().Password;

            var multiplexerConfig = new ConfigurationOptions()
            {
                ClientName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "@" + Dns.GetHostName(),
                AllowAdmin = true,
                Password = clusterPassword,
                /*CommandMap = CommandMap.Create(new HashSet<string>
                { // EXCLUDE a few commands (security thing :) )
                    "INFO", "CONFIG", "CLUSTER",
                    "PING", "ECHO", "CLIENT"
                }, available: false),*/
            };
            ConnectionMultiplexer multiplexer = null;
            if (endPoints != null)
            {
                endPoints.ToList().ForEach(x => multiplexerConfig.EndPoints.Add(x));
                try
                {

                    multiplexer = ConnectionMultiplexer.Connect(multiplexerConfig);
                }
                catch (RedisConnectionException ex)
                {

                }
                /*
                 * Test @StackExchange.Redis
                 * 
                 */
            }
            return multiplexer;
        }
        public static IServiceCollection UseServerSideCache(this IServiceCollection builder, CacheConfigurationModel cacheConfigurationModel)
        {

            MemoryCacheOptions localCacheOptions = new MemoryCacheOptions { };
            var actionLocal1 = new Action<MemoryCacheOptions>((localCacheOptions) => { });

            builder.AddMemoryCache(actionLocal1);
            builder.AddOptions();
            var multiplexer = CreateConnectionMultiplexer(cacheConfigurationModel);
            if (multiplexer != null)
            {
                builder.AddSingleton<IConnectionMultiplexer>(multiplexer);
            }




            /*
             * Obsolte da StackExchange.Redis
             * 
             * builder.Add(ServiceDescriptor.Singleton<IDistributedMainCache, RedisMainCache>());
            builder.Add(ServiceDescriptor.Singleton<IDistributedFailoverCache, RedisFailoverCache>());*/
            builder.AddSingleton<ICachingHandler, CachingHandler>();
            return builder;
        }
    }
}
