using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using WebApiFunction.Data.Format.Json;

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

namespace WebApiFunction.Healthcheck
{
    public static class HealthCheckResponseWriter
    {

        public static async Task<Task> WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = GeneralDefs.ApiContentType;

            var options = new JsonWriterOptions
            {
                Indented = true
            };

            Process currentProcess = null;
            HealthCheckResponseObject healthCheckResponseObject = new HealthCheckResponseObject();
            ConcurrentDictionary<int, ProcessUsage> usages = new ConcurrentDictionary<int, ProcessUsage>();
            List<Task> getCpuUsageFromProcesses = new List<Task>();
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.Id == healthCheckResponseObject.ProcessId)
                {
                    currentProcess = proc;
                }
                var task = GetProcessUsage(proc, usages);
                getCpuUsageFromProcesses.Add(task);
            }
            Task.WaitAll(getCpuUsageFromProcesses.ToArray());
            var processUsage = await GetProcessUsage(currentProcess);
            healthCheckResponseObject.ProcessorUsageAll = usages.Values.Select(x => x.CpuUsage).Aggregate((current, usage) => current + usage);
            healthCheckResponseObject.RamSystem = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024.0 / 1024.0;
            healthCheckResponseObject.RamUsage = processUsage.RamUsage;
            healthCheckResponseObject.ProcessStartTime = currentProcess.StartTime;
            healthCheckResponseObject.ProcessorUsage = processUsage.CpuUsage;


            using (var stream = new MemoryStream())
            {
                using (JsonHandler jsonHandler = new JsonHandler())
                {
                    HealthCheckNodeResponseObject healthCheckNodeResponseObject = new HealthCheckNodeResponseObject();
                    foreach (var key in result.Entries.Keys)
                    {
                        HealthReportEntry value = result.Entries[key];
                        healthCheckResponseObject.HealthCheckChain.Add(key, new HealthCheckNodeResponseObject(value));
                    }
                    healthCheckResponseObject.RefreshStatistics();

                    string jsonStr = jsonHandler.JsonSerialize(healthCheckResponseObject);

                    byte[] data = Encoding.UTF8.GetBytes(jsonStr);
                    var json = Encoding.UTF8.GetString(data);
                    var response = context.Response.WriteAsync(json);
                    return response;
                }
            }
        }
        public class ProcessUsage
        {
            public double RamUsage { get; set; }
            public double CpuUsage { get; set; }

            public override string ToString()
            {
                return "CPU: " + CpuUsage + "%, RAM: " + RamUsage + "Mb";
            }
        }
        public static async Task<ProcessUsage> GetProcessUsage(Process proc, ConcurrentDictionary<int, ProcessUsage> usage = null)
        {
            ProcessUsage processUsage = new ProcessUsage();
            double cpuUsageTotal = 0;
            try
            {
                if (proc.Responding)
                {
                    var startTime = DateTime.UtcNow;
                    var startCpuUsage = proc.TotalProcessorTime;
                    await Task.Delay(500);

                    var endTime = DateTime.UtcNow;
                    var endCpuUsage = proc.TotalProcessorTime;
                    var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                    var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                    cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100.0;
                    processUsage.CpuUsage = cpuUsageTotal;
                    processUsage.RamUsage = proc.WorkingSet64 / 1024.0 / 1024.0;
                }
            }
            catch (Exception ex)
            {

            }

            if (usage != null)
                usage.TryAdd(proc.Id, processUsage);
            return processUsage;
        }
    }
}
