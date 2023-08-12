using System;
using System.Threading.Tasks;
using InfluxDB.Client;

namespace Application.Shared.Kernel.Infrastructure.Metric.Influxdb
{
    public interface IInfluxDbHandlerInterface
    {
        public InfluxDBClient GetClientInstance();
        public Task<bool> IsReachable();
        public void Write(Action<WriteApi> action);
        public Task<T> Read<T>(Func<QueryApi, Task<T>> action);
    }
}
