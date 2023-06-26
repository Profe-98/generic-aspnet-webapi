using Microsoft.AspNetCore.Http.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Web.Websocket.SignalR.HubService.Attribute;

namespace WebApiFunction.Web.Websocket.SignalR.HubService
{
    public interface IHubService
    {
        public HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions { get; set; }
        public HubServiceRouteAttribute RouteAttribute{ get; }
        public MethodInfo[] HubMethods { get; }
    }
}
