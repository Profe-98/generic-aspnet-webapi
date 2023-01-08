using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{

    [Serializable]
    public class UserRelationToRoleModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        [DatabaseColumnPropertyAttribute("user_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnPropertyAttribute("role_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
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
