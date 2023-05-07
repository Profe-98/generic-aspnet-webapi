using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ConnectionstringValuesModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public


        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(96, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("valueName")]
        [DatabaseColumnPropertyAttribute("value_name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string ValueName{ get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("type")]
        [DatabaseColumnPropertyAttribute("type", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Type { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("description")]
        [DatabaseColumnPropertyAttribute("description", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Description { get; set; }

        [JsonPropertyName("mandatory")]
        [DatabaseColumnPropertyAttribute("mandatory", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public bool Mandatory { get; set; }

        #region Ctor & Dtor
        public ConnectionstringValuesModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
