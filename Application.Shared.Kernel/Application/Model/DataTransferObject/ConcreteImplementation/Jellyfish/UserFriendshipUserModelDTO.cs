using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{

    [Serializable]
    public class UserFriendshipUserModelDTO : UserDTO
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("uuid")]
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        public override Guid Uuid { get => base.Uuid; set => base.Uuid = value; }
        [JsonPropertyName("target_user_uuid")]
        [DatabaseColumnProperty("target_user_uuid", MySqlDbType.String)]
        public Guid TargetUserUuid { get; set; } = Guid.Empty;
        [JsonPropertyName("user_uuid")]
        [DatabaseColumnProperty("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [DatabaseColumnProperty("target_user_request_message", MySqlDbType.String)]
        [JsonPropertyName("target_user_request_message")]
        public string TargetUserRequestMessage { get; set; }

        #region Ctor & Dtor
        [JsonConstructor()]
        public UserFriendshipUserModelDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
