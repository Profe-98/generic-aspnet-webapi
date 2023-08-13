using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.IO;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService;
using Application.Shared.Kernel.Web.AspNet.Healthcheck;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Extension;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Configuration;
using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;
using Application.Shared.Kernel.Configuration.Model.Abstraction;
using Application.Shared.Kernel.Infrastructure.Log;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Web.AspNet.Startup;
using Application.Shared.Web.Api.Shared.Middleware;

namespace Application.Shared.Web.Api.Shared
{
    public class Startup: WebApiStartup
    {
        public override string[] DatabaseEntityNamespaces { get; } = new string[] { "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View",
                            "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table" };

        readonly string AllowOrigin = "api-gateway";

        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) : base(configuration,env)
        {
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebApi(Configuration,DatabaseEntityNamespaces);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IServiceProvider serviceProvider)
        {
            app.UseExceptionHandler("/error");
            //app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-debug");
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseCookiePolicy();
            //app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseCors(AllowOrigin);//must used between UseRouting & UseEndpoints
            app.UseRequestLocalization();
            
            //app.UseAuthentication();
            //app.UseAuthorization();
            //app.UseSession();
            app.UseCustomResponseBodyMiddleware();
            app.UseHealthChecks(BackendAPIDefinitionsProperties.HealthController, new HealthCheckOptions()
            {
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                ResponseWriter = HealthCheckResponseWriter.WriteResponse, AllowCachingResponses = false
                 
            });
            ISingletonNodeDatabaseHandler singletonNodeDatabaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
            ISingletonDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonDatabaseHandler>();
            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            IAppconfig appConfig = serviceProvider.GetService<IAppconfig>();
            app.UseEndpoints(endpoints =>
            {
                if (appConfig.AppServiceConfiguration.SignalRHubConfigurationModel != null && appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.UseLocalHub)
                    endpoints.RegisterSignalRHubs(serviceProvider);//signalr init before register backend for route register
                endpoints.RegisterBackend(nodeManager, serviceProvider, env, singletonNodeDatabaseHandler, actionDescriptorCollectionProvider,Configuration, DatabaseEntityNamespaces, HubServiceExtensions.RegisteredHubServices);

            });

            /*app.UseSpa(spa =>
            {
                if(env.IsDevelopment())
                {
                    spa.Options.SourcePath = System.IO.Path.Join(env.ContentRootPath, "ClientApp");
                    Console.WriteLine(env.ContentRootPath);

                }
                else
                {
                    spa.Options.SourcePath = "ClientApp";

                }

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });*/
        }

        public override IMainConfigurationModel SetInitialConfiguration(string rootDir)
        {
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

            ClamAvConfigurationModel initialAntiVirusConfigurationModel = new ClamAvConfigurationModel();
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
            initialMailConfigurationModel.ImapSettings.LoggerFolderPath = Path.Combine(Environment.CurrentDirectory, "imap_logs");
            initialMailConfigurationModel.ImapSettings.Timeout = 10000;
            initialMailConfigurationModel.SmtpSettings = new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.SmtpSettings.Server = "smtp.strato.log";
            initialMailConfigurationModel.SmtpSettings.Port = 465;
            initialMailConfigurationModel.SmtpSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.SmtpSettings.LoggerFolderPath = Path.Combine(Environment.CurrentDirectory, "smtp_logs");
            initialMailConfigurationModel.SmtpSettings.Timeout = 10000;

            DatabaseConfigurationModel initialDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialDatabaseConfigurationModel.Host = "localhost";
            initialDatabaseConfigurationModel.Port = 3306;
            initialDatabaseConfigurationModel.Database = "api_gateway";
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

            SignalRConfigurationModel initialSignalRConfigurationModel = new SignalRConfigurationModel();
            initialSignalRConfigurationModel.UseLocalHub = false;
            initialSignalRConfigurationModel.DebugErrorsDetailedClientside = false;
            initialSignalRConfigurationModel.TimoutTimeSec = 15;
            initialSignalRConfigurationModel.KeepaliveTimeout = 15;
            initialSignalRConfigurationModel.ClientTimeoutSec = 30;
            initialSignalRConfigurationModel.HandshakeTimeout = 5;
            initialSignalRConfigurationModel.MaximumParallelInvocationsPerClient = 1;

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
            MainConfigurationModel initialAppServiceConfigurationModel = new MainConfigurationModel(rootDir);
            initialAppServiceConfigurationModel.AntivirusConfigurationModel = initialAntiVirusConfigurationModel;
            initialAppServiceConfigurationModel.DatabaseConfigurationModel = initialDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.ApiSecurityConfigurationModel = initialApiSecurityConfigurationModel;
            initialAppServiceConfigurationModel.WebApiConfigurationModel = initialWebApiConfigurationModel;
            initialAppServiceConfigurationModel.LogConfigurationModel = initialLogConfigurationModel;
            initialAppServiceConfigurationModel.MailConfigurationModel = initialMailConfigurationModel;
            initialAppServiceConfigurationModel.CacheConfigurationModel = initialCacheConfigurationModel;
            initialAppServiceConfigurationModel.RabbitMqConfigurationModel = initialRabbitMqConfigurationModel;
            initialAppServiceConfigurationModel.SignalRHubConfigurationModel = initialSignalRConfigurationModel;

            #endregion Initial Configurations
            return initialAppServiceConfigurationModel;
        }

        public override IConfiguration LoadConfiguration(IConfiguration previousConfig, IWebHostEnvironment env, IMainConfigurationModel cfg)
        {
            Console.WriteLine("ContetnRootPath: " + env.ContentRootPath + "");
            string basePath = Path.Combine(env.ContentRootPath, "Config");
            string appsettingsFile = Path.Combine(basePath, "appsettings.json");
            var configurationBuilder = new ConfigurationBuilder().
                SetBasePath(env.ContentRootPath).
                AddCustomWebApiConfig<AbstractConfigurationModel>(basePath, (AbstractConfigurationModel)cfg).
                AddJsonFile(appsettingsFile).

                AddEnvironmentVariables();

            var envVars = Environment.GetEnvironmentVariables();
            foreach (var envVar in envVars.Keys)
            {
                Console.WriteLine("ENV: " + envVar + ":" + envVars[envVar].ToString());
            }
            var config = configurationBuilder.AddConfiguration(previousConfig).Build();
            return config;  
        }
    }
}
