using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Application.Shared.Kernel.Application.Controller.Modules;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Security.Encryption;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.Http;
using Application.Shared.Kernel.Application;
using Application.Shared.Kernel.Web.Authentification.JWT;
using InfluxDB.Client.Core.Exceptions;
using Application.Shared.Kernel.Web.AspNet.Filter;
using Application.Shared.Kernel.Web.AspNet.Healthcheck;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Application.Model.Dapper.TypeMapper;
using Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context;
using Application.Shared.Kernel.Infrastructure.Mail;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Infrastructure.Antivirus.nClam;
using Application.Shared.Kernel.Infrastructure.Database.Dapper.Converter;
using Application.Shared.Kernel.Infrastructure.Database.Mysql;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Infrastructure.LocalSystem.IO.File;
using Application.Shared.Kernel.Application.Model.Database.MySQL;

namespace Application.Shared.Web.Api.Shared
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
                var filterForDataModels = allModelsFromRelfection.ToList().FindAll(x => (x.BaseType == typeof(AbstractModel) || IsDerivedFromType(x,typeof(AbstractModel))) && x.Name!=nameof(AbstractModel) && !x.Namespace.StartsWith("Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway"));
                
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
            jsonSerOpt.Converters.Add(new Application.Shared.Kernel.Data.Format.Converter.JsonConverter.JsonBoolConverter());
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
            services.AddScoped<IAuthHandler, AuthHandler>();//dependent on IHttpContextHandler & IScopedDatabaseHandler this is why these both are instancing first by a request
            services.AddScoped<IJsonApiDataHandler, JsonApiDataHandler>();
            services.AddTransient<IClientErrorFactory, ClientErrorHandler>();
            services.AddSingleton<ISingletonEncryptionHandler, EncryptionHandler>();
            services.AddScoped<IScopedEncryptionHandler, EncryptionHandler>();

            serviceProvider = services.BuildServiceProvider();

            ISingletonNodeDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
            CustomControllerBaseExtensions.RegisterNetClasses(databaseHandler, databaseEntityNamespace);
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
            var hCS = serviceProvider.GetService<HealthCheckService>();
            var resultHealthCheck = async() => {
                var result = await hCS.CheckHealthAsync();
                foreach(var item in result.Entries)
                {
                    Console.WriteLine("[HealthCheckService]: "+item.Key+": "+item.Value.Description+"");
                }
            };
            resultHealthCheck.Invoke();

            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            nodeManager.Register();

            /*services.AddMvc(options => 
            {
                options.InputFormatters.Insert(0, new RequestInputFormatter());
            }).;*/


            services.AddControllerModules(cacheService, databaseSingletonService, databaseMySqlDapperService);

            services.AddControllers(options =>
            {
                int minVal = int.MinValue;
                options.Filters.Add(typeof(HttpFlowFilter), minVal);//wird immer als erster Filter(Global-Filter, pre-executed vor allen Controllern) ausgeführt, danach kommt wenn als Attribute an Controller-Action 'CustomConsumesFilter' mit Order=int.MinValue+1
                options.Filters.Add(typeof(ContextualResponseSerializerFilter),minVal+1);
                options.Filters.Add(typeof(ValidateModelFilter), minVal + 2);
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
            databaseSingletonService.ExecuteQuery("UPDATE user SET signalr_connection_id = null WHERE signalr_connection_id is not null;");
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
