using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using WebApiApplicationService.Handler;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Middleware;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Linq;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq.Expressions;
using System.IO;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Microsoft.AspNetCore.Http;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
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
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
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
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Database.Dapper.Converter;
using Microsoft.AspNetCore.SignalR;
using WebApiFunction.Web.Websocket.SignalR.HubService;
using Microsoft.Identity.Client;
using WebApiApplicationService;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using InfluxDB.Client.Core.Exceptions;
using Microsoft.AspNetCore.Http.Connections;
using WebApiFunction.Application.Model.Database.MySQL.Dapper.TypeMapper;
using WebApiFunction.Application.Model.Database.MySQL.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Web.AspNet.Filter;
using WebApiFunction.Web.AspNet.Healthcheck;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiApplicationServiceV2
{
    public static class WebApiApplicationServiceExtension
    {
        private static bool IsDerivedFromType(Type derivedType, Type baseType)
        {
            if (derivedType == baseType)
                return true;

            Type currentBaseType = derivedType.BaseType;
            while (currentBaseType != null)
            {
                if (currentBaseType == baseType)
                    return true;

                currentBaseType = currentBaseType.BaseType;
            }

            return false;
        }
        private static readonly string AllowOrigin = "api-gateway";
        public static IServiceCollection AddControllerModules(this IServiceCollection services,
            ICachingHandler cachingHandler, 
            ISingletonDatabaseHandler databaseHandler,
            AbstractDapperContext mysqlDapperContext)
        {
            Type builderType = typeof(ServiceCollectionServiceExtensions);
            if (builderType == null)
                throw new NotImplementedException();
            var targetAssembly = Assembly.GetAssembly(typeof(AbstractModel));
            var allModelsFromRelfection = targetAssembly.GetTypes();

            if(allModelsFromRelfection != null)
            {
                var filterForDataModels = allModelsFromRelfection.ToList().FindAll(x => (x.BaseType == typeof(AbstractModel) || IsDerivedFromType(x,typeof(AbstractModel))) && x.Name!=nameof(AbstractModel));
                
                if (filterForDataModels != null)
                {
                    var backendModuleAbstractType = typeof(AbstractBackendModule<>);
                    foreach (var item in filterForDataModels)
                    {
                        var abstractBackendModuleType = typeof(AbstractBackendModule<>).MakeGenericType(item);
                        var foundConcreteImplementation = allModelsFromRelfection.ToList().Find(x => (IsDerivedFromType(x, abstractBackendModuleType)));


                        var backendModuleType = foundConcreteImplementation == null?backendModuleAbstractType.MakeGenericType(item): foundConcreteImplementation;
                        var backendModuleInterfaceType = typeof(IAbstractBackendModule<>).MakeGenericType(item);
                        var backendModuleInstance = Activator.CreateInstance(backendModuleType,databaseHandler,cachingHandler, mysqlDapperContext);



                        var methodParams = new List<Type> { typeof(IServiceCollection) };
                        
                        var f = builderType.GetMethod("AddSingleton",2,methodParams.ToArray());
                        MethodInfo method = f;
                        if (method == null)
                            throw new NotFoundException("IServiceCollection.AddSingleton<T,T2> not found");
                        method = method.MakeGenericMethod(backendModuleInterfaceType, backendModuleType);
                        var callParams = new List<object> { services } ;
                        method.Invoke(services, callParams.ToArray());
                    }
                }
            }
            return services;
        }
        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration,string[] databaseEntityNamespace)
        {
            services.AddSingleton<IConfiguration>(configuration);
            services.AddCors(options => {
                options.AddPolicy(AllowOrigin, builder => {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.SetIsOriginAllowed(origin => {
                        if (origin != null)
                        {
                            var uri = new Uri(origin);
                            if (uri.Port == 5000)
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                });
            });
            // In production, the React files will be served from this directory
            /*services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });*/
            services.AddHttpContextAccessor();


            var jsonSerOpt = JsonHandler.Settings;
            jsonSerOpt.Converters.Add(new WebApiFunction.Converter.JsonConverter.JsonBoolConverter());
            var jsonHandler = new JsonHandler(jsonSerializerOptions: jsonSerOpt);
            services.AddSingleton<ISingletonJsonHandler>(x => jsonHandler);
            services.AddScoped<IScopedJsonHandler>(x => jsonHandler);

            services.AddSingleton<IAppconfig, AppConfig>();
            services.AddSingleton<ITaskSchedulerBackgroundServiceQueuer, TaskSchedulerBackgroundServiceQueuer>();
            services.AddScoped<IHttpContextHandler, HttpContextHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var appConfigService = serviceProvider.GetService<IAppconfig>();
            var cacheConfig = appConfigService.AppServiceConfiguration.CacheConfigurationModel;
            var fileHandler = new FileHandler(appConfigService);
            services.UseServerSideCache(cacheConfig);
            services.AddSingleton<IFileHandler, FileHandler>();
            services.AddSingleton<ISingletonVulnerablityHandler>(x => new VulnerablityHandler(appConfigService.AppServiceConfiguration.AppPaths[AppConfigDefinitionProperties.PathDictKeys.File.NClamQuarantinePath], appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Host, appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Port, true));
            services.AddScoped<IScopedVulnerablityHandler>(x => new VulnerablityHandler(appConfigService.AppServiceConfiguration.AppPaths[AppConfigDefinitionProperties.PathDictKeys.File.NClamQuarantinePath], appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Host, appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Port, true));
            services.AddSingleton<IMailHandler, MailHandler>();
            services.AddScoped<IJWTHandler, JWTHandler>();
            DataMapperForDapperExtension.UseCustomDataMapperForDapper(databaseEntityNamespace);
            DapperTypeConverterHandler.UseStringToGuidConversion();
            services.AddSingleton<IMysqlDapperContext,AbstractDapperContext>();

            services.AddTransient<ITransientDatabaseHandler, MySqlDatabaseHandler>();
            services.AddScoped<IScopedDatabaseHandler, MySqlDatabaseHandler>();
            if (appConfigService.AppServiceConfiguration.NodeManagerDatabaseConfigurationModel != null)
            {
                services.AddSingleton<ISingletonNodeDatabaseHandler>(new MySqlDatabaseHandler(appConfigService.AppServiceConfiguration.NodeManagerDatabaseConfigurationModel.MysqlConnectionString, appConfigService.AppServiceConfiguration.NodeManagerDatabaseConfigurationModel.AutoCommit));

            }
            services.AddSingleton<ISingletonDatabaseHandler>(new MySqlDatabaseHandler(appConfigService.AppServiceConfiguration.DatabaseConfigurationModel.MysqlConnectionString, appConfigService.AppServiceConfiguration.DatabaseConfigurationModel.AutoCommit));

            services.AddSingleton<INodeManagerHandler, NodeManagerHandler>();
            services.AddScoped<IScopedSiteProtectHandler, SiteProtectHandler>();
            services.AddSingleton<ISingletonSiteProtectHandler, SiteProtectHandler>();
            services.AddScoped<IAuthHandler, AuthHandler>();//dependent on IHttpContextHandler & IScopedDatabaseHandler this is why these both are instancing first by a request
            services.AddScoped<IJsonApiDataHandler, JsonApiDataHandler>();
            services.AddTransient<IClientErrorFactory, ClientErrorHandler>();
            services.AddSingleton<ISingletonEncryptionHandler, EncryptionHandler>();
            services.AddScoped<IScopedEncryptionHandler, EncryptionHandler>();

            serviceProvider = services.BuildServiceProvider();

            ISingletonNodeDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
            CustomControllerBaseExtensions.RegisterNetClasses(databaseHandler, databaseEntityNamespace);
            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            nodeManager.Register();
            services.AddRabbitMq();

            serviceProvider = services.BuildServiceProvider();
            var appConfig = serviceProvider.GetService<IAppconfig>();
            var databaseSingletonService = serviceProvider.GetService<ISingletonDatabaseHandler>();
            var databaseMySqlDapperService = serviceProvider.GetService<AbstractDapperContext>();
            var cacheService = serviceProvider.GetService<ICachingHandler>();
            var avService = serviceProvider.GetService<IScopedVulnerablityHandler>();
            var rabbitMqService = serviceProvider.GetService<IRabbitMqHandler>();


            services.AddHealthChecks()
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(HealthCheckMySql),"mysql", HealthStatus.Unhealthy, new object[]
                    {
                        HealthCheckMySql.CheckMySqlBackend, databaseSingletonService
                    })
                })
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(HealthCheckRabbitMq),"rabbitmq", HealthStatus.Unhealthy, new object[]
                    {
                        HealthCheckRabbitMq.CheckRabbitMqBackend, rabbitMqService
                    })
                })
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(HealthCheckCache),"cache", HealthStatus.Unhealthy, new object[]
                    {
                        HealthCheckCache.CheckCacheBackend, cacheService
                    })
                })
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(HealthCheckNClam),"antivirus", HealthStatus.Unhealthy, new object[]
                    {
                        HealthCheckNClam.CheckNClamBackend, avService
                    })
                });

            serviceProvider = services.BuildServiceProvider();
            var test = serviceProvider.GetService<HealthCheckService>();
            /*services.AddMvc(options => 
            {
                options.InputFormatters.Insert(0, new RequestInputFormatter());
            }).;*/


            services.AddControllerModules(cacheService, databaseSingletonService, databaseMySqlDapperService);

            services.AddControllers(options =>
            {
                int minVal = int.MinValue;
                options.Filters.Add(typeof(HttpFlowFilter), minVal);//wird immer als erster Filter(Global-Filter, pre-executed vor allen Controllern) ausgeführt, danach kommt wenn als Attribute an Controller-Action 'CustomConsumesFilter' mit Order=int.MinValue+1
                options.Filters.Add(typeof(ValidateModelFilter), minVal + 1);
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            //services.UseCustomModelStateResponseFactory();//wird nicht von der service collection berücksichtigt ????
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressMapClientErrors = false;
                options.SuppressModelStateInvalidFilter = true;
                /*
                 * Wird durch Filter in den Controller geregelt, sprich Content-Type Validierung
                 * options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(context.ModelState);

                    result.ContentTypes.Add(GeneralDefs.ApiContentType);

                    return result;
                };*/
            });
            services.UseAsyncTaskScheduler();
            if (appConfig.AppServiceConfiguration.SignalRHubConfigurationModel != null && appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.UseLocalHub)
            {

                services.AddSignalR(x => {

                    x.KeepAliveInterval = TimeSpan.FromSeconds(appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.KeepaliveTimeout);
                    x.ClientTimeoutInterval = TimeSpan.FromSeconds(appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.ClientTimeoutSec);
                    x.EnableDetailedErrors = appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.DebugErrorsDetailedClientside;
                    x.MaximumParallelInvocationsPerClient = appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.MaximumParallelInvocationsPerClient;
                    x.HandshakeTimeout = TimeSpan.FromSeconds(appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.HandshakeTimeout);

                }).AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });
            }
            return services;    
        }
    }
}
