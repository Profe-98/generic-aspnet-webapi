using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiErrorSourceModel : BaseModel
    {

        [JsonPropertyName("pointer")]
        public string pointer { get; set; }
        [JsonPropertyName("parameter")]
        public string parameter { get; set; }
    }
}
