using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService.Attribute;

namespace Application.Shared.Kernel.Web.Websocket.SignalR.HubService
{
    public abstract class HubService:Hub, IHubService
    {
        public virtual HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions { get; set; }
        public HubServiceRouteAttribute RouteAttribute
        {
            get
            {
                return this.GetType().GetCustomAttribute<HubServiceRouteAttribute>();
            }
        }
        public MethodInfo[] HubMethods
        {
            get
            {
                return this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            }
        }
        public HubService() 
        {
            
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
    public class HubService<T> : Hub<T>,IHubService where T : class 
    {
        public virtual HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions { get; set; }
        public HubServiceRouteAttribute RouteAttribute
        {
            get
            {
                return this.GetType().GetCustomAttribute<HubServiceRouteAttribute>();
            }
        }
        public MethodInfo[] HubMethods
        {
            get
            {
                return this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            }
        }
        public HubService() {
        
        }
    }

}
