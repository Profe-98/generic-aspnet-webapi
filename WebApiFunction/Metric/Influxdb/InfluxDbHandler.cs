using System;
using InfluxDB.Client;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WebApiFunction.Metric.Influxdb
{
    public class InfluxDbHandler : IInfluxDbHandlerInterface
    {
        private int _pingFailureCounter = 0;
        private DateTime _pingNextTryBlockingTime = DateTime.MinValue;
        private readonly string _influxDbHostUrl;
        private readonly uint _influxDbHostPort;
        private readonly string _token;

        public string FullyHostEndpoint
        {
            get
            {
                return _influxDbHostUrl + ":" + _influxDbHostPort;
            }
        }

        public InfluxDbHandler(IConfiguration configuration)
        {
            _token = configuration.GetValue<string>("InfluxDB:Token");
            _influxDbHostUrl = configuration.GetValue<string>("InfluxDB:HostUrl");
            _influxDbHostPort = configuration.GetValue<uint>("InfluxDB:Port");
        }

        public async void Write(Action<WriteApi> action)
        {
            var client = InfluxDBClientFactory.Create(FullyHostEndpoint, _token);
            bool ping = await client.PingAsync();
            var buckets = client.GetBucketsApi();
            var bucket = await buckets.FindBucketByNameAsync("api_gateway");
            var write = client.GetWriteApi();
            action(write);
        }

        public async Task<T> Read<T>(Func<QueryApi, Task<T>> action)
        {
            using var client = InfluxDBClientFactory.Create(FullyHostEndpoint, _token);
            var query = client.GetQueryApi();
            return await action(query);
        }

        public async Task<bool> IsReachable()
        {
            if (DateTime.Now < _pingNextTryBlockingTime)
            {
                return false;
            }
            else
            {
                _pingNextTryBlockingTime = DateTime.MinValue;
            }
            using (Ping ping = new Ping())
            {
                IPAddress ip = ResolveUrlToIp(new Uri(FullyHostEndpoint));

                var reply = await ping.SendPingAsync(ip, 50);
                bool response = reply != null && reply.Status == IPStatus.Success;
                if(response)
                {

                    try
                    {
                        using (TcpClient client = new TcpClient())
                        {
                            client.BeginConnect(ip, (int)_influxDbHostPort, (x) => { 
                                
                            }, null);
                            response = client.Connected;
                        }
                    }
                    catch (Exception ex)
                    {
                        response = false;
                    }
                }
                if (!response)
                    _pingFailureCounter++;

                if (_pingFailureCounter >= 3)
                {
                    _pingNextTryBlockingTime = DateTime.Now.AddMinutes(5);
                    _pingFailureCounter = 0;
                }

                return response;
            }
        }

        private IPAddress ResolveUrlToIp(Uri uri)
        {
            return Dns.GetHostAddresses(uri.Host)?.ToList().FirstOrDefault();
        }

        public InfluxDBClient GetClientInstance()
        {

            return InfluxDBClientFactory.Create(FullyHostEndpoint, _token);
        }
    }
}
