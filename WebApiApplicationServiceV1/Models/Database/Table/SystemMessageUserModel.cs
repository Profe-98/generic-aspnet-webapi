using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace WebApiApplicationService.Models.Database
{
    /// <summary>
    /// For internal ticket system
    /// </summary>
    [Serializable]
    public class SystemMessageUserModel : AbstractModel
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user")]
        [DatabaseColumnPropertyAttribute("user", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string User { get; set; }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        [DatabaseColumnPropertyAttribute("password", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Password { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("communication_medium_uuid",MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid CommunicationMediumUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public SystemMessageUserModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
