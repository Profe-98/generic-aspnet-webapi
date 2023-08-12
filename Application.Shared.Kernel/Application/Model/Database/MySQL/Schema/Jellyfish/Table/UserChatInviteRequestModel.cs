using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table
{

    [Serializable]
    public class UserChatInviteRequestModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        [DatabaseColumnProperty("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("chat_uuid")]
        [DatabaseColumnProperty("chat_uuid", MySqlDbType.String)]
        public Guid ChatUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("target_user_uuid")]
        [DatabaseColumnProperty("target_user_uuid", MySqlDbType.String)]
        public Guid TargetUserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [JsonPropertyName("target_user_request_message")]
        [DatabaseColumnProperty("target_user_request_message", MySqlDbType.String)]
        public string TargetUserRequestMessage { get; set; }

        #region Ctor & Dtor
        public UserChatInviteRequestModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
