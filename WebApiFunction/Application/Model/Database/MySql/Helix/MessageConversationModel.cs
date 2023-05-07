using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApiFunction.Database;
using WebApiFunction.Configuration;
using MySql.Data.MySqlClient;

namespace WebApiFunction.Application.Model.Database.MySql.Helix
{
    [Serializable]
    public class MessageConversationModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("communication_medium_uuid")]
        [DatabaseColumnProperty("communication_medium_uuid", MySqlDbType.String)]
        public Guid CommunicationMediumUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("message_conversation_state_type_uuid")]
        [DatabaseColumnProperty("message_conversation_state_type_uuid", MySqlDbType.String)]
        public Guid MessageConversationStateTypeUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("display_conf_identifier")]
        [DatabaseColumnProperty("display_conf_identifier", MySqlDbType.String)]
        public string DisplayConfIdentifier { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hash")]
        [DatabaseColumnProperty("hash", MySqlDbType.String)]
        public string Hash { get; set; }

        [JsonPropertyName("scoped_conf_count")]
        [DatabaseColumnProperty("scoped_conf_count", MySqlDbType.Int32)]
        public int ScopedConfCount { get; set; }

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("message_queue_uuid")]
        [DatabaseColumnProperty("message_queue_uuid", MySqlDbType.String)]
        public Guid MessageQueueUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public MessageConversationModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
