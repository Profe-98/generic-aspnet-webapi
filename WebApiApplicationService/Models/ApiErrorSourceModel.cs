using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebApiApplicationService.Models
{
    [Serializable]
    public class ApiErrorSourceModel
    {

        [JsonPropertyName("pointer")]
        public string pointer { get; set; }
        [JsonPropertyName("parameter")]
        public string parameter { get; set; }
    }
}
