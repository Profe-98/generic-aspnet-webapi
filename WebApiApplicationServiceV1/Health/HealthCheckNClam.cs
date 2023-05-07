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
    public class HealthCheckNClam : AbstractHealthCheck<IScopedVulnerablityHandler,Task<HealthCheckResult>>
    {
        public static Func<IScopedVulnerablityHandler, Task<HealthCheckResult>> CheckNClamBackend = new Func<IScopedVulnerablityHandler, Task<HealthCheckResult>>(async(antivirusService) =>
        {
            HealthStatus healthStatus = HealthStatus.Unhealthy;
            string desciption = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            bool connectionResponse = await antivirusService.CheckConnection();
            stopwatch.Stop();
            healthStatus = connectionResponse ? HealthStatus.Healthy : HealthStatus.Unhealthy;
            desciption += "nclam-connection="+connectionResponse.ToString()+";whole-check-time="+ stopwatch .ElapsedMilliseconds+ "ms;";

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyName currentAssemblyName = currentAssembly.GetName();
            string currentWorkingDir = System.IO.Directory.GetParent(currentAssembly.Location).FullName;
            string libFileName = "nClam.dll";
            Version versionClient = AssemblyName.GetAssemblyName(System.IO.Path.Combine(currentWorkingDir, libFileName)).Version;

            desciption += "client-version="+versionClient.ToString()+";";
            return new HealthCheckResult(healthStatus, desciption);
        });


        private readonly IScopedVulnerablityHandler _vulnerablityHandler;
        public HealthCheckNClam(Func<IScopedVulnerablityHandler, Task<HealthCheckResult>> func, IScopedVulnerablityHandler vulnerablityHandler) : base(func)
        {
            _vulnerablityHandler = vulnerablityHandler;
        }
        public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken = default(CancellationToken))
        {
            return (Task<HealthCheckResult>)CheckHealthAction.DynamicInvoke(new object[] { _vulnerablityHandler });
        }
    }
}
