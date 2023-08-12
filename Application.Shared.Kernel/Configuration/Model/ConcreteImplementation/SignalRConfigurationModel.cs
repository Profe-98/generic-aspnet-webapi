using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class SignalRConfigurationModel : AbstractConfigurationModel
    {
        [JsonPropertyName("use_local_hub")]
        public bool UseLocalHub { get; set; } = false;
        [JsonPropertyName("debug_errors_detailed_clientside")]
        public bool DebugErrorsDetailedClientside { get; set; } = false;
        [JsonPropertyName("timeout_sec")]
        public int TimoutTimeSec { get; set; } = 15;
        [JsonPropertyName("keepalive_timemout")]
        public int KeepaliveTimeout { get; set; } = 15;
        [JsonPropertyName("client_timeout_sec")]
        public int ClientTimeoutSec { get; set; } = 30;
        [JsonPropertyName("handshake_timeout")]
        public int HandshakeTimeout { get; set; } = 5;
        [JsonPropertyName("maximum_parallel_invocations_per_per_client")]
        public int MaximumParallelInvocationsPerClient { get; set; } = 1;



    }
}
