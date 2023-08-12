using InfluxDB.Client.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService.Attribute;

namespace Application.Shared.Kernel.Web.Websocket.SignalR.HubService
{
    public static class HubServiceExtensions
    {
        public static Dictionary<string,IHubService> RegisteredHubServices = new Dictionary<string, IHubService>();
        public static IEndpointRouteBuilder RegisterSignalRHubs(this IEndpointRouteBuilder builder, IServiceProvider serviceProvider)
        {
            Type builderType = typeof(HubEndpointRouteBuilderExtensions);
            if(builderType == null)
                throw new NotImplementedException();
            Assembly assembly = Assembly.GetCallingAssembly();
            if (assembly != null)
            {
                var types = assembly.GetTypes()?.ToList();
                if (types != null && types.Count != 0)
                {
                    foreach (Type type in types)
                    {
                        var attr = type.GetCustomAttribute<HubServiceRouteAttribute>();
                        if (attr != null && type.GetInterface(nameof(IHubService))!=null)
                        {
                            var ctors = type.GetConstructors();
                            if (ctors.Length > 1)
                                throw new InvalidOperationException("multi-ctors are currently not supported");
                            var ctor = ctors.First();
                            var ctorParams = ctor.GetParameters();
                            object[] ctorArgsFilled = new object[ctorParams.Length];

                            int i = 0;
                            foreach(var param in ctorParams)
                            {
                                var foundService = serviceProvider.GetService(param.ParameterType);
                                if (foundService == null)
                                    throw new NotFoundException(""+param.Name+" not found in given ServiceProvider");

                                ctorArgsFilled[i] = foundService;
                                i++;
                            }

                            var hubServiceInstance = (IHubService)Activator.CreateInstance(type, args: ctorArgsFilled);
                            if(hubServiceInstance==null)
                                throw new NotImplementedException();  
                            var methodParams = new List<Type> { typeof(IEndpointRouteBuilder), typeof(string) };
                            if(hubServiceInstance.HttpConnectionDispatcherOptions!= null)
                            {
                                methodParams.Add(typeof(Action<HttpConnectionDispatcherOptions>));
                            }
                            MethodInfo mapHubMethod = builderType.GetMethod("MapHub", BindingFlags.Public | BindingFlags.Static, methodParams.ToArray());
                            if (mapHubMethod == null)
                                throw new NotImplementedException();
                            mapHubMethod =mapHubMethod.MakeGenericMethod(type);
                            var callParams = new List<object> { builder, attr.Route ?? ("/" + type.Name.ToLower()) };
                            if (hubServiceInstance.HttpConnectionDispatcherOptions != null)
                            {
                                
                                var action = new Action<HttpConnectionDispatcherOptions>(x => { });
                                action(hubServiceInstance.HttpConnectionDispatcherOptions);

                                callParams.Add(action);
                            }
                            mapHubMethod.Invoke(builder, callParams.ToArray());
                            RegisteredHubServices.Add(type.Name.ToLower(),hubServiceInstance);
                        }
                    }
                }
            }


            return builder;
        }
    }
}
