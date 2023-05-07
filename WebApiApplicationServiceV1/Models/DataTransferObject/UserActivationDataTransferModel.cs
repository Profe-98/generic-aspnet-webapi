using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApiApplicationService.Models.DataTransferObject
{
    public class UserActivationDataTransferModel
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(4, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(4, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("activation_code")]
        public string ActivationCode { get; set; }
    }
}
