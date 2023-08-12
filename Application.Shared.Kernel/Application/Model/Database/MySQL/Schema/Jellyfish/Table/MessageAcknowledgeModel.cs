using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table
{
    [Serializable]
    public class MessageAcknowledgeModel : AbstractModel
    {

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("message_uuid")]
        [DatabaseColumnProperty("message_uuid", MySqlDbType.String)]
        public Guid MessageUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("user_uuid")]
        [DatabaseColumnProperty("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

    }
}
