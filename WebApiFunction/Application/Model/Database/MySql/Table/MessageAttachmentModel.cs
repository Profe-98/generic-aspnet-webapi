using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;
using WebApiFunction.Configuration;

namespace WebApiFunction.Application.Model.Database.MySql.Table
{
    /// <summary>
    /// INFO: Mail attachments would be stored on filesystem with the uuid in filename unindependent from extension + the path is given by environment config and not separated stored in database
    /// </summary>
    [Serializable]
    public class MessageAttachmentModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public


        /// <summary>
        /// Source Msg mit der das Attachment ins System gekommen ist
        /// </summary>
        [JsonPropertyName("first_source_message_uuid")]
        [DatabaseColumnProperty("first_source_message_uuid", MySqlDbType.String)]
        public Guid FirstSourceMessageUuid { get; set; } = Guid.Empty;

        /// <summary>
        /// Autor, Creator des Attachments bzw. der Account der die Ressource als erstes angelegt hat (Absender von first_source_message_uuid)
        /// </summary>
        [JsonPropertyName("account_uuid")]
        [DatabaseColumnPropertyAttribute("account_uuid", MySqlDbType.String)]
        public Guid AccountUuid { get; set; } = Guid.Empty;

        /// <summary>
        /// Für Abgleich, nicht das Attachments obwohl FileHash gleich mehrfach gespeichert werden
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hash")]
        [DatabaseColumnPropertyAttribute("hash", MySqlDbType.String)]
        public string Hash { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(5, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("file_extension")]
        [DatabaseColumnPropertyAttribute("file_extension", MySqlDbType.String)]
        public string FileExtension { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// !!!Wenn Virsu durch AV (nClam) erkannt wird, Inhalt sperren
        /// </summary>
        [JsonPropertyName("checked_for_virus")]
        [DatabaseColumnPropertyAttribute("checked_for_virus", MySqlDbType.Bit, true)]
        public bool CheckForVirus { get; set; }

        /// <summary>
        /// !!!Wenn Violent-Speech durch KI erkannt wird, Inhalt sperren
        /// </summary>
        [JsonPropertyName("checked_for_violence")]
        [DatabaseColumnPropertyAttribute("checked_for_violence", MySqlDbType.Bit, true)]
        public bool CheckForViolence { get; set; }

        /// <summary>
        /// !!!Wenn Sexual Content durch KI erkannt wird, Inhalt sperren
        /// </summary>
        [JsonPropertyName("checked_for_sexual_content")]
        [DatabaseColumnPropertyAttribute("checked_for_sexual_content", MySqlDbType.Bit, true)]
        public bool CheckForSexualContent { get; set; }

        /// <summary>
        /// Speichert den KI Score für das Attachment
        /// </summary>
        [JsonPropertyName("checked_for_sexual_content_meta")]
        [DatabaseColumnPropertyAttribute("checked_for_sexual_content_meta", MySqlDbType.String, true)]
        public string CheckForSexualContentMeta { get; set; }

        [JsonIgnore]
        public bool FileExists
        {
            get
            {
                if (Path != null)
                {
                    return File.Exists(Path);
                }
                return false;
            }
        }
        #region Ctor & Dtor
        public MessageAttachmentModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
