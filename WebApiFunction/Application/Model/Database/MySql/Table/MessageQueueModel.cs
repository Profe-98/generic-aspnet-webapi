using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApiFunction.Configuration;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;

namespace WebApiFunction.Application.Model.Database.MySql.Table
{
    [Serializable]
    public class MessageQueueModel : AbstractModel
    {
        #region Private
        private string _inboxFolder = null;
        private string _proceddedFolder = null;
        #endregion Private
        #region Public
        #endregion Public

        /// <summary>
        /// The related system_message_user_uuid where the queue is related
        /// </summary>
        [JsonPropertyName("system_message_user_uuid")]
        [DatabaseColumnPropertyAttribute("system_message_user_uuid", MySqlDbType.String)]
        public Guid SystemMessageUserUuid { get; set; } = Guid.Empty;

        /// <summary>
        /// The Parent Message Queue, when queue has parent, it couldnt have a system_message_user_uuid
        /// </summary>
        [JsonPropertyName("message_queue_uuid")]
        [DatabaseColumnProperty("message_queue_uuid", MySqlDbType.String)]
        public Guid MessageQueueUuid { get; set; } = Guid.Empty;

        /// <summary>
        /// The Queue Displayname
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySqlDbType.String)]
        public string QueueName { get; set; }

        /// <summary>
        /// The initial msg when a ticket is newly created, send to recipient
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("initial_message")]
        [DatabaseColumnPropertyAttribute("initial_message", MySqlDbType.String)]
        public string InitialMessage { get; set; }

        /// <summary>
        /// The Inbox folder where the source mails 
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("inbox_folder")]
        [DatabaseColumnPropertyAttribute("inbox_folder", MySqlDbType.String)]
        public string InboxFolder { get => string.IsNullOrEmpty(_inboxFolder) ? _inboxFolder = "INBOX" : _inboxFolder; set => _inboxFolder = value; }

        /// <summary>
        /// The Folder after parsing, so the place where the mail should be moved
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("procedded_folder")]
        [DatabaseColumnPropertyAttribute("procedded_folder", MySqlDbType.String)]
        public string ProceddedFolder { get => _proceddedFolder; set => _proceddedFolder = value; }

        /// <summary>
        /// The source emails would be deleted after parsing (parsing=put mail data into database)
        /// </summary>
        [JsonPropertyName("delete_mails_after_processing")]
        [DatabaseColumnPropertyAttribute("delete_mails_after_processing", MySqlDbType.Bit)]
        public bool DeleteMailsAfterProcessing { get; set; } = false;

        /// <summary>
        /// Messages they are move to junk will be flagged as deleted
        /// </summary>
        [JsonPropertyName("is_junk")]
        [DatabaseColumnPropertyAttribute("is_junk", MySqlDbType.Bit)]
        public bool IsJunk { get; set; } = false;


        /// <summary>
        /// The Max Attachment-File Size of an Message Attachment in kilobyte (kB 10^3)
        /// </summary>
        [JsonPropertyName("max_attachment_file_size")]
        [DatabaseColumnPropertyAttribute("max_attachment_file_size", MySqlDbType.Int32)]
        public int MaxAttachmentFileSize { get; set; } = GeneralDefs.NotFoundResponseValue;


        /// <summary>
        /// The allowed File Extensions for Message Attachments
        /// </summary>
        [JsonPropertyName("allowed_attachment_file_extensions")]
        [DatabaseColumnPropertyAttribute("allowed_attachment_file_extensions", MySqlDbType.JSON)]
        public List<MessageQueueFileExtensions_SubModel> MaxAllowedFileExtensions { get; set; } = null;


        /// <summary>
        /// Gets or Sets the state of checking attachment of viruses etc.
        /// </summary>
        [JsonPropertyName("attachment_av_check")]
        [DatabaseColumnPropertyAttribute("attachment_av_check", MySqlDbType.Bit)]
        public bool AttachmentAvCheck { get; set; } = false;

        /// <summary>
        /// Procedded folder muss be given to move parsed emails to them, when it is not given the mails would be only marked as readed
        /// </summary>
        [JsonIgnore]
        public bool HasIndividualFolders
        {
            get
            {
                return !string.IsNullOrEmpty(ProceddedFolder);
            }
        }

        /// <summary>
        /// When Queue is a child of a parent message_queue_uuid, then it cant store data of a system_message_user_uuid
        /// </summary>
        [JsonIgnore]
        public bool HasHirarchicalError
        {
            get
            {
                return MessageQueueUuid != Guid.Empty && MessageQueueUuid != Guid.Empty;
            }
        }

        [JsonIgnore]
        public bool HasFileExtensionRestriction
        {
            get
            {
                return MaxAllowedFileExtensions != null && MaxAllowedFileExtensions.Count != 0;
            }
        }
        [JsonIgnore]
        public bool HasMaxFileSizeRestriction
        {
            get
            {
                return MaxAttachmentFileSize != GeneralDefs.NotFoundResponseValue;
            }
        }

        #region Ctor & Dtor
        public MessageQueueModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
