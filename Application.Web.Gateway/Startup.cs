using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json.Serialization;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.DownstreamPathManipulation.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Ocelot.Multiplexer;
using Ocelot.Requester.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.QueryStrings.Middleware;
using Ocelot.RateLimit.Middleware;
using Ocelot.Security.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.Claims.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.WebSockets.Middleware;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Web.Http;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Web.Authentification.JWT;
using Application.Shared.Kernel.Security.Encryption;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.AspNet.Filter;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Extension;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Configuration;
using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;
using Application.Shared.Kernel.Configuration.Model.Abstraction;
using System.Text.Json;
using Application.Shared.Kernel.Infrastructure.Log;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq.Data;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Infrastructure.Metric.Influxdb;
using Application.Shared.Kernel.Infrastructure.Database.Mysql;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Web.AspNet.Startup;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View;
using Application.Web.Gateway.Middleware;
using Application.Shared.Kernel.Web.AspNet.Controller;
using StackExchange.Redis;

namespace Application.Web.Gateway
{
    public class Startup : WebApiStartup
    {
        private object FileAccessLockObject = new object();
        public override string[] DatabaseEntityNamespaces { get; } = new string[] { "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table",
                            "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View",
                            "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table",
                            "Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish" };
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) : base(configuration, env)
        {

        }

        readonly string AllowOrigin = "frontend";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options => {
                options.AddPolicy(AllowOrigin, builder => {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.SetIsOriginAllowed(origin => {
                        if (origin != null)
                        {
                            var uri = new Uri(origin);
                            return true;
                        }
                        return false;
                    });
                });
            });
            var jsonSerOpt = new JsonSerializerOptions(JsonHandler.Settings);
            jsonSerOpt.Converters.Add(new Application.Shared.Kernel.Data.Format.Converter.JsonConverter.JsonBoolConverter());
            var jsonHandler = new JsonHandler(jsonSerializerOptions: jsonSerOpt);
            services.AddSingleton<ISingletonJsonHandler>(x => jsonHandler);
            services.AddScoped<IScopedJsonHandler>(x => jsonHandler);

            services.AddSingleton<IAppconfig, AppConfig>();
            services.AddSingleton<ITaskSchedulerBackgroundServiceQueuer, TaskSchedulerBackgroundServiceQueuer>();
            services.AddScoped<IHttpContextHandler, HttpContextHandler>();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IInfluxDbHandlerInterface, InfluxDbHandler>();

            services.AddTransient<ITransientDatabaseHandler, MySqlDatabaseHandler>();
            services.AddScoped<IScopedDatabaseHandler, MySqlDatabaseHandler>();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var appConfigService = serviceProvider.GetService<IAppconfig>();
            if (appConfigService.AppServiceConfiguration.NodeManagerDatabaseConfigurationModel != null)
            {
                services.AddSingleton<ISingletonNodeDatabaseHandler>(new MySqlDatabaseHandler(appConfigService.AppServiceConfiguration.NodeManagerDatabaseConfigurationModel.MysqlConnectionString, appConfigService.AppServiceConfiguration.NodeManagerDatabaseConfigurationModel.AutoCommit));

            }
            services.AddSingleton<ISingletonDatabaseHandler>(new MySqlDatabaseHandler(appConfigService.AppServiceConfiguration.DatabaseConfigurationModel.MysqlConnectionString, appConfigService.AppServiceConfiguration.DatabaseConfigurationModel.AutoCommit));
           
            services.AddSingleton<INodeManagerHandler, NodeManagerHandler>();
            services.AddSingleton<ISingletonEncryptionHandler, EncryptionHandler>();
            services.AddScoped<IScopedEncryptionHandler, EncryptionHandler>();
            services.AddScoped<IJWTHandler, JWTHandler>();
            services.AddScoped<IAuthHandler, AuthHandler>();//dependent on IHttpContextHandler & IScopedDatabaseHandler this is why these both are instancing first by a request

            services.AddScoped<IJsonApiDataHandler, JsonApiDataHandler>();

            serviceProvider = services.BuildServiceProvider();

            ISingletonNodeDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
            CustomControllerBaseExtensions.RegisterNetClasses(databaseHandler, DatabaseEntityNamespaces);
            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            nodeManager.Register();
            services.AddRabbitMq();


            services.AddControllers(options =>
            {
                int minVal = int.MinValue;
                options.Filters.Add(typeof(HttpFlowFilter), minVal);//wird immer als erster Filter(Global-Filter, pre-executed vor allen Controllern) ausgeführt, danach kommt wenn als Attribute an Controller-Action 'CustomConsumesFilter' mit Order=int.MinValue+1
                options.Filters.Add(typeof(AuthorizationFilterC), minVal + 1);//wird immer als erster Filter(Global-Filter, pre-executed vor allen Controllern) ausgeführt, danach kommt wenn als Attribute an Controller-Action 'CustomConsumesFilter' mit Order=int.MinValue+1
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
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
            /*
             * für OpenId Dienste like Facebook, Google, Microsoft
             * 
             * Action<JwtBearerOptions> options = o =>
            {
                o.RequireHttpsMetadata = false;
                o.Authority = "http://localhost:5009/";
                o.Audience = "http://localhost:5009/";
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    
                };
                
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options);*/



            /*
            var envVars = Environment.GetEnvironmentVariables();
            if(!envVars.Contains("AUTH_HOST"))
            {
                throw new InvalidOperationException("ENV Var 'AUTH_HOST' contains not the ip of the auth endpoint");
            }
            var authHost = envVars["AUTH_HOST"].ToString(); 
            services.AddHttpClient("RemoteAuthentificationServiceClient", c => 
            {
                
                c.BaseAddress = new Uri("http://"+ authHost + ":5009/helix-api-1/authentification/validate");

                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });*/
            services.AddAuthentication("Base").
                AddScheme<BasicAuthenticationOptions, AuthentificationHandler>("Base",null, new Action<BasicAuthenticationOptions>(o =>
                {
                    /*o.Events = new JwtBearerEvents()
                    { 
                        OnTokenValidated = context =>
                        {
                            
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {

                            return Task.CompletedTask;
                        },
                    };*/
                    //o.ForwardDefault = "RemoteAuthentificationService";

                    //o.BackchannelHttpHandler = new RemoteAuthentificationRequester(handler, "");

                }))/*.
                    * 
                    * für OpenId Dienste
                    * 
                AddRemoteScheme<BasicRemoteAuthenticationOptions, RemoteAuthentificationHandler>("RemoteAuthentificationService", "RemoteAuthentificationService", new Action<BasicRemoteAuthenticationOptions>(async(o)=> 
                {
                    try
                    {
                        o.CallbackPath = new Microsoft.AspNetCore.Http.PathString("/api/test/login");
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }))*/;

            /*
             * services.AddAuthorization wird nicht genutzt, da über den AuthenticationProviderKey in der routes.json (Auth Schema) über den AuthentificationHandler das Claim an die ClaimsIdentity angehangen wird
             * 
             * services.AddAuthorization(option => 
            {
                option.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Base")
                    .RequireClaim("roles", "roots")
                    .RequireAuthenticatedUser()
                    .Build();
            });*/

            services.UseAsyncTaskScheduler();
            services.AddOcelot(Configuration)
                .AddDelegatingHandler<GeneralMiddlewareDelegate>(true);
        }
        public async void WriteRouteToFile(string contentRootPath,RoutesConfigurationModel routesConfigurationModel,JsonHandler jsonHandler)
        {
            string json = jsonHandler.JsonSerialize<RoutesConfigurationModel>(routesConfigurationModel,
                            new System.Text.Json.JsonSerializerOptions
                            {
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                WriteIndented = true,

                            });

            string basePath = Path.Combine(contentRootPath, "Config");
            string routesFile = Path.Combine(basePath, "routes.json");
            await System.IO.File.WriteAllTextAsync(routesFile, json);
        }
        public static async Task<RoutesConfigurationModel> CreateRoutes(ITransientDatabaseHandler databaseHandler)
        {
            RoutesConfigurationModel routesConfigurationModel = new RoutesConfigurationModel();
            routesConfigurationModel.Routes = new List<RoutesConfigurationModel.RouteModel>();
            routesConfigurationModel.GlobalConfiguration = new RoutesConfigurationModel.GlobalConfigurationModel();
            routesConfigurationModel.GlobalConfiguration.BaseUrl = "http://localhost:5000/";
            routesConfigurationModel.GlobalConfiguration.DelegatingHandlers = new List<string>() { "GeneralMiddlewareDelegate" };

            QueryResponseData<NodeHealthEndpointViewModel> routesHealthQueryData = await databaseHandler.ExecuteQuery<NodeHealthEndpointViewModel>("SELECT * FROM node_health_endpoint_view;");
            if (routesHealthQueryData.HasData)
            {
                foreach (var item in routesHealthQueryData.DataStorage)
                {
                    var route = new RoutesConfigurationModel.RouteModel();
                    route.AuthenticationOptions = new RoutesConfigurationModel.AuthenticationOptionsModel
                    {
                        AuthenticationProviderKey = "Base"
                    };
                    string pathTemplateDownStream = "/health";
                    string pathTemplateUpStream = "/" + item.Uuid + "/health";
                    route.DownstreamPathTemplate = pathTemplateDownStream;
                    route.DownstreamScheme = "http";

                    route.UpstreamHttpMethod = new List<string>() { "Get" };
                    route.UpstreamPathTemplate = pathTemplateUpStream;
                    route.RouteClaimsRequirement = new RoutesConfigurationModel.RouteClaimsRequirementModel { Role=new Dictionary<string, string> { { BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, "root" } } };
                    route.LoadBalancerOptions = new RoutesConfigurationModel.LoadBalancerOptionsModel
                    {
                        Type = "RoundRobin"
                    };
                    route.DownstreamHostAndPorts = new List<RoutesConfigurationModel.DownstreamHostAndPortModel>() { new RoutesConfigurationModel.DownstreamHostAndPortModel() { Host = item.IPAddr, Port = item.Port } };
                    routesConfigurationModel.Routes.Add(route);
                    System.Diagnostics.Debug.WriteLine("register+" + route.UpstreamPathTemplate);
                }
            }
            QueryResponseData<SignalrHubsViewModel> routesSignalRHubsQueryData = await databaseHandler.ExecuteQuery<SignalrHubsViewModel>("SELECT * FROM signalr_hubs group by route;");
            if (routesSignalRHubsQueryData.HasData)
            {
                foreach (var item in routesSignalRHubsQueryData.DataStorage)
                {
                    bool isAnonEndpoint = item.Roles == null ? true : false;
                    var route = new RoutesConfigurationModel.RouteModel();
                    route.AuthenticationOptions = new RoutesConfigurationModel.AuthenticationOptionsModel
                    {
                        AuthenticationProviderKey = "Base"
                    };

                    /*List<string> claims = new List<string>();
                    if (item.Roles != null && !isAnonEndpoint)
                    {
                        foreach (string role in item.Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!claims.Contains(role))
                            {
                                claims.Add(role);
                            }
                        }
                    }*/
                    string pathTemplateDownStream = item.Route;
                    string pathTemplateUpStream = item.Route;
                    route.DownstreamPathTemplate = pathTemplateDownStream;
                    route.DownstreamScheme = "http";

                    Dictionary<string, string> claims = new Dictionary<string, string>();
                    if (item.Roles != null && !isAnonEndpoint)
                    {
                        /*
                        foreach (string role in row.Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                        }
                        */
                        if (!claims.ContainsKey(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole))
                        {
                            claims.Add(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, null);
                        }
                        claims[BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole] = item.Roles.ToLower();
                    }
                    route.RouteClaimsRequirement = !isAnonEndpoint ? new RoutesConfigurationModel.RouteClaimsRequirementModel { Role = claims } : null;

                    List<string> methodData = item.HttpMethods != null ? item.HttpMethods.Split(',').ToList() : new List<string>();
                    for (int i = 0; i < methodData.Count; i++)
                    {
                        string firstLetter = methodData[i].Substring(0, 1);
                        string commonString = methodData[i].Substring(1, methodData[i].Length - 1);
                        methodData[i] = ((firstLetter.ToUpper()) + (commonString.ToLower()));
                    }
                    route.UpstreamHttpMethod = methodData;
                    route.UpstreamPathTemplate = pathTemplateUpStream;
                    route.LoadBalancerOptions = new RoutesConfigurationModel.LoadBalancerOptionsModel
                    {
                        Type = "RoundRobin"
                    };
                    route.DownstreamHostAndPorts = new List<RoutesConfigurationModel.DownstreamHostAndPortModel>();
                    foreach (var socket in item.AvailableNodes)
                    {
                        route.DownstreamHostAndPorts.Add(new RoutesConfigurationModel.DownstreamHostAndPortModel
                        {
                            Host = socket.Key.Address.ToString(),
                            Port = socket.Key.Port
                        });
                    }
                    routesConfigurationModel.Routes.Add(route);
                    //negotiation route
                    RoutesConfigurationModel.RouteModel negotiationRoute = new RoutesConfigurationModel.RouteModel();
                    negotiationRoute.AuthenticationOptions = route.AuthenticationOptions;
                    negotiationRoute.UpstreamPathTemplate = pathTemplateUpStream + "/negotiate";
                    negotiationRoute.DownstreamPathTemplate = pathTemplateUpStream + "/negotiate";
                    negotiationRoute.DownstreamScheme = route.DownstreamScheme;
                    negotiationRoute.UpstreamHttpMethod = route.UpstreamHttpMethod;
                    negotiationRoute.RouteClaimsRequirement = route.RouteClaimsRequirement;
                    negotiationRoute.LoadBalancerOptions = route.LoadBalancerOptions;
                    negotiationRoute.DownstreamHostAndPorts = route.DownstreamHostAndPorts;
                    routesConfigurationModel.Routes.Add(negotiationRoute);
                    System.Diagnostics.Debug.WriteLine("register+" + negotiationRoute.UpstreamPathTemplate);
                }
            }
            QueryResponseData<ControllerRelationToRoleView> routesQueryData = await databaseHandler.ExecuteQuery<ControllerRelationToRoleView>("SELECT * FROM controller_relation_to_role_view;");
            if (routesQueryData.HasData)
            {

                foreach (var row in routesQueryData.DataStorage)
                {
                    bool isAnonEndpoint = row.Roles == null ? true : false;
                    //optionale Routeparameter können von Ocelot nicht verarbeitet werden, diese rufe eine Exception in der UseRouting Methode von Ocelot aus und da die Mainroute eh besteht passt es auch so
                    //Bsp.:
                    // Route 1: /account
                    // Route 2: /account/{id}
                    // Route 3: /account/{id?} <----------- Exception und doppelt da Route 2 ja schon existiert
                    if (row.ActionRoute == null)//||row.ActionRoute.Contains("?}"))
                        continue;

                    var route = new RoutesConfigurationModel.RouteModel();
                    if (!isAnonEndpoint)
                    {
                        route.AuthenticationOptions = new RoutesConfigurationModel.AuthenticationOptionsModel
                        {
                            AuthenticationProviderKey = "Base"
                        };
                    }
                    else
                    {

                    }
                    route.DownstreamHostAndPorts = new List<RoutesConfigurationModel.DownstreamHostAndPortModel>();
                    foreach (var item in row.AvailableNodes)
                    {
                        var endPoint = item.Key;
                        string ipStr = endPoint.Address.ToString();
                        /*using (System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping())
                        {
                            var reply = await ping.SendPingAsync(ipStr);
                            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                            {
                            }
                        }*/
                        bool available = (DateTime.Now - new TimeSpan(0, 0, 60) < item.Value);
                        if (available)
                            route.DownstreamHostAndPorts.Add(new RoutesConfigurationModel.DownstreamHostAndPortModel() { Host = ipStr, Port = endPoint.Port });
                        else
                        {
                        }


                    }

                    List<string> methodData = row.HttpMethods != null ? row.HttpMethods.Split(',').ToList() : new List<string>();
                    for (int i = 0; i < methodData.Count; i++)
                    {
                        string firstLetter = methodData[i].Substring(0, 1);
                        string commonString = methodData[i].Substring(1, methodData[i].Length - 1);
                        methodData[i] = ((firstLetter.ToUpper()) + (commonString.ToLower()));
                    }
                    string pathTemplate = row.ActionRoute != null ? ("/" + row.ActionRoute) : null;
                    route.DownstreamPathTemplate = pathTemplate;
                    route.DownstreamScheme = "http";
                    route.UpstreamHttpMethod = methodData;
                    route.UpstreamPathTemplate = pathTemplate;
                    route.LoadBalancerOptions = new RoutesConfigurationModel.LoadBalancerOptionsModel
                    {
                        Type = "RoundRobin"
                    };
                    if (row.Roles == null)
                    {
                        row.Roles = BackendAPIDefinitionsProperties.AnonymousRoleName;
                    }
                    if (row.ActionRoute == null || row.HttpMethods == null || row.Roles == null || route.DownstreamHostAndPorts.Count == 0)
                    {
                        Console.WriteLine("avoid route '" + row.ActionRoute + "' because (if (row.ActionRoute == null || row.HttpMethods == null || row.Roles == null || route.DownstreamHostAndPorts.Count == 0))");
                        continue;
                    }
                    Dictionary<string,string> claims = new Dictionary<string,string>();
                    if (row.Roles != null && !isAnonEndpoint)
                    {
                        /*
                        foreach (string role in row.Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                        }
                        */
                        if (!claims.ContainsKey(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole))
                        {
                            claims.Add(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, null);
                        }
                        claims[BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole] = row.Roles.ToLower();
                    }
                    route.RouteClaimsRequirement = !isAnonEndpoint ? new RoutesConfigurationModel.RouteClaimsRequirementModel { Role=claims } : null;

                    bool hasOptionalIdParameter = route.UpstreamPathTemplate.Contains(BackendAPIDefinitionsProperties.ActionParameterOptionalIdWildcard);

                    string compareValue = route.UpstreamPathTemplate.Replace("?", "");
                    var findItem = routesConfigurationModel.Routes.Find(x => x.UpstreamPathTemplate == compareValue);
                    if (findItem == null)
                    {
                        route.UpstreamPathTemplate = compareValue;
                        route.DownstreamPathTemplate = compareValue;
                        routesConfigurationModel.Routes.Add(route);
                        System.Diagnostics.Debug.WriteLine("register+" + route.UpstreamPathTemplate);

                    }
                    else
                    {
                        int index = routesConfigurationModel.Routes.IndexOf(findItem);
                        foreach (string methodDataItem in route.UpstreamHttpMethod)
                        {
                            var existHttpMethodAlready = routesConfigurationModel.Routes[index].UpstreamHttpMethod.Find(x => x == methodDataItem);
                            if (existHttpMethodAlready == null)
                            {
                                routesConfigurationModel.Routes[index].UpstreamHttpMethod.Add(methodDataItem);
                            }
                        }
                    }

                    if (hasOptionalIdParameter)
                    {

                        string foundPath = route.UpstreamPathTemplate.Replace(BackendAPIDefinitionsProperties.ActionParameterOptionalIdWildcard, "").Replace(BackendAPIDefinitionsProperties.ActionParameterIdWildcard, "");
                        var findItemWithoutId = routesConfigurationModel.Routes.Find(x => x.UpstreamPathTemplate == foundPath);
                        if (findItemWithoutId == null)
                        {
                            var newRouteItem = route;
                            newRouteItem.DownstreamPathTemplate = foundPath;
                            newRouteItem.UpstreamPathTemplate = foundPath;

                            routesConfigurationModel.Routes.Add(newRouteItem);
                        }
                        else
                        {
                            int indexOfRouteItem = routesConfigurationModel.Routes.IndexOf(findItemWithoutId);
                            foreach (string methodDataItem in route.UpstreamHttpMethod)
                            {
                                var existHttpMethodAlready = routesConfigurationModel.Routes[indexOfRouteItem].UpstreamHttpMethod.Find(x => x == methodDataItem);
                                if (existHttpMethodAlready == null)
                                {
                                    routesConfigurationModel.Routes[indexOfRouteItem].UpstreamHttpMethod.Add(methodDataItem);
                                }
                            }
                        }
                    }
                }
            }
            return routesConfigurationModel;
        }
        Func<MessageModel, MessageModelResponse> Func = new Func<MessageModel, MessageModelResponse>((x) => {
            MessageModelResponse messageModelResponse = new MessageOK();
            Console.WriteLine("route-management: node exposed some routes (" + x.Message + ", uuid: " + x.NodeId + ")");
            return messageModelResponse;
        });
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider serviceProvider, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {

            ITransientDatabaseHandler databaseHandler = serviceProvider.GetService<ITransientDatabaseHandler>();
            IAppconfig appconfig = serviceProvider.GetService<IAppconfig>();
            IRabbitMqHandler rabbitMqHandler = serviceProvider.GetService<IRabbitMqHandler>();

            string contentRootPath = env.ContentRootPath;

            rabbitMqHandler.SubscibeExchange("route-management", x=>Func(x),new Action(async() => {

                var connStr = new MySqlConnectionStringBuilder();
                connStr.Server = "localhost";
                connStr.Port = 3306;
                connStr.UserID = "rest";
                connStr.Database = "api_gateway";
                connStr.Password = "meinDatabasePassword!";
                MySqlDatabaseHandler mySqlDatabaseHandler = new MySqlDatabaseHandler(connStr,true);
                var r = await CreateRoutes(mySqlDatabaseHandler);
                if(r != null && r.Routes.FindAll(x=>x.DownstreamHostAndPorts.Count == 0).Count != 0) 
                {
                    throw new InvalidOperationException("is seems that at least one route has no defined DownstreamHostAndPorts");
                }
                WriteRouteToFile(contentRootPath,r,new JsonHandler());
            }), "register-notification");
            app.UseCors(AllowOrigin);
            app.UseWebSockets();
            app.UseOcelot((builder, ocelotPipelineConfiguration) =>
            {

                builder.UseRouting();


                builder.UseDownstreamContextMiddleware();

                // This is registered to catch any global exceptions that are not handled
                // It also sets the Request Id if anything is set globally
                builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
                // If the request is for websockets upgrade we fork into a different pipeline
                builder.MapWhen(httpContext => httpContext.WebSockets.IsWebSocketRequest,
                    wenSocketsApp =>
                    {
                        wenSocketsApp.UseDownstreamRouteFinderMiddleware();
                        wenSocketsApp.UseMultiplexingMiddleware();
                        wenSocketsApp.UseDownstreamRequestInitialiser();
                        wenSocketsApp.UseLoadBalancingMiddleware();
                        wenSocketsApp.UseDownstreamUrlCreatorMiddleware();
                        wenSocketsApp.UseWebSocketsProxyMiddleware();
                    });

                // Allow the user to respond with absolutely anything they want.
                //app.UseIfNotNull(pipelineConfiguration.PreErrorResponderMiddleware);

                // This is registered first so it can catch any errors and issue an appropriate response
                builder.UseResponderMiddleware();

                // Then we get the downstream route information
                builder.UseDownstreamRouteFinderMiddleware();

                // Multiplex the request if required
                builder.UseMultiplexingMiddleware();

                // This security module, IP whitelist blacklist, extended security mechanism
                builder.UseSecurityMiddleware();

                //Expand other branch pipes
                /*if (pipelineConfiguration.MapWhenOcelotPipeline != null)
                {
                    foreach (var pipeline in pipelineConfiguration.MapWhenOcelotPipeline)
                    {
                        // todo why is this asking for an app app?
                        app.MapWhen(pipeline.Key, pipeline.Value);
                    }
                }*/

                // Now we have the ds route we can transform headers and stuff?
                builder.UseHttpHeadersTransformationMiddleware();

                // Initialises downstream request
                builder.UseDownstreamRequestInitialiser();

                // We check whether the request is ratelimit, and if there is no continue processing
                builder.UseRateLimiting();

                // This adds or updates the request id (initally we try and set this based on global config in the error handling middleware)
                // If anything was set at global level and we have a different setting at re route level the global stuff will be overwritten
                // This means you can get a scenario where you have a different request id from the first piece of middleware to the request id middleware.
                builder.UseRequestIdMiddleware();

                // Allow pre authentication logic. The idea being people might want to run something custom before what is built in.
                //app.UseIfNotNull(pipelineConfiguration.PreAuthenticationMiddleware);

                // Now we know where the client is going to go we can authenticate them.
                // We allow the ocelot middleware to be overriden by whatever the
                // user wants
                /*if (pipelineConfiguration.AuthenticationMiddleware == null)
                {
                    app.UseAuthenticationMiddleware();
                }
                else
                {
                    app.Use(pipelineConfiguration.AuthenticationMiddleware);
                }*/

                // The next thing we do is look at any claims transforms in case this is important for authorization


                builder.UseMiddleware<CustomAuthentificationMiddleware>();
                builder.UseClaimsToClaimsMiddleware();

                builder.UseMiddleware<CustomAuthorizationMiddleware>();



                // Allow pre authorization logic. The idea being people might want to run something custom before what is built in.
                //app.UseIfNotNull(pipelineConfiguration.PreAuthorizationMiddleware);

                // Now we have authenticated and done any claims transformation we
                // can authorize the request
                // We allow the ocelot middleware to be overriden by whatever the
                // user wants
                /*if (pipelineConfiguration.AuthorizationMiddleware == null)
                {
                    app.UseAuthorizationMiddleware();
                }
                else
                {
                    app.Use(pipelineConfiguration.AuthorizationMiddleware);
                }*/

                // Now we can run the claims to headers transformation middleware
                builder.UseClaimsToHeadersMiddleware();

                // Allow the user to implement their own query string manipulation logic
                //app.UseIfNotNull(pipelineConfiguration.PreQueryStringBuilderMiddleware);

                // Now we can run any claims to query string transformation middleware
                builder.UseClaimsToQueryStringMiddleware();

                builder.UseClaimsToDownstreamPathMiddleware();

                // Get the load balancer for this request
                builder.UseLoadBalancingMiddleware();

                ISingletonNodeDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
                
                INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
                app.UseEndpoints(endpoints =>
                {
                    //endpoints.RegisterBackend(nodeManager,serviceProvider, env, databaseHandler, actionDescriptorCollectionProvider, Configuration, DatabaseEntityNamespaces);

                });


                // This takes the downstream route we retrieved earlier and replaces any placeholders with the variables that should be used
                builder.UseDownstreamUrlCreatorMiddleware();

                // Not sure if this is the best place for this but we use the downstream url
                // as the basis for our cache key.
                builder.UseOutputCacheMiddleware();

                //We fire off the request and set the response on the scoped data repo
                
                builder.UseHttpRequesterMiddleware();

            }).Wait();
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

            LogConfigurationModel initialLogConfigurationModel = new LogConfigurationModel();
            initialLogConfigurationModel.LogdateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.LogtimeFormat = "HH:mm:ss";
            initialLogConfigurationModel.LogLevel = General.MESSAGE_LEVEL.LEVEL_INFO;
            initialLogConfigurationModel.UserInterfaceDateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.UserInterfaceTimeFormat = "HH:mm:ss";

            DatabaseConfigurationModel initialNodeManagerDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialNodeManagerDatabaseConfigurationModel.Host = "localhost";
            initialNodeManagerDatabaseConfigurationModel.Port = 3306;
            initialNodeManagerDatabaseConfigurationModel.Database = "api_gateway";
            initialNodeManagerDatabaseConfigurationModel.User = "rest";
            initialNodeManagerDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialNodeManagerDatabaseConfigurationModel.Timeout = 300;
            initialNodeManagerDatabaseConfigurationModel.ConvertZeroDateTime = true;
            initialNodeManagerDatabaseConfigurationModel.OldGuids = true;

            AmpqConfigurationModel initialRabbitMqConfigurationModel = new AmpqConfigurationModel();
            initialRabbitMqConfigurationModel.Host = "localhost";
            initialRabbitMqConfigurationModel.Port = 5672;
            initialRabbitMqConfigurationModel.User = "webapi";
            initialRabbitMqConfigurationModel.Password = "admin1234";
            initialRabbitMqConfigurationModel.VirtualHost = "webapi";
            initialRabbitMqConfigurationModel.HeartBeatMs = 30000;

            WebApiConfigurationModel initialWebApiConfigurationModel = new WebApiConfigurationModel();
            initialWebApiConfigurationModel.Encoding = "UTF-8";

            //merged alle config in eine json namens: appservice.json
            MainConfigurationModel initialAppServiceConfigurationModel = new MainConfigurationModel(rootDir);
            initialAppServiceConfigurationModel.NodeManagerDatabaseConfigurationModel = initialNodeManagerDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.RabbitMqConfigurationModel = initialRabbitMqConfigurationModel;
            initialAppServiceConfigurationModel.ApiSecurityConfigurationModel = initialApiSecurityConfigurationModel;
            initialAppServiceConfigurationModel.WebApiConfigurationModel = initialWebApiConfigurationModel;
            initialAppServiceConfigurationModel.LogConfigurationModel = initialLogConfigurationModel;

            RoutesConfigurationModel initialRoutesConfigurationModel = new RoutesConfigurationModel();

            #endregion Initial Configurations
            return initialAppServiceConfigurationModel;
        }

        public override IConfiguration LoadConfiguration(IConfiguration previousConfig, IWebHostEnvironment env, IMainConfigurationModel cfg)
        {
            string basePath = Path.Combine(env.ContentRootPath, "Config");
            string appsettingsFile = Path.Combine(basePath, "appsettings.json");
            string routesFile = Path.Combine(basePath, "routes.json");
            if (!File.Exists(routesFile))
            {
                JsonHandler jsonHandler = new JsonHandler();
                FileStream fs = File.Create(routesFile);
                StreamWriter sw = new StreamWriter(fs);
                string defaultJsonStruct = jsonHandler.JsonSerialize(new RoutesConfigurationModel());
                sw.Write(defaultJsonStruct);
                sw.Close();
                sw.Dispose();
                fs.Close();
                fs.Dispose();
            }
            var configurationBuilder = new ConfigurationBuilder().
                SetBasePath(env.ContentRootPath).
                AddCustomWebApiConfig<AbstractConfigurationModel>(basePath, (AbstractConfigurationModel)cfg).
                AddJsonFile(routesFile, reloadOnChange: true, optional: false).
                AddJsonFile(appsettingsFile, reloadOnChange: true, optional: false).

                AddEnvironmentVariables();
            var config = configurationBuilder.AddConfiguration(previousConfig).Build();
            return config;
        }
    }


}
