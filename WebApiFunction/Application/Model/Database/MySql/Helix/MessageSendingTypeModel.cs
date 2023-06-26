using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;
using WebApiFunction.Configuration;

namespace WebApiFunction.Application.Model.Database.MySQL.Helix
{
    [Serializable]
    public class MessageSendingTypeModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("uuid")]
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("description")]
        [DatabaseColumnProperty("description", MySqlDbType.String)]
        public string Description { get; set; }

        #region Ctor & Dtor
        public MessageSendingTypeModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
