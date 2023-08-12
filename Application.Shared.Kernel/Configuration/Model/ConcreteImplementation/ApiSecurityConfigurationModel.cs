using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class ApiSecurityConfigurationModel : AbstractConfigurationModel
    {
        public class JsonWebTokenModel
        {
            [JsonPropertyName("jwt_bearer_secret_string")]
            public string JwtBearerSecretStr { get; set; } = "this is my custom Secret key for authnetication";
            [JsonIgnore]
            public byte[] JwtBearerSecretByteArr
            {
                get
                {
                    return JwtBearerSecretStr != null ? Encoding.UTF8.GetBytes(JwtBearerSecretStr) : null;
                }
            }
        }
        [JsonPropertyName("api_content_type")]
        public string ApiContentType { get; set; } = GeneralDefs.ApiContentType;
        public class SiteProtectModel
        {
            [JsonPropertyName("max_http_request_uri_len")]
            public int MaxHttpRequUriLen { get; set; }
            [JsonPropertyName("max_http_header_field_len")]
            public int MaxHttpHeaderFieldLen { get; set; }
            [JsonPropertyName("max_http_header_field_value_len")]
            public int MaxHttpHeaderFieldValueLen { get; set; }
            [JsonPropertyName("max_http_content_len")]
            public int MaxHttpContentLen { get; set; }
        }
        [JsonPropertyName("jwt")]
        public JsonWebTokenModel Jwt { get; set; } = new JsonWebTokenModel();
        [JsonPropertyName("site_protect")]
        public SiteProtectModel SiteProtect { get; set; }
    }
}
