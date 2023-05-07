using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using MimeKit;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApiApplicationServiceV2;
using WebApiApplicationService;

namespace WebApiApplicationServiceV2_Test.Tests.IntegrationTests
{
    [Collection("signalr-tests")]
    public class SignalRTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        HubConnection connection;
        public SignalRTest()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://11.0.0.200:5010/testhub")
                .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {

            });

        }
        [Fact]
        public async void SendTest()
        {

            try
            {
                await connection.InvokeAsync("SendMessage",
                    "arg1", "arg2");
            }
            catch (Exception ex)
            {

            }
        }
    }

}
