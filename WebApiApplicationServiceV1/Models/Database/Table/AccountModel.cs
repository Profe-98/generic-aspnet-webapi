using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class AccountModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("uuid")]
        [DatabaseColumnPropertyAttribute("uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user")]
        [DatabaseColumnPropertyAttribute("user", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string User { get; set; }

        [JsonPropertyName("communication_medium_uuid")]
        [DatabaseColumnPropertyAttribute("communication_medium_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid CommunicationMediumUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public AccountModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        
        #endregion Methods
    }
}
