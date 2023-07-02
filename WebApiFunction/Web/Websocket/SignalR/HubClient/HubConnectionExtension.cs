using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Web.Websocket.SignalR.HubClient
{
    public static class HubConnectionExtension
    {
        public static async Task<bool> ConnectWithRetryAsync(this HubConnection connection, int delayBetweenReinitConnectionInMs, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(delayBetweenReinitConnectionInMs);
                }
            }
        }
    }
}
