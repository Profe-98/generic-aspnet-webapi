using Application.Shared.Kernel.Configuration.Const;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{
    public class UserActivationDataTransferModel
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(4, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(4, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("activation_code")]
        public string ActivationCode { get; set; }
    }
}
