using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApiFunction.Web.Websocket.SignalR.HubService;
using WebApiFunction.Web.Websocket.SignalR.HubService.Attribute;

namespace WebApiApplicationServiceV2.SignalR.Hubs
{
    [HubServiceRoute("/testhub")]
    public class TestHub : HubService
    {
        public override HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions => new HttpConnectionDispatcherOptions 
        { 
            Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling,
            MinimumProtocolVersion=1,
        };
        public TestHub() 
        {
        }
        public async Task SendMessage(string user, string message)
    => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.SendAsync("ReceiveMessage", user, message);

        public async Task SendMessageToGroup(string user, string message)
            => await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
    }
}
