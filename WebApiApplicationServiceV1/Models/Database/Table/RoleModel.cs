using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class RoleModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Name { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("description")]
        [DatabaseColumnPropertyAttribute("description", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Description { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("max_request_per_hour", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int MaxRequestPerHour { get; set; } = GeneralDefs.NotFoundResponseValue;

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("max_time_after_request_in_ms", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int MaxTimeAfterRequestInMs { get; set; } = GeneralDefs.NotFoundResponseValue;

        #region Ctor & Dtor
        public RoleModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
