using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using WebApiApplicationService;
using WebApiApplicationService.Controllers.APIv1;
using WebApiApplicationService.Handler;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySql.Dapper.TypeMapper;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Configuration;
using WebApiFunction.Controller;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Database;
using WebApiFunction.Database.Dapper.Converter;
using WebApiFunction.Database.MySQL;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Mail;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading.Service;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using Microsoft.Extensions.Configuration;
using WebApiFunction.Log;
using System;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Reflection;
using InfluxDB.Client.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using CsvHelper;
using Org.BouncyCastle.Ocsp;
using System.Text;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using System.Runtime.CompilerServices;

namespace WebApiApplicationServiceV2_Test.Tests.IntegrationTests
{
    public abstract class CustomApiControllerBaseTest<T1, T2>
        : IClassFixture<WebApplicationFactory<Startup>>
        where T1 : AbstractModel
        where T2 : CustomApiControllerBase<T1, AbstractBackendModule<T1>>
    {
        public static Dictionary<string, T2> ControllerInstaces = new Dictionary<string, T2>();
        public static bool InitialSetUp = false;
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Startup> _factory;
        public static IContainer Container;
        protected T2 _controller;
        public CustomApiControllerBaseTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = CreateHttpClient(WEBAPI_VERSION.V2);
            var controllerType = typeof(T2);
            var ctorInfo = controllerType.GetConstructors();

            if (!InitialSetUp)
                SetUp();


            var scopedDatabaseHandler = Container.Resolve<IScopedDatabaseHandler>();
            var dapperCtx = Container.Resolve<MysqlDapperContext>();
            var cachingHandler = Container.Resolve<ICachingHandler>();


            var tt = scopedDatabaseHandler.ExecuteQuerySync("SHOW TABLES;");

            string key = typeof(T2).FullName;
            if (!ControllerInstaces.ContainsKey(key))
            {
                var t = typeof(T2);
                var ctor = t.GetConstructors().ToList().Find(x => x.GetParameters().ToList().Find(y => y.ParameterType != typeof(IActionDescriptorCollectionProvider)) != null);
                var ctorP = ctor.GetParameters();
                int indexer = 0;
                object[] initFilledCtor = new object[ctorP.Length];
                foreach (var p in ctorP)
                {
                    object service = null;
                    if (Container.IsRegistered(p.ParameterType))
                    {
                        service = Container.Resolve(p.ParameterType);

                    }
                    else
                    {

                    }
                    initFilledCtor[indexer] = service;
                    indexer++;
                }
                ControllerInstaces.Add(key, (T2)Activator.CreateInstance(t, args: initFilledCtor));
            }
            _controller = ControllerInstaces[key];


        }

        public enum WEBAPI_VERSION : int
        {
            V1,
            V2
        }


        public HttpClient CreateHttpClient(WEBAPI_VERSION webApiVersion)
        {
            var client = _factory.CreateClient();
            switch (webApiVersion)
            {
                case WEBAPI_VERSION.V1:
                    break;
                case WEBAPI_VERSION.V2:
                    client.DefaultRequestHeaders.Add("Authorization", "xxx");

                    break;
                default:
                    break;
            }
            return client;
        }

        public static void SetUp()
        {
            var builder = new ContainerBuilder();

            var jsonSerOpt = JsonHandler.Settings;
            //jsonSerOpt.Converters.Add(new WebApiFunction.Converter.JsonConverter.JsonBoolConverter());
            var jsonHandler = new JsonHandler(jsonSerializerOptions: jsonSerOpt);

            string configPath = Environment.CurrentDirectory;
            #region Initial Configurations
            ApiSecurityConfigurationModel initialApiSecurityConfigurationModel = new ApiSecurityConfigurationModel();
            initialApiSecurityConfigurationModel.ApiContentType = GeneralDefs.ApiContentType;
            initialApiSecurityConfigurationModel.Jwt = new ApiSecurityConfigurationModel.JsonWebTokenModel();
            initialApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr = "this is my custom Secret key for authnetication";
            initialApiSecurityConfigurationModel.SiteProtect = new ApiSecurityConfigurationModel.SiteProtectModel();
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpContentLen = 0;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldLen = 255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldValueLen = 255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpRequUriLen = 255;

            AntivirusConfigurationModel initialAntiVirusConfigurationModel = new AntivirusConfigurationModel();
            initialAntiVirusConfigurationModel.DeleteInfectedFilesPermantly = true;
            initialAntiVirusConfigurationModel.Host = "localhost";
            initialAntiVirusConfigurationModel.Port = 3310;

            LogConfigurationModel initialLogConfigurationModel = new LogConfigurationModel();
            initialLogConfigurationModel.LogdateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.LogtimeFormat = "HH:mm:ss";
            initialLogConfigurationModel.LogLevel = General.MESSAGE_LEVEL.LEVEL_INFO;
            initialLogConfigurationModel.UserInterfaceDateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.UserInterfaceTimeFormat = "HH:mm:ss";

            MailConfigurationModel initialMailConfigurationModel = new MailConfigurationModel();
            initialMailConfigurationModel.EmailAttachmentPath = "";
            initialMailConfigurationModel.ImapSettings = new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.ImapSettings.Server = "imap.strato.de";
            initialMailConfigurationModel.ImapSettings.Port = 993;
            initialMailConfigurationModel.ImapSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.ImapSettings.LoggerFile = "imap.log";
            initialMailConfigurationModel.ImapSettings.Timeout = 10000;
            initialMailConfigurationModel.SmtpSettings = new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.SmtpSettings.Server = "smtp.strato.log";
            initialMailConfigurationModel.SmtpSettings.Port = 465;
            initialMailConfigurationModel.SmtpSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.SmtpSettings.LoggerFile = "smtp.log";
            initialMailConfigurationModel.SmtpSettings.Timeout = 10000;

            DatabaseConfigurationModel initialDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialDatabaseConfigurationModel.Host = "localhost";
            initialDatabaseConfigurationModel.Port = 3306;
            initialDatabaseConfigurationModel.Database = "rest_api";
            initialDatabaseConfigurationModel.User = "rest";
            initialDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialDatabaseConfigurationModel.Timeout = 300;
            initialDatabaseConfigurationModel.ConvertZeroDateTime = true;
            initialDatabaseConfigurationModel.OldGuids = true;

            AmpqConfigurationModel initialRabbitMqConfigurationModel = new AmpqConfigurationModel();
            initialRabbitMqConfigurationModel.Host = "localhost";
            initialRabbitMqConfigurationModel.Port = 5672;
            initialRabbitMqConfigurationModel.User = "webapi";
            initialRabbitMqConfigurationModel.Password = "admin1234";
            initialRabbitMqConfigurationModel.VirtualHost = "webapi";
            initialRabbitMqConfigurationModel.HeartBeatMs = 30000;

            CacheConfigurationModel initialCacheConfigurationModel = new CacheConfigurationModel();
            initialCacheConfigurationModel.Hosts = new CacheHostConfigurationModel[] {
                new CacheHostConfigurationModel{ Host="10.0.0.77",Port=7300,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7301,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7302,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7303,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7304,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7305,Timeout=20, User="",Password="test"},
            };

            WebApiConfigurationModel initialWebApiConfigurationModel = new WebApiConfigurationModel();
            initialWebApiConfigurationModel.Encoding = "UTF-8";


            //merged alle config in eine json namens: appservice.json
            AppServiceConfigurationModel initialAppServiceConfigurationModel = new AppServiceConfigurationModel(configPath);
            initialAppServiceConfigurationModel.AntivirusConfigurationModel = initialAntiVirusConfigurationModel;
            initialAppServiceConfigurationModel.DatabaseConfigurationModel = initialDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.ApiSecurityConfigurationModel = initialApiSecurityConfigurationModel;
            initialAppServiceConfigurationModel.WebApiConfigurationModel = initialWebApiConfigurationModel;
            initialAppServiceConfigurationModel.LogConfigurationModel = initialLogConfigurationModel;
            initialAppServiceConfigurationModel.MailConfigurationModel = initialMailConfigurationModel;
            initialAppServiceConfigurationModel.CacheConfigurationModel = initialCacheConfigurationModel;
            initialAppServiceConfigurationModel.RabbitMqConfigurationModel = initialRabbitMqConfigurationModel;

            #endregion Initial Configurations
            string basePath = Path.Combine(configPath, "Config");
            string appsettingsFile = Path.Combine(basePath, "appsettings.json");
            var configurationBuilder = new ConfigurationBuilder().
                SetBasePath(configPath).
                AddCustomWebApiConfig(basePath, initialAppServiceConfigurationModel).
                AddJsonFile(appsettingsFile).

                AddEnvironmentVariables();

            var envVars = Environment.GetEnvironmentVariables();
            foreach (var envVar in envVars.Keys)
            {
                Console.WriteLine("ENV: " + envVar + ":" + envVars[envVar].ToString());
            }

            builder.Register(x => configurationBuilder.Build()).As<IConfiguration>().SingleInstance();

            var appConfig = new AppConfig(jsonHandler, configPath);
            builder.Register(x => jsonHandler).As<ISingletonJsonHandler>().SingleInstance();
            builder.Register(x => jsonHandler).As<IScopedJsonHandler>().OwnedByLifetimeScope();
            builder.Register(x => appConfig).As<IAppconfig>().SingleInstance();
            builder.RegisterType<TaskSchedulerBackgroundServiceQueuer>().As<ITaskSchedulerBackgroundServiceQueuer>().SingleInstance();
            builder.RegisterType<HttpContextHandler>().As<IHttpContextHandler>().OwnedByLifetimeScope();



            MemoryCacheOptions localCacheOptions = new MemoryCacheOptions { };

            builder.Register(x => new MemoryCache(localCacheOptions)).As<IMemoryCache>().SingleInstance();
            var config = appConfig.AppServiceConfiguration.CacheConfigurationModel;
            var connectionMultiplexer = CachingHandlerExtensions.CreateConnectionMultiplexer(config);
            if (connectionMultiplexer != null)
                builder.Register(x => connectionMultiplexer).As<IConnectionMultiplexer>().SingleInstance();
            builder.RegisterType<CachingHandler>().As<ICachingHandler>().SingleInstance();

            builder.RegisterType<FileHandler>().As<IFileHandler>().SingleInstance();
            builder.Register(x => new VulnerablityHandler(appConfig.AppServiceConfiguration.AppPaths[AppConfigDefinitionProperties.PathDictKeys.File.NClamQuarantinePath], "", 1, true)).As<ISingletonVulnerablityHandler>().SingleInstance();
            builder.Register(x => new VulnerablityHandler(appConfig.AppServiceConfiguration.AppPaths[AppConfigDefinitionProperties.PathDictKeys.File.NClamQuarantinePath], "", 1)).As<IScopedVulnerablityHandler>().OwnedByLifetimeScope();
            builder.RegisterType<MailHandler>().As<IMailHandler>().SingleInstance();
            builder.RegisterType<JWTHandler>().As<IJWTHandler>().OwnedByLifetimeScope();

            DataMapperForDapperExtension.UseCustomDataMapperForDapper();
            DapperTypeConverterHandler.UseStringToGuidConversion();
            builder.RegisterType<MysqlDapperContext>().SingleInstance();
            builder.RegisterType<MySqlDatabaseHandler>().As<IScopedDatabaseHandler>().OwnedByLifetimeScope();
            builder.RegisterType<MySqlDatabaseHandler>().As<ISingletonDatabaseHandler>().SingleInstance();
            builder.RegisterType<NodeManagerHandler>().As<INodeManagerHandler>().SingleInstance();
            builder.RegisterType<SiteProtectHandler>().As<IScopedSiteProtectHandler>().OwnedByLifetimeScope();
            builder.RegisterType<SiteProtectHandler>().As<ISingletonSiteProtectHandler>().SingleInstance();
            builder.RegisterType<AuthHandler>().As<IAuthHandler>().OwnedByLifetimeScope();
            builder.RegisterType<JsonApiDataHandler>().As<IJsonApiDataHandler>().OwnedByLifetimeScope();
            builder.RegisterType<ClientErrorHandler>().As<IClientErrorFactory>().InstancePerDependency();
            builder.RegisterType<EncryptionHandler>().As<ISingletonEncryptionHandler>().SingleInstance();
            builder.RegisterType<EncryptionHandler>().As<IScopedEncryptionHandler>().OwnedByLifetimeScope();
            builder.RegisterType<RabbitMqHandler>().As<IRabbitMqHandler>().SingleInstance();
            builder.Register(x => new NullLogger<T2>()).As<ILogger<T2>>().SingleInstance();
            builder.Register(x => new NullLogger<RabbitMqHandler>()).As<ILogger<RabbitMqHandler>>().SingleInstance();



            Container = builder.Build();



            InitialSetUp = true;
        }



    }

    public static class CustomApiControllerBaseTestExtension
    {

        public static async Task<HttpResponseMessage> ExecuteHttpMethod(WebApplicationFactory<Startup> webApplicationFactory, HttpClient httpClient, HttpMethod httpMethod, string relativeRequestUri, string content, string contentType)
        {

            var request = new HttpRequestMessage(httpMethod, relativeRequestUri);

            request.Content = new StringContent(String.IsNullOrEmpty(content) ? String.Empty : content, Encoding.UTF8, contentType);
            var serverClient = webApplicationFactory.Server.CreateClient();
            var context = serverClient.GetAsync("/").Result.Content;

            var response = await httpClient.SendAsync(request);
            return response ?? new HttpResponseMessage(System.Net.HttpStatusCode.BadGateway)
            {
                Content = new StringContent("remote host or service not available, check your internet connection", Encoding.UTF8, "text/plain")
            };
        }
    }

}
