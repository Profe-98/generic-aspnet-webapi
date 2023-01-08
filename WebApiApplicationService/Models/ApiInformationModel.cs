using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebApiApplicationService.Models
{
    [Serializable]
    public class ApiInformationModel
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
