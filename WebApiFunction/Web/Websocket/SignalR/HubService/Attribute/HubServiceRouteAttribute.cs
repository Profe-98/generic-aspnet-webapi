using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Web.Websocket.SignalR.HubService.Attribute
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =true)]
    public class HubServiceRouteAttribute : System.Attribute
    {
        public string Route { get;private set; }
        public HubServiceRouteAttribute(string route = null)
        {
            this.Route= route;
        }
    }

    
}
