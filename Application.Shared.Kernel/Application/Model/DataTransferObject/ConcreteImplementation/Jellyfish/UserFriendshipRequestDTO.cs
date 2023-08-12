using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{

    [Serializable]
    public class UserFriendshipRequestDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("target_user_uuid")]
        public Guid TargetUserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [JsonPropertyName("target_user_request_message")]
        public string TargetUserRequestMessage { get; set; }

        #region Ctor & Dtor
        [JsonConstructor()]
        public UserFriendshipRequestDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
