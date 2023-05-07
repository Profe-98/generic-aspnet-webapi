using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{

    [Serializable]
    public class OperatingSystemModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(90, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string OperatingSystem { get; set; }

        #region Ctor & Dtor
        public OperatingSystemModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
