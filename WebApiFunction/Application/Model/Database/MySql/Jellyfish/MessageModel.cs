using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MimeKit;
using WebApiFunction.Configuration;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class MessageModel : AbstractModel
    {
        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("chat_uuid")]
        [DatabaseColumnProperty("chat_uuid", MySqlDbType.String)]
        public Guid ChatUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("message_owner")]
        [DatabaseColumnProperty("message_owner", MySqlDbType.String)]
        public Guid MessageOwner { get; set; } = Guid.Empty;

        /// <summary>
        /// Message Content e.g. HTML+CSS (MIME), JSON, Plain-Text etc.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(65535, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("text")]
        [DatabaseColumnProperty("text", MySqlDbType.Text)]
        public string Text { get; set; }

        /// <summary>
        /// For Pictures, Videos etc.
        /// </summary>
        /// 
        [JsonPropertyName("binary_content")]
        [DatabaseColumnProperty("binary_content", MySqlDbType.Binary)]
        public byte[] ?Content { get; set; } = null;
    }
}
