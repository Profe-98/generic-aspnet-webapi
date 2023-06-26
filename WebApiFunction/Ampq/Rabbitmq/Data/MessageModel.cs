using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using WebApiFunction.Application.Model.Database.MySQL;

namespace WebApiFunction.Ampq.Rabbitmq.Data
{
    [Serializable]
    public class MessageModel : AbstractModel
    {
        [JsonPropertyName("node-id")]
        public Guid NodeId { get; set; }
        [JsonPropertyName("message-timestamp")]
        public long CreateTimestamp { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("data")]
        public string DataSerialized { get; set; }
        [JsonIgnore]
        public object DataDeserialized { get; set; }
        [JsonPropertyName("data-type")]
        public string DataType { get; set; }
        //sha256
        [JsonPropertyName("data-hash")]
        public string DataHash { get; set; }
        [JsonPropertyName("data-hash-algo")]
        public string DataHashAlgo { get; set; }
    }
}
