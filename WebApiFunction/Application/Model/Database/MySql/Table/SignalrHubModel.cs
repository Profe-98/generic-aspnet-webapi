using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Configuration;
using WebApiFunction.Database;

namespace WebApiFunction.Application.Model.Database.MySQL.Table
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
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("route")]
        [DatabaseColumnProperty("route", MySqlDbType.String)]
        public string Route { get; set; }

        [JsonPropertyName("is_registered")]
        [DatabaseColumnProperty("is_registered", MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonPropertyName("node_uuid")]
        [DatabaseColumnProperty("node_uuid", MySqlDbType.String)]
        public Guid NodeUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public SignalrHubModel()
        {

        }
        #endregion Ctor & Dtor

    }
}
