using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Web.Websocket.SignalR.HubService.Attribute;

namespace WebApiFunction.Web.Websocket.SignalR.HubService
{
    public abstract class HubService:Hub
    {
        public virtual HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions { get; private set; }
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
}
