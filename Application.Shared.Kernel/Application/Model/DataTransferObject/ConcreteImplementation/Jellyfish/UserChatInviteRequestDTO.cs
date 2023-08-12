using Application.Shared.Kernel.Configuration.Const;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{

    [Serializable]
    public class UserChatInviteRequestDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("chat_uuid")]
        public Guid ChatUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("target_user_uuid")]
        public Guid TargetUserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [JsonPropertyName("target_user_request_message")]
        public string TargetUserRequestMessage { get; set; }

        #region Ctor & Dtor
        public UserChatInviteRequestDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
