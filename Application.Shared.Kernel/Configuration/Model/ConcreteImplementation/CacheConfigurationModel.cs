using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;
using Application.Shared.Kernel.Utility;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class CacheConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("hosts")]
        public CacheHostConfigurationModel[] Hosts { get; set; }

    }
    public class CacheHostConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public uint Port { get; set; }
        [JsonPropertyName("timeout_s")]
        public uint Timeout { get; set; }
        [JsonPropertyName("user")]
        public string User { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonIgnore]
        public IPAddress IPAddr
        {
            get
            {
                return Host != null ? Utils.ParseEx(Host) : null;
            }
        }
        [JsonIgnore]
        public EndPoint EndPoint
        {
            get
            {
                return Port != 0 && IPAddr != null ? new IPEndPoint(IPAddr, (int)Port) : null;
            }
        }

    }
}
