using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiFunction.Web.Websocket.SignalR.HubClient
{
    public abstract class AbstractSignalRClient : ISignalRClient, IDisposable
    {
        public static readonly string HubConnectionSendMethod = "InvokeAsync";
        public IHubConnectionBuilder ConnectionBuilder { get; private set; }
        public HubConnection HubConnection { get; private set; }
        public AbstractSignalRClient(string url, Func<Task<string>> getWebApiTokenAction, Microsoft.AspNetCore.Http.Connections.HttpTransportType transportType, Microsoft.AspNetCore.Connections.TransferFormat transferFormat)
        {
            var userAgent = GetUserAgent();
            ConnectionBuilder = new HubConnectionBuilder()
                .WithUrl(url, options =>
                {
                    options.Headers.Add("User-Agent", userAgent);
                    options.AccessTokenProvider = () => getWebApiTokenAction();
                    options.Transports = transportType;
                    options.DefaultTransferFormat = transferFormat;
                })
                .AddJsonProtocol(options =>
                {

                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });
            HubConnection = ConnectionBuilder.Build();
        }
        public string GetUserAgent()
        {

            var currentAssembly = Assembly.GetExecutingAssembly();
            var hubAssembly = Assembly.GetAssembly(typeof(HubConnection));
            var libInfoHub = System.Diagnostics.FileVersionInfo.GetVersionInfo(hubAssembly.Location);
            var libInfoCurAss = System.Diagnostics.FileVersionInfo.GetVersionInfo(hubAssembly.Location);
            string userAgent = libInfoCurAss.ProductName + "@" + "" + libInfoCurAss.ProductVersion + "@" + libInfoHub.ProductName + "/" + libInfoHub.ProductVersion;
            return userAgent;
        }

        public async void Send(string methodName, object[] args, CancellationToken cancellationToken)
        {
            if (HubConnection != null && HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
            var methods = HubConnection.GetType().GetMethods();
            var targetMethod = methods.FirstOrDefault(m => (m.GetParameters().Length - 2) == args.Length && m.Name == HubConnectionSendMethod);//m.GetParameters().Length-2 Subtraktion von 2, da MethodName und CancelationToken keine Methodparams der SignalR Function sind
            if (targetMethod == null)
            {
                throw new InvalidOperationException("cant find a method of " + HubConnectionSendMethod + " with a signatur of " + args.Length + " params");
            }
            object[] methodParams = new object[args.Length + 2];
            methodParams[0] = methodName;
            int i = 1;//da Idx 0 == Methodname
            args.ToList().ForEach(x =>
            {
                methodParams[i] = x;
                i++;
            });
            methodParams[methodParams.Length - 1] = cancellationToken;

            targetMethod.Invoke(HubConnection, args);
        }

        public void Dispose()
        {
            if (HubConnection != null && HubConnection.State == HubConnectionState.Connected)
            {
                HubConnection.StopAsync();
            }
        }
    }
}
