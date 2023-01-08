using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.DependencyInjection;

using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
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
    public static class HealthChecksExtensions
    {

        public static IHealthChecksBuilder AddHealthChecks(this IHealthChecksBuilder healthChecksBuilder, List<HealthCheckDescriptor> healthCheckDescriptors)
        {
            foreach (var healthCheckDescriptor in healthCheckDescriptors)
            {
                var methodConvertedModel = typeof(HealthChecksBuilderAddCheckExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public).ToList().Find(x => x.Name == "AddTypeActivatedCheck" && x.GetParameters().Length == 4);
                var m = methodConvertedModel.MakeGenericMethod(healthCheckDescriptor.Type);
                healthChecksBuilder = (IHealthChecksBuilder)m.Invoke(healthChecksBuilder, new object[] { healthChecksBuilder, "healthcheck_" + healthCheckDescriptor.Name, healthCheckDescriptor.FailureStatus, healthCheckDescriptor.Args });


            }
            return healthChecksBuilder;
        }
    }
    public abstract class AbstractHealthCheck<T1, T2> : IHealthCheck
        where T2 : Task<HealthCheckResult>
    {


        public readonly Func<T1, T2> CheckHealthAction;
        private ReadOnlyDictionary<string, object> _properties;

        protected ReadOnlyDictionary<string, object> PropertiesViaCtor
        {
            get
            {
                return _properties;
            }
        }
        public AbstractHealthCheck(Func<T1, T2> checkHealthAction)
        {
            CheckHealthAction = checkHealthAction;
        }
        public AbstractHealthCheck(Func<T1, T2> checkHealthAction, Dictionary<string, object> data) : this(checkHealthAction)
        {
            _properties = new ReadOnlyDictionary<string, object>(data);
        }
        public abstract Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
        public ReadOnlyDictionary<string, object> GetInstanceProps(object instance)
        {
            var dict = new Dictionary<string, object>();
            List<PropertyInfo> properties = GetType().GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                var methodInfoSetter = property.GetSetMethod();
                bool isPublic = methodInfoSetter != null;
                if (!isPublic)
                    continue;
                string key = property.Name;
                object value = property.GetValue(instance);
                dict.Add(key, value);
            }
            var dictR = new ReadOnlyDictionary<string, object>(dict);
            return dictR;
        }
    }
    public class HealthCheckDescriptor
    {
        public string Name { get; set; }
        public HealthStatus FailureStatus { get; set; }
        public object[] Args { get; set; }
        public Type Type { get; set; }

        public HealthCheckDescriptor(Type type, string name, HealthStatus failureStatus, params object[] args)
        {
            Name = name;
            FailureStatus = failureStatus;
            Args = args;
            Type = type;
        }
    }

}
