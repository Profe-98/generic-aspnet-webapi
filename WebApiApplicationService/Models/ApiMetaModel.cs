using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebApiApplicationService.Models
{
    [Serializable]
    public class ApiMetaModel
    {
        #region Private
        #endregion
        #region Public
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("message")]
        public string OptionalMessage { get; set; }
        /// <summary>
        /// Debug Message is only avaible in development-mode
        /// </summary>
        [JsonPropertyName("debug-message")]
        public string DebugMessage { get; set; }
        /// <summary>
        /// Debug Object is only avaible in development-mode
        /// </summary>
        [JsonPropertyName("debug-object")]
        public object DebugObject { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
