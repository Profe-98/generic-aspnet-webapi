using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApiFunction.Configuration;

namespace WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer
{
    public class RegisterDataTransferModel : DataTransferModelAbstract
    {
        private string _email = null;
        private string _firstname = null;
        private string _lastname = null;
        private string _phone = null;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("first_name")]
        public string FirstName { get => _firstname; set => _firstname = TransformString(value); }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("last_name")]
        public string LastName { get => _lastname; set => _lastname = TransformString(value); }

        [DataType(DataType.EmailAddress, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("email")]
        public string EMail { get => _email; set => _email = TransformString(value); }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("phone")]
        public string Phone { get => _phone; set => _phone = TransformString(value); }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("activation_token")]
        public string ActivateTokenBase64 { get; set; } = string.Empty;

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("resend_code")]
        public bool ResendActivationCode { get; set; }


    }
}
