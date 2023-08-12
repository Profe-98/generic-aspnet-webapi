using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiRelationshipModel : BaseModel
    {
        #region Private
        #endregion
        #region Public
        public ApiLinkModel Links { get; set; }
        //Data as a single object of Type ApiDataModel or List<ApiDataModel>
        [JsonPropertyName("data")]
        public object Data { get; set; } //data beinhaltet nur die ausgefüllten attribute= type+id
        [JsonPropertyName("meta")]
        public ApiMetaModel Meta { get; set; }
        #endregion

        #region Ctor
        #endregion

    }
}
