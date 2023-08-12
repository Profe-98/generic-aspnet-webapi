using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{
    [Serializable]
    public class MessageDTO : DataTransferModelAbstract
    {
        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("chat_uuid")]
        public Guid ChatUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("message_owner")]
        public Guid MessageOwner { get; set; } = Guid.Empty;

        /// <summary>
        /// Message Content e.g. HTML+CSS (MIME), JSON, Plain-Text etc.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(65535, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// For Pictures, Videos etc.
        /// </summary>
        [JsonPropertyName("binary_content")]
        public byte[] Content { get; set; }
    }
}
