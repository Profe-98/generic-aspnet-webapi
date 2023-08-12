using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Web.Websocket.SignalR.HubClient
{
    public interface ISignalRClient
    {
        public void Send(string methodName, object[] args, CancellationToken cancellationToken);
        public string GetUserAgent();
    }
}
