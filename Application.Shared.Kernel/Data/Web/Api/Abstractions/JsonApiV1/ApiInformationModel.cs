using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiInformationModel : BaseModel
    {
        #region Private
        #endregion
        #region Public
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("company")]
        public string Company { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonPropertyName("copyright")]
        public string Copyright { get; set; }
        [JsonPropertyName("use")]
        public string Use { get; set; }
        [JsonPropertyName("rfc")]
        public string Rfc { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
