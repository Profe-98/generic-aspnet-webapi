using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class AmpqConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public uint Port { get; set; }
        [JsonPropertyName("user")]
        public string User { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("virtual_host")]
        public string VirtualHost { get; set; }
        [JsonPropertyName("heartbeat_ms")]
        public int HeartBeatMs { get; set; } = 30000;
    }
}
