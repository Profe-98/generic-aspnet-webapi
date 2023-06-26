using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System;



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
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
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

namespace WebApiFunction.Web.AspNet.Healthcheck
{
    public class HealthCheckCache : AbstractHealthCheck<ICachingHandler, Task<HealthCheckResult>>
    {
        public static Func<ICachingHandler, Task<HealthCheckResult>> CheckCacheBackend = new Func<ICachingHandler, Task<HealthCheckResult>>(async (cacheService) =>
        {
            HealthStatus healthStatus = HealthStatus.Unhealthy;
            string desciption = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var response = await cacheService.Ping();
            var responseLocal = cacheService.PingLocalCache();
            stopwatch.Stop();
            HealthStatus[] cacheHealthStatus = new HealthStatus[response.Keys.Count];//+1 wegen localcache der immer existiert
            HealthStatus cacheHealthStatusLocalCache = HealthStatus.Unhealthy;
            if (responseLocal != GeneralDefs.NotFoundResponseValue)
            {
                cacheHealthStatusLocalCache = responseLocal < 5 ? HealthStatus.Healthy : HealthStatus.Degraded;
            }
            desciption += "local-cache=;Up-state=Up;GET=\"\";fetch-time=0ms;errors=no;warning=no;details=;\n";
            int i = 0;
            foreach (var key in response.Keys)//index start by 1 wegen localcache
            {
                var data = response[key];
                if (data == GeneralDefs.NotFoundResponseValue)
                {
                    desciption += "distributed-cache=" + key.ToString() + ";Up-state=Down;GET=\"\";fetch-time=?;errors=yes;warning=no;details=host is not reachable;\n";
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
            if (healthStatus == HealthStatus.Unhealthy && cacheHealthStatusLocalCache == HealthStatus.Healthy)
                healthStatus = HealthStatus.Degraded;

            desciption += "whole-check-time=" + stopwatch.ElapsedMilliseconds + "ms;";

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyName currentAssemblyName = currentAssembly.GetName();
            string currentWorkingDir = Directory.GetParent(currentAssembly.Location).FullName;
            string libFileName = "Microsoft.Extensions.Caching.StackExchangeRedis.dll";
            Version versionClient = AssemblyName.GetAssemblyName(Path.Combine(currentWorkingDir, libFileName)).Version;

            desciption += "client-version=" + versionClient.ToString() + ";";
            return new HealthCheckResult(healthStatus, desciption);
        });


        private readonly ICachingHandler _cachingHandler;
        public HealthCheckCache(Func<ICachingHandler, Task<HealthCheckResult>> func, ICachingHandler cachingHandler) : base(func)
        {
            _cachingHandler = cachingHandler;
        }
        public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return (Task<HealthCheckResult>)CheckHealthAction.DynamicInvoke(new object[] { _cachingHandler });
        }
    }
}
