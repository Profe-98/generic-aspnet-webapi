using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Database;
using WebApiFunction.Web.Websocket.SignalR.HubService;
using WebApiFunction.Web.Websocket.SignalR.HubService.Attribute;

namespace JellyFishBackend.SignalR.Hub
{
    [HubServiceRoute("/messenger")]
    public class MessengerHub : HubService
    {
        private readonly IRabbitMqHandler _rabbitMqHandler;
        private readonly ICachingHandler _cachingHandler;
        private readonly ISingletonDatabaseHandler _singletonDatabaseHandler;
        public override HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions => new HttpConnectionDispatcherOptions
        {
            Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling,
            MinimumProtocolVersion = 1,
        };
        public MessengerHub(IRabbitMqHandler rabbitMqHandler,ICachingHandler cachingHandler, ISingletonDatabaseHandler singletonDatabaseHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
            _cachingHandler = cachingHandler;
            _singletonDatabaseHandler = singletonDatabaseHandler;

            //Entities: WebApiFunction.Application.Model.Database.MySql.Jellyfish
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToCaller(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToGroup(string user, string message)
        {
            await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
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
