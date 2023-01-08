using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebApiApplicationService.Models
{
    [Serializable]
    public class ApiRelationshipModel
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
