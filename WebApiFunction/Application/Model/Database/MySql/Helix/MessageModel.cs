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

namespace WebApiFunction.Application.Model.Database.MySQL.Helix
{
    [Serializable]
    public class MessageModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        /// <summary>
        /// Every Message must be a member of a conversation, because every sent msg is received by  a recipient, so two participant (sender + recipient) surrender to a conversation
        /// </summary>
        [JsonPropertyName("message_conversation_uuid")]
        [DatabaseColumnProperty("message_conversation_uuid", MySqlDbType.String)]
        public Guid MessageConversationUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("subject")]
        [DatabaseColumnProperty("subject", MySqlDbType.String)]
        public string Subject { get; set; }

        /// <summary>
        /// Message Content e.g. HTML+CSS (MIME), JSON, Plain-Text etc.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("content")]
        [DatabaseColumnProperty("content", MySqlDbType.Binary)]
        public byte[] Content { get; set; }

        /// <summary>
        /// !!!Wenn Virsu durch AV (nClam) erkannt wird, Inhalt sperren
        /// </summary>
        [JsonPropertyName("checked_for_virus")]
        [DatabaseColumnProperty("checked_for_virus", MySqlDbType.Bit, true)]
        public bool CheckForVirus { get; set; }

        /// <summary>
        /// !!!Wenn Violent-Speech durch KI erkannt wird, Inhalt sperren
        /// </summary>
        [JsonPropertyName("checked_for_violence")]
        [DatabaseColumnProperty("checked_for_violence", MySqlDbType.Bit, true)]
        public bool CheckForViolence { get; set; }

        /// <summary>
        /// !!!Wenn Sexual Content durch KI erkannt wird, Inhalt sperren
        /// </summary>
        [JsonPropertyName("checked_for_sexual_content")]
        [DatabaseColumnProperty("checked_for_sexual_content", MySqlDbType.Bit, true)]
        public bool CheckForSexualContent { get; set; }

        /// <summary>
        /// Speichert den KI Score für das Attachment
        /// </summary>
        [JsonPropertyName("checked_for_sexual_content_meta")]
        [DatabaseColumnProperty("checked_for_sexual_content_meta", MySqlDbType.String, true)]
        public string CheckForSexualContentMeta { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hash")]
        [DatabaseColumnProperty("hash", MySqlDbType.String)]
        public string Hash { get; set; }

        #region Ctor & Dtor
        public MessageModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
