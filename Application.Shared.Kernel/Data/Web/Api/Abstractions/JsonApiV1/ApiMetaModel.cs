using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiMetaModel : BaseModel
    {
        #region Private
        #endregion
        #region Public
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("message")]
        public string ?OptionalMessage { get; set; }
        /// <summary>
        /// Debug Message is only avaible in development-mode
        /// </summary>
        [JsonPropertyName("debug-message")]
        public string ?DebugMessage { get; set; }
        /// <summary>
        /// Debug Object is only avaible in development-mode
        /// </summary>
        [JsonPropertyName("debug-object")]
        public object ?DebugObject { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
