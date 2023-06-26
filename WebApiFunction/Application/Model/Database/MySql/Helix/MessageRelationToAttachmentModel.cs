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

namespace WebApiFunction.Application.Model.Database.MySQL.Helix
{
    [Serializable]
    public class MessageRelationToAttachmentModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("message_uuid")]
        [DatabaseColumnProperty("message_uuid", MySqlDbType.String)]
        public Guid MessageUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("message_attachment_uuid")]
        [DatabaseColumnProperty("message_attachment_uuid", MySqlDbType.String)]
        public Guid MessageAttachmentUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(5, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("file_extension")]
        [DatabaseColumnProperty("file_extension", MySqlDbType.String)]
        public string FileExtension { get; set; }

        #region Ctor & Dtor
        public MessageRelationToAttachmentModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
