using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;

namespace WebApiFunction.Web.Authentification.JWT
{
    [Serializable]
    public class JWTHeaderModel
    {
        #region Public
        [JsonPropertyName("alg")]
        public string Algorithm { get; set; }

        [JsonPropertyName("typ")]
        public string Type { get; set; }
        #endregion Public
    }
}
