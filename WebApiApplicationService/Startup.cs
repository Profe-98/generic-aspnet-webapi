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
using WebApiApplicationService.Modules;
using System.Collections.Generic;
using WebApiApplicationService.Models;
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
using WebApiApplicationService.Formatters;
using System.Linq.Expressions;
using System.IO;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Microsoft.AspNetCore.Http;
using WebApiApplicationService.Health;

namespace WebApiApplicationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            #region Initial Configurations
            ApiSecurityConfigurationModel initialApiSecurityConfigurationModel = new ApiSecurityConfigurationModel();
            initialApiSecurityConfigurationModel.ApiContentType = GeneralDefs.ApiContentType;
            initialApiSecurityConfigurationModel.Jwt = new ApiSecurityConfigurationModel.JsonWebTokenModel();
            initialApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr = "this is my custom Secret key for authnetication";
            initialApiSecurityConfigurationModel.SiteProtect = new ApiSecurityConfigurationModel.SiteProtectModel();
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpContentLen =0;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldLen =255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldValueLen =255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpRequUriLen =255;

            AntivirusConfigurationModel initialAntiVirusConfigurationModel = new AntivirusConfigurationModel();
            initialAntiVirusConfigurationModel.DeleteInfectedFilesPermantly =true;
            initialAntiVirusConfigurationModel.Host = "localhost";
            initialAntiVirusConfigurationModel.Port=3310;

            LogConfigurationModel initialLogConfigurationModel = new LogConfigurationModel();
            initialLogConfigurationModel.LogdateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.LogtimeFormat = "HH:mm:ss";
            initialLogConfigurationModel.LogLevel = AppManager.MESSAGE_LEVEL.LEVEL_INFO;
            initialLogConfigurationModel.UserInterfaceDateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.UserInterfaceTimeFormat = "HH:mm:ss";

            MailConfigurationModel initialMailConfigurationModel = new MailConfigurationModel();
            initialMailConfigurationModel.EmailAttachmentPath = System.IO.Path.Combine(AppPaths.RootDir, "mail_attachment");
            initialMailConfigurationModel.ImapSettings =new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.ImapSettings.Server = "imap.strato.de";
            initialMailConfigurationModel.ImapSettings.Port =993;
            initialMailConfigurationModel.ImapSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.ImapSettings.LoggerFile = "imap.log";
            initialMailConfigurationModel.ImapSettings.Timeout = 10000;
            initialMailConfigurationModel.SmtpSettings = new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.SmtpSettings.Server ="smtp.strato.log";
            initialMailConfigurationModel.SmtpSettings.Port =465;
            initialMailConfigurationModel.SmtpSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.SmtpSettings.LoggerFile ="smtp.log";
            initialMailConfigurationModel.SmtpSettings.Timeout = 10000;

            DatabaseConfigurationModel initialDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialDatabaseConfigurationModel.Host = "localhost";
            initialDatabaseConfigurationModel.Port =3306;
            initialDatabaseConfigurationModel.Database ="rest_api";
            initialDatabaseConfigurationModel.User = "rest";
            initialDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialDatabaseConfigurationModel.Timeout = 300;
            initialDatabaseConfigurationModel.ConvertZeroDateTime =true;
            initialDatabaseConfigurationModel.OldGuids =true;

            WebApiConfigurationModel initialWebApiConfigurationModel = new WebApiConfigurationModel();
            initialWebApiConfigurationModel.Encoding = "UTF-8";


            //merged alle config in eine json namens: appservice.json
            AppServiceConfigurationModel initialAppServiceConfigurationModel = new AppServiceConfigurationModel();
            initialAppServiceConfigurationModel.AntivirusConfigurationModel = initialAntiVirusConfigurationModel;
            initialAppServiceConfigurationModel.DatabaseConfigurationModel = initialDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.ApiSecurityConfigurationModel =initialApiSecurityConfigurationModel;
            initialAppServiceConfigurationModel.WebApiConfigurationModel = initialWebApiConfigurationModel;
            initialAppServiceConfigurationModel.LogConfigurationModel = initialLogConfigurationModel;
            initialAppServiceConfigurationModel.MailConfigurationModel = initialMailConfigurationModel;
            #endregion Initial Configurations
            string basePath = Path.Combine(env.ContentRootPath, "Config");
            string appsettingsFile = Path.Combine(basePath, "appsettings.json");
            var configurationBuilder = new ConfigurationBuilder().
                SetBasePath(env.ContentRootPath).
                AddCustomWebApiConfig<AppServiceConfigurationModel>(basePath, initialAppServiceConfigurationModel).
                AddJsonFile(appsettingsFile).
                
                AddEnvironmentVariables();

            var envVars = Environment.GetEnvironmentVariables();
            foreach (var envVar in envVars.Keys)
            {
                Console.WriteLine("ENV: "+ envVar + ":" + envVars[envVar].ToString());
            }
            Configuration = configurationBuilder.AddConfiguration(configuration).Build();
        }

        readonly string AllowOrigin = "api-gateway";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
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
            jsonSerOpt.Converters.Add(new JsonConverter.JsonBoolConverter());
            var jsonHandler = new JsonHandler(jsonSerializerOptions: jsonSerOpt);
            services.AddSingleton<ISingletonJsonHandler>(x => jsonHandler);
            services.AddScoped<IScopedJsonHandler>(x => jsonHandler);

            services.AddSingleton<IAppconfig, AppConfig>();
            services.AddSingleton<ITaskSchedulerBackgroundServiceQueuer, TaskSchedulerBackgroundServiceQueuer>();
            services.AddScoped<IHttpContextHandler, HttpContextHandler>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var appConfigService = serviceProvider.GetService<IAppconfig>();
            var cacheConfig = appConfigService.AppServiceConfiguration.CacheConfigurationModel;
            services.UseServerSideCache(cacheConfig) ; 
            services.AddSingleton< IFileHandler, FileHandler>();
            services.AddSingleton<ISingletonVulnerablityHandler>(x => new VulnerablityHandler(appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Host, appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Port, true));
            services.AddScoped<IScopedVulnerablityHandler>(x => new VulnerablityHandler(appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Host, appConfigService.AppServiceConfiguration.AntivirusConfigurationModel.Port, true));
            services.AddSingleton<IMailHandler, MailHandler>();
            services.AddScoped<IJWTHandler, JWTHandler>();
            services.AddScoped<IScopedDatabaseHandler, DatabaseHandler>();
            services.AddSingleton<ISingletonDatabaseHandler, DatabaseHandler>();
            services.AddSingleton<INodeManagerHandler,NodeManagerHandler>();
            services.AddScoped<IScopedSiteProtectHandler, SiteProtectHandler>();
            services.AddSingleton<ISingletonSiteProtectHandler, SiteProtectHandler>();
            services.AddScoped<IAuthHandler, AuthHandler>();//dependent on IHttpContextHandler & IScopedDatabaseHandler this is why these both are instancing first by a request
            services.AddScoped<IJsonApiDataHandler, JsonApiDataHandler>();
            services.AddTransient<IClientErrorFactory, ClientErrorHandler>();
            services.AddSingleton<ISingletonEncryptionHandler, EncryptionHandler>();
            services.AddScoped<IScopedEncryptionHandler, EncryptionHandler>();
            services.AddRabbitMq();
            
            serviceProvider = services.BuildServiceProvider();
            var appConfig = serviceProvider.GetService<IAppconfig>();
            var databaseService = serviceProvider.GetService<IScopedDatabaseHandler>();
            var cacheService = serviceProvider.GetService<ICachingHandler>();
            var avService = serviceProvider.GetService<IScopedVulnerablityHandler>();
            var rabbitMqService = serviceProvider.GetService<WebApiApplicationService.IRabbitMqHandler>();


            services.AddHealthChecks()
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(Health.HealthCheckMySql),"mysql", HealthStatus.Unhealthy, new object[]
                    {
                        Health.HealthCheckMySql.CheckMySqlBackend, databaseService
                    })
                })
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(Health.HealthCheckRabbitMq),"rabbitmq", HealthStatus.Unhealthy, new object[]
                    {
                        Health.HealthCheckRabbitMq.CheckRabbitMqBackend, rabbitMqService
                    })
                })
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(Health.HealthCheckCache),"cache", HealthStatus.Unhealthy, new object[]
                    {
                        Health.HealthCheckCache.CheckCacheBackend, cacheService
                    })
                })
                .AddHealthChecks(new List<HealthCheckDescriptor>()
                {
                    new HealthCheckDescriptor(typeof(Health.HealthCheckNClam),"antivirus", HealthStatus.Unhealthy, new object[]
                    {
                        Health.HealthCheckNClam.CheckNClamBackend, avService
                    })
                });

            serviceProvider = services.BuildServiceProvider();
            var test = serviceProvider.GetService<HealthCheckService>();
            /*services.AddMvc(options => 
            {
                options.InputFormatters.Insert(0, new RequestInputFormatter());
            }).;*/
            services.AddControllers(options =>
            {
                int minVal = int.MinValue;
                options.Filters.Add(typeof(HttpFlowFilter), minVal);//wird immer als erster Filter(Global-Filter, pre-executed vor allen Controllern) ausgeführt, danach kommt wenn als Attribute an Controller-Action 'CustomConsumesFilter' mit Order=int.MinValue+1
                options.Filters.Add(typeof(ValidateModelFilter), minVal+1);
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
            //services.AddHostedService<TaskSchedulerBackgroundService>();
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IServiceProvider serviceProvider)
        {
            AppPaths.RootDir = env.ContentRootPath;
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
            app.UseCustomSiteProtectMiddleware();
            app.UseHealthChecks(BackendAPIDefinitionsProperties.HealthController, new HealthCheckOptions()
            {
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                ResponseWriter = Health.HealthCheckResponseWriter.WriteResponse, AllowCachingResponses = false
                 
            });
            ISingletonDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonDatabaseHandler>();
            app.UseEndpoints(endpoints =>
            {

                
                endpoints.RegisterBackend(env,databaseHandler, actionDescriptorCollectionProvider,Configuration);
                
            });
            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            nodeManager.Register(BackendAPIDefinitionsProperties.NodeTypes.Application);

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
    }
}
