using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;

namespace WebApiFunction.Application.Model.Database.MySql.Helix
{
    [Serializable]
    public class MessageRelationToAccountModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public
        [JsonPropertyName("message_uuid")]
        [DatabaseColumnProperty("message_uuid", MySqlDbType.String)]
        public Guid MessageUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("account_uuid")]
        [DatabaseColumnProperty("account_uuid", MySqlDbType.String)]
        public Guid AccountUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("message_sending_type_uuid")]
        [DatabaseColumnProperty("message_sending_type_uuid", MySqlDbType.String)]
        public Guid MessageSendingTypeUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public MessageRelationToAccountModel()
        {

        }
        #endregion
        #region Methods

        #endregion Methods
    }
}
