using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Configuration;
using WebApiFunction.Database;

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    [Serializable]
    public class SignalrHubMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public


        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("class_location")]
        [DatabaseColumnPropertyAttribute("class_location", MySqlDbType.String)]
        public string ClassLocation { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("access_modifyer")]
        [DatabaseColumnPropertyAttribute("access_modifyer", MySqlDbType.String)]
        public string AccessModifiyer { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySqlDbType.String)]
        public string Name { get; set; }

        [JsonPropertyName("signalr_hub_uuid")]
        [DatabaseColumnPropertyAttribute("signalr_hub_uuid", MySqlDbType.String)]
        public Guid SignalrHubUuid { get; set; } = Guid.Empty;


        #region Ctor & Dtor
        public SignalrHubMethodModel()
        {

        }
        #endregion Ctor & Dtor

    }
}
