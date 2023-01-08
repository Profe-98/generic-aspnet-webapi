using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class RoleRelationToControllerActionRelationToHttpMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnPropertyAttribute("role_uuid",MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("controller_action_relation_to_http_method_uuid")]
        [DatabaseColumnPropertyAttribute( "controller_action_relation_to_http_method_uuid",MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ControllerActionRelationToHttpMethodUuid { get; set; } = Guid.Empty;
        #endregion Public


        #region Ctor & Dtor
        public RoleRelationToControllerActionRelationToHttpMethodModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
