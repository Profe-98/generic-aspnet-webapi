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
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Logging;



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
    public class HealthCheckRabbitMq : AbstractHealthCheck<IRabbitMqHandler, Task<HealthCheckResult>>
    {
        public static Func<IRabbitMqHandler, Task<HealthCheckResult>> CheckRabbitMqBackend = new Func<IRabbitMqHandler, Task<HealthCheckResult>>((rabbitMqService) =>
        {
            HealthStatus healthStatus = HealthStatus.Unhealthy;
            string desciption = null;
            string serverProperties = null;
            Version serverVersion = null;
            string serverPlatform = null;
            try
            {
                try
                {
                    using (IConnection connection = rabbitMqService.GetConnection())
                    {
                        foreach (var item in connection.ServerProperties)
                        {
                            if (item.Value is byte[])
                            {
                                var dataStr = System.Text.Encoding.UTF8.GetString((byte[])item.Value);
                                serverProperties += item.Key + "=" + dataStr + ";";
                                if (item.Key == "platform")
                                {
                                    serverPlatform = dataStr;
                                }
                                else if (item.Key == "version")
                                {
                                    var splitStr = dataStr.Split(new string[] { "." }, StringSplitOptions.None);
                                    int[] versionParts = new int[3] { 0, 0, 0 };
                                    for (int i = 0; i < splitStr.Length; i++)
                                    {
                                        if (i <= versionParts.Length)
                                        {
                                            if (int.TryParse(splitStr[i], out int versionPartInt))
                                            {
                                                versionParts[i] = versionPartInt;
                                            }
                                        }
                                    }
                                    serverVersion = new Version(versionParts[0], versionParts[1], versionParts[2]);
                                }
                            }
                        }
                        desciption += serverProperties;
                        healthStatus = connection.IsOpen ? HealthStatus.Healthy : HealthStatus.Unhealthy;
                    }

                }
                catch (Exception ex)
                {

                }
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                AssemblyName currentAssemblyName = currentAssembly.GetName();
                string currentWorkingDir = Directory.GetParent(currentAssembly.Location).FullName;
                string libFileName = "RabbitMQ.Client.dll";
                Version versionClient = AssemblyName.GetAssemblyName(Path.Combine(currentWorkingDir, libFileName)).Version;
                desciption += "client-version=" + versionClient.ToString() + "";


            }
            catch (Exception ex)
            {
                healthStatus = HealthStatus.Unhealthy;
            }

            desciption += ";rabbitmq=" + (healthStatus == HealthStatus.Healthy ? "up" : "down") + "";
            return Task.FromResult(new HealthCheckResult(healthStatus, desciption));
        });

        private readonly IRabbitMqHandler _rabbitMqHandler;
        public HealthCheckRabbitMq(Func<IRabbitMqHandler, Task<HealthCheckResult>> func, IRabbitMqHandler rabbitMqHandler) : base(func)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }
        public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return (Task<HealthCheckResult>)CheckHealthAction.DynamicInvoke(new object[] { _rabbitMqHandler });
        }
    }
}
