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
using WebApiFunction.Database;

namespace WebApiFunction.Web.AspNet.Healthcheck
{
    public class HealthCheckMySql : AbstractHealthCheck<IScopedDatabaseHandler, Task<HealthCheckResult>>
    {
        public static Func<IScopedDatabaseHandler, Task<HealthCheckResult>> CheckMySqlBackend = new Func<IScopedDatabaseHandler, Task<HealthCheckResult>>((databaseService) =>
        {
            HealthStatus healthStatus = HealthStatus.Unhealthy;
            string desciption = null;
            try
            {
                QueryResponseData data = databaseService.ExecuteQuerySync("SELECT 1;");
                if (data != null)
                {
                    if (data.FetchTime > 1000)
                    {
                        healthStatus = HealthStatus.Degraded;
                        desciption = "Up-state=Up;SELECT 1=1;fetch-time=" + data.FetchTime + " (too long);errors=no;warning=yes;details=;";
                    }
                    else if (data.HasData)
                    {
                        bool convertedResponse = int.TryParse(data.Data.Rows[0][0].ToString(), out int number);
                        if (number == 1 && convertedResponse)
                        {
                            healthStatus = HealthStatus.Healthy;
                            desciption = "Up-state=Up;SELECT 1=1;fetch-time=" + data.FetchTime + ";errors=no;warning=no;details=;";
                        }
                        else
                        {
                            desciption = "Up-state=Up;SELECT 1=?;fetch-time=" + data.FetchTime + ";errors=yes;warning=yes;details=convertion error or not equal than 1;";
                        }
                    }
                    else if (!data.HasData || data.HasErrors)
                    {
                        desciption = "Up-state=Up;SELECT 1=?;fetch-time=" + data.FetchTime + ";errors=yes;warning=no;details=;";
                    }
                }
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                AssemblyName currentAssemblyName = currentAssembly.GetName();
                string currentWorkingDir = Directory.GetParent(currentAssembly.Location).FullName;
                string libFileName = "MySql.Data.dll";
                Version versionClient = AssemblyName.GetAssemblyName(Path.Combine(currentWorkingDir, libFileName)).Version;

                QueryResponseData dataVersion = databaseService.ExecuteQuerySync("SELECT @@version;");
                if (dataVersion.HasData)
                {
                    string versionServer = dataVersion.Data.Rows[0][0].ToString();
                    bool versionCompare = versionClient.ToString().ToLower() != versionServer.ToLower();
                    desciption += "server-version=" + versionServer + ";client-version=" + versionClient.ToString() + ";version-compare=" + (versionCompare ? "mismatch (incombabilities may there)" : "match") + ";";
                    healthStatus = versionCompare ? HealthStatus.Healthy : HealthStatus.Degraded;
                }
                else
                {
                    desciption += "server-version=n.a.;client-version=" + versionClient.ToString() + ";version-compare=mismatch (incombabilities may there);";
                    healthStatus = HealthStatus.Degraded;
                }
            }
            catch (MySqlException ex)
            {
                desciption = "Up-state=Down;SELECT 1=?;fetch-time=?;errors=yes;warning=no;details=" + ex.Message + ";";
                healthStatus = HealthStatus.Unhealthy;
            }
            return Task.FromResult(new HealthCheckResult(healthStatus, desciption));
        });

        private readonly IScopedDatabaseHandler _databaseHandler;
        public HealthCheckMySql(Func<IScopedDatabaseHandler, Task<HealthCheckResult>> func, IScopedDatabaseHandler databaseHandler) : base(func)
        {
            _databaseHandler = databaseHandler;
        }
        public override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return (Task<HealthCheckResult>)CheckHealthAction.DynamicInvoke(new object[] { _databaseHandler });
        }
    }
}
