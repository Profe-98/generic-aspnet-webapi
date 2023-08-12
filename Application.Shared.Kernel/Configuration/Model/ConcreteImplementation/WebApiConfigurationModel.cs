using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class WebApiConfigurationModel : AbstractConfigurationModel
    {
        [JsonPropertyName("encoding")]
        public string Encoding { get; set; } = System.Text.Encoding.UTF8.EncodingName;
        [JsonPropertyName("node_uuid")]
        public Guid NodeUuid { get; set; } = Guid.Empty;

    }
}
