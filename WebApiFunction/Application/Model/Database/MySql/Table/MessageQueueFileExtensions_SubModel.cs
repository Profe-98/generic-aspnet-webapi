using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WebApiFunction.Application.Model.Database.MySql.Table
{
    public class MessageQueueFileExtensions_SubModel
    {
        [JsonPropertyName("extension")]
        public string Extension { get; set; }
    }
}
