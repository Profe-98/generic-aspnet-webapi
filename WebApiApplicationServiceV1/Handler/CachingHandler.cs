using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Data;
using System.Net;
using WebApiApplicationService.Models;
using WebApiApplicationService.Handler;
using WebApiApplicationService.InternalModels;
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
using WebApiApplicationService.Models.InternalModels;
using WebApiApplicationService.Models.Database;
using System.Diagnostics;
using StackExchange.Redis.Profiling;
using StackExchange.Redis;

namespace WebApiApplicationService.Handler
{
    public class CachingHandler : ICachingHandler, IDisposable
    {
        #region Private
        private readonly bool _useLocalCache = true;
        private readonly StackExchange.Redis.IConnectionMultiplexer _connectionMultiplexer;
        private readonly IMemoryCache _localCache;
        /*private readonly IDistributedMainCache _mainCache;
        private readonly IDistributedFailoverCache _failoverCache;*/
        private readonly ISingletonJsonHandler _jsonHandler;
        private readonly IAppconfig _appconfig;
        private readonly IServiceProvider _serviceProvider;

        #endregion
        #region Public
        #endregion
        #region Ctor & Dtor
        public CachingHandler(IServiceProvider serviceProvider,StackExchange.Redis.IConnectionMultiplexer connectionMultiplexer,IAppconfig appconfig, ISingletonJsonHandler jsonHandler, IMemoryCache localCache/*, IDistributedMainCache mainDistributedCache,IDistributedFailoverCache failoverDistributedCache*/)
        {
            _localCache = localCache;
            /*_mainCache = mainDistributedCache;
            _failoverCache = failoverDistributedCache;*/
            _jsonHandler = jsonHandler;
            _connectionMultiplexer = connectionMultiplexer;
            _useLocalCache = true;
            _appconfig = appconfig;
            _serviceProvider  = serviceProvider;

        }
        public CachingHandler(IServiceProvider serviceProvider, ISingletonJsonHandler jsonHandler, IAppconfig appconfig, IMemoryCache localCache/*, IDistributedMainCache mainDistributedCache,IDistributedFailoverCache failoverDistributedCache*/)
        {
            _localCache = localCache;
            /*_mainCache = mainDistributedCache;
            _failoverCache = failoverDistributedCache;*/
            _jsonHandler = jsonHandler;
            _useLocalCache = true;
            _appconfig = appconfig;
            _serviceProvider = serviceProvider;

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
                if(_connectionMultiplexer != null )
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

                    bool conn = (_connectionMultiplexer.GetServer(endPoints[i])).IsConnected;
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

        public void ReInitConnectionMultiplexer()
        {
            var serviceProvider = _serviceProvider.GetService<IConnectionMultiplexer>();
            if(serviceProvider == null)
            {

                var cacheConfig = _appconfig.AppServiceConfiguration.CacheConfigurationModel;
                var serviceCollection = _serviceProvider.GetService<IServiceCollection>();
                
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
                        if(resultFromMain != null)
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
                if ( _useLocalCache)
                    _localCache.Set(key, value,new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow });

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
            foreach(IServer server in Nodes)
            {
                IAsyncEnumerable<RedisKey> keys = server.KeysAsync(pattern: filter,flags: CommandFlags.PreferMaster);
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
        public static IServiceCollection UseServerSideCache(this IServiceCollection builder, CacheConfigurationModel cacheConfigurationModel)
        {

            /*builder.AddStackExchangeRedisCache(x =>
                {
                    x.Configuration = $"{server.Address}:{server.Port}"+ password == null?"": (",password="+ password + "") + "";
                });*/
            //builder.AddStackExchangeRedisCache(x => x = mainDistributedCacheOptions);
            MemoryCacheOptions localCacheOptions= new MemoryCacheOptions { };
            var hosts = cacheConfigurationModel?.Hosts?.ToList();
            EndPoint[] endPoints = hosts.Select<CacheHostConfigurationModel, EndPoint>(x => x.EndPoint).ToArray();
            string clusterPassword = hosts.First().Password;
            builder.AddOptions();

            var actionLocal1 = new Action<MemoryCacheOptions>((localCacheOptions) => { });

            builder.AddMemoryCache(actionLocal1);

            var multiplexerConfig = new StackExchange.Redis.ConfigurationOptions() 
            { 
                ClientName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name+"@"+Dns.GetHostName(),
                AllowAdmin = true,
                Password = clusterPassword,
                /*CommandMap = CommandMap.Create(new HashSet<string>
                { // EXCLUDE a few commands (security thing :) )
                    "INFO", "CONFIG", "CLUSTER",
                    "PING", "ECHO", "CLIENT"
                }, available: false),*/
            };
            if(endPoints != null)
            {
                endPoints.ToList().ForEach(x => multiplexerConfig.EndPoints.Add(x));
                ConnectionMultiplexer multiplexer = null;
                try
                {

                    multiplexer = ConnectionMultiplexer.Connect(multiplexerConfig);
                }
                catch (StackExchange.Redis.RedisConnectionException ex)
                {

                }
                /*
                 * Test @StackExchange.Redis
                 * 
                 */
                if (multiplexer != null)
                {
                    var connState = multiplexer.GetStatus();
                    foreach (EndPoint endpoint in multiplexer.GetEndPoints())
                    {
                        var server = multiplexer.GetServer(endpoint);

                        try
                        {
                            var clients = server.ClientList(CommandFlags.None);
                            var keys = server.Keys().ToList();
                            var memoryStats = server.MemoryStats();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString() + ", Solution for no route to private net (class c) is to add route on system");
                        }
                    }
                    var db = multiplexer.GetDatabase();
                    db.StringSet(new StackExchange.Redis.RedisKey("test2"), new StackExchange.Redis.RedisValue("testbymika"), expiry: new TimeSpan(0, 10, 0), flags: StackExchange.Redis.CommandFlags.None);

                    var get = db.StringGet(new StackExchange.Redis.RedisKey("test2"));

                    /**/
                    builder.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(multiplexer);
                }
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
