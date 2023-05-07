using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Configuration;
using WebApiFunction.Database;

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    [Serializable]
    public class SignalrHubMethodArgumentsModel : AbstractModel
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

        [JsonPropertyName("signalr_hub_method_uuid")]
        [DatabaseColumnPropertyAttribute("signalr_hub_method_uuid", MySqlDbType.String)]
        public Guid SignalrHubMethodUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("net_type")]
        [DatabaseColumnPropertyAttribute("net_type", MySqlDbType.String)]
        public string NetType { get; set; }


        #region Ctor & Dtor
        public SignalrHubMethodArgumentsModel()
        {

        }
        #endregion Ctor & Dtor

    }
}
