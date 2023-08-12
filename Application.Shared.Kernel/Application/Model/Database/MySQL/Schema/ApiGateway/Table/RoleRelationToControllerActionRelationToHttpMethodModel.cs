using System;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class RoleRelationToControllerActionRelationToHttpMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnProperty("role_uuid", MySqlDbType.String)]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("controller_action_relation_to_http_method_uuid")]
        [DatabaseColumnProperty("controller_action_relation_to_http_method_uuid", MySqlDbType.String)]
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
