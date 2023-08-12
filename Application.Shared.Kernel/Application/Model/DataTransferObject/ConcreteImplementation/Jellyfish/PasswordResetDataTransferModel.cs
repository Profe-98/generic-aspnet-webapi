using Application.Shared.Kernel.Configuration.Const;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{
    public class PasswordResetDataTransferModel : DataTransferModelAbstract
    {
        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        public string Password { get; set; }


        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password_reset_code")]
        public string PasswordResetCode { get; set; }


    }
    public class PasswordResetConfirmationCodeDataTransferModel : DataTransferModelAbstract
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password_reset_code")]
        public string PasswordResetCode { get; set; }
    }
}
