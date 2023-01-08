﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
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
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Logging;

namespace WebApiApplicationService.Health
{
    public class HealthCheckRabbitMq : AbstractHealthCheck<IRabbitMqHandler,Task<HealthCheckResult>>
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
                    using(IConnection connection = rabbitMqService.GetConnection())
                    {
                        foreach(var item in connection.ServerProperties)
                        {
                            if(item.Value is byte[])
                            {
                                var dataStr = System.Text.Encoding.UTF8.GetString((byte[])item.Value);
                                serverProperties += item.Key + "=" + dataStr + ";";
                                if(item.Key =="platform")
                                {
                                    serverPlatform = dataStr;
                                }
                                else if (item.Key == "version")
                                {
                                    var splitStr = dataStr.Split(new string[] {"." },StringSplitOptions.None);
                                    int[] versionParts = new int[3] {0,0,0 };
                                    for (int i = 0; i < splitStr.Length; i++)
                                    {
                                        if(i <= versionParts.Length)
                                        {
                                            if (int.TryParse(splitStr[i],out int versionPartInt))
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
                catch(Exception ex)
                {

                }
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                AssemblyName currentAssemblyName = currentAssembly.GetName();
                string currentWorkingDir = System.IO.Directory.GetParent(currentAssembly.Location).FullName;
                string libFileName = "RabbitMQ.Client.dll";
                Version versionClient = AssemblyName.GetAssemblyName(System.IO.Path.Combine(currentWorkingDir, libFileName)).Version;
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
        public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken = default(CancellationToken))
        {
            return (Task<HealthCheckResult>)CheckHealthAction.DynamicInvoke(new object[] { _rabbitMqHandler });
        }
    }
}
