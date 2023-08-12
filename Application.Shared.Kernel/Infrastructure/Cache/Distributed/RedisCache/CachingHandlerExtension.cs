using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache
{
    public static class CachingHandlerExtensions
    {
        public static IConnectionMultiplexer CreateConnectionMultiplexer(CacheConfigurationModel cacheConfigurationModel)
        {
            if (cacheConfigurationModel == null)
                return null;
            try
            {
                var hosts = cacheConfigurationModel?.Hosts?.ToList();
                EndPoint[] endPoints = hosts.Select(x => x.EndPoint).ToArray();
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
            catch (Exception ex)
            {

            }
            return null;

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
                builder.AddSingleton(multiplexer);
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
