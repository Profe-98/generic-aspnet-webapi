using System;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{

    [Serializable]
    public class UserFriendshipRequestAcceptDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_friendship_request_uuid")]
        public List<Guid> UserFriendshipRequestUuids { get; set; } = null;

        #region Ctor & Dtor
        public UserFriendshipRequestAcceptDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
