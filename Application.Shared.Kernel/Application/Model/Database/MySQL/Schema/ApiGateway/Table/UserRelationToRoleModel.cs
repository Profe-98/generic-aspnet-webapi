using System;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{

    [Serializable]
    public class UserRelationToRoleModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        [DatabaseColumnProperty("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnProperty("role_uuid", MySqlDbType.String)]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        public UserModel UserModel { get; set; }

        #region Ctor & Dtor
        public UserRelationToRoleModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
