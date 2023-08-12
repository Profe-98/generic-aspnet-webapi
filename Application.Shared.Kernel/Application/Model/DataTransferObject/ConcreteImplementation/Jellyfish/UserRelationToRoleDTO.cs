using System;
using System.Text.Json.Serialization;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{

    [Serializable]
    public class UserRelationToRoleDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("role_uuid")]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        public UserModel UserModel { get; set; }

        #region Ctor & Dtor
        public UserRelationToRoleDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
