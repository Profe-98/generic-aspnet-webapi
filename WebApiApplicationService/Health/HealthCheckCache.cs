using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using MySql.Data.MySqlClient;
using WebApiApplicationService.InternalModels;
using System.Diagnostics;
using System;

namespace WebApiApplicationService.Health
{
    public class HealthCheckCache : AbstractHealthCheck<ICachingHandler,Task<HealthCheckResult>>
    {
        public static Func<ICachingHandler, Task<HealthCheckResult>> CheckCacheBackend = new Func<ICachingHandler, Task<HealthCheckResult>>(async(cacheService) =>
        {
            HealthStatus healthStatus = HealthStatus.Unhealthy;
            string desciption = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var response = await cacheService.Ping();
            var responseLocal = cacheService.PingLocalCache();
            stopwatch.Stop();
            HealthStatus[] cacheHealthStatus = new HealthStatus[response.Keys.Count];//+1 wegen localcache der immer existiert
            HealthStatus cacheHealthStatusLocalCache = HealthStatus.Unhealthy;
            if(responseLocal != GeneralDefs.NotFoundResponseValue)
            {
                cacheHealthStatusLocalCache = responseLocal<5?HealthStatus.Healthy: HealthStatus.Degraded;
            }
            desciption += "local-cache=;Up-state=Up;GET=\"\";fetch-time=0ms;errors=no;warning=no;details=;\n";
            int i = 0;
            foreach(var key in response.Keys)//index start by 1 wegen localcache
            {
                var data = response[key];
                if(data == GeneralDefs.NotFoundResponseValue)
                {
                    desciption += "distributed-cache="+key.ToString()+";Up-state=Down;GET=\"\";fetch-time=?;errors=yes;warning=no;details=host is not reachable;\n";
                    cacheHealthStatus[i] = HealthStatus.Unhealthy;
                }
                else
                {
                    desciption += "distributed-cache=" + key.ToString() + ";Up-state=Up;GET=\"\";fetch-time=" + data + "ms;errors=no;warning=no;details=;\n";
                    cacheHealthStatus[i] = data < 10 ? HealthStatus.Healthy : HealthStatus.Degraded;
                }
                i++;
            }
            healthStatus = HealthStatus.Unhealthy;
            int countHealthy = cacheHealthStatus.ToList().FindAll(x => x.HasFlag(HealthStatus.Healthy)).Count;
            int countUnHealthy = cacheHealthStatus.ToList().FindAll(x => x.HasFlag(HealthStatus.Unhealthy)).Count;
            int countDegraded = cacheHealthStatus.ToList().FindAll(x => x.HasFlag(HealthStatus.Degraded)).Count;
            int ratioUnHealthy = 100 / cacheHealthStatus.Length * countUnHealthy;
            int ratioDegraded = 100 / cacheHealthStatus.Length * countDegraded;
            if (ratioUnHealthy > 75)
            {
                healthStatus = HealthStatus.Unhealthy;
            }
            else
            {
                healthStatus = HealthStatus.Healthy;
            }
            if(healthStatus == HealthStatus.Unhealthy && cacheHealthStatusLocalCache == HealthStatus.Healthy)
                healthStatus = HealthStatus.Degraded;

            desciption += "whole-check-time="+ stopwatch .ElapsedMilliseconds+ "ms;";

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyName currentAssemblyName = currentAssembly.GetName();
            string currentWorkingDir = System.IO.Directory.GetParent(currentAssembly.Location).FullName;
            string libFileName = "Microsoft.Extensions.Caching.StackExchangeRedis.dll";
            Version versionClient = AssemblyName.GetAssemblyName(System.IO.Path.Combine(currentWorkingDir, libFileName)).Version;

            desciption += "client-version="+versionClient.ToString()+";";
            return new HealthCheckResult(healthStatus, desciption);
        });


        private readonly ICachingHandler _cachingHandler;
        public HealthCheckCache(Func<ICachingHandler, Task<HealthCheckResult>> func, ICachingHandler cachingHandler) : base(func)
        {
            _cachingHandler = cachingHandler;
        }
        public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken = default(CancellationToken))
        {
            return (Task<HealthCheckResult>)CheckHealthAction.DynamicInvoke(new object[] { _cachingHandler });
        }
    }
}
