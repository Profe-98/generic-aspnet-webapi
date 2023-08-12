using System;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Application.Model.Database.MySQL;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table
{

    [Serializable]
    public class UserFriendsModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("user_uuid")]
        [DatabaseColumnProperty("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("friend_user_uuid")]
        [DatabaseColumnProperty("target_user_uuid", MySqlDbType.String)]
        public Guid TargetUserUuid { get; set; } = Guid.Empty;


        #region Ctor & Dtor
        public UserFriendsModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
