using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WebApiApplicationService.Models.InternalModels
{
    [Serializable]
    public class CacheDescriptorData
    {
        /// <summary>
        /// Net Type as String for Searching in BackendTablesEx
        /// </summary>
        [JsonPropertyName("entity-net-type")]
        public string EntityType { get; set; }

        /// <summary>
        /// Entity 
        /// </summary>
        [JsonPropertyName("entity")]
        public ApiDataModel Entity { get; set; }

        /// <summary>
        /// Entity 
        /// </summary>
        [JsonPropertyName("agg-entity")]
        public List<Guid> AggregateEntities { get; set; }



    }
}
