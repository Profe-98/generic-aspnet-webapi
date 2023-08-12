using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Web.Websocket.SignalR.HubClient
{
    public static class HubConnectionExtension
    {
        public static async Task<bool> ConnectWithRetryAsync(this HubConnection connection, int delayBetweenReinitConnectionInMs, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.

            while (connection.State != HubConnectionState.Connected &&!token.IsCancellationRequested)
            {
                try
                {
                    await connection.StartAsync(token);
                }
                catch when (token.IsCancellationRequested)
                {

                }
                catch
                {
                    await Task.Delay(delayBetweenReinitConnectionInMs);
                }
            }
            return true;
        }
    }
}
