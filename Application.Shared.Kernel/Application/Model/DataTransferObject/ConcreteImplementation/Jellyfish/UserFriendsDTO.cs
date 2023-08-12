using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{

    [Serializable]
    public class UserFriendsDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("friend_user_uuid")]
        public Guid TargetUserUuid { get; set; } = Guid.Empty;


        #region Ctor & Dtor
        public UserFriendsDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
