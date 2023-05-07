using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Configuration;
using WebApiFunction.Database;

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    [Serializable]
    public class SignalrHubModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySqlDbType.String)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("route")]
        [DatabaseColumnPropertyAttribute("route", MySqlDbType.String)]
        public string Route { get; set; }

        [JsonPropertyName("is_registered")]
        [DatabaseColumnPropertyAttribute("is_registered", MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonPropertyName("node_uuid")]
        [DatabaseColumnPropertyAttribute("node_uuid", MySqlDbType.String)]
        public Guid NodeUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public SignalrHubModel()
        {

        }
        #endregion Ctor & Dtor

    }
}
