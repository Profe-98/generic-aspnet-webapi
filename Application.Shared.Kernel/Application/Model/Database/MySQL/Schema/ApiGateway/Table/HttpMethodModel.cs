using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class HttpMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public


        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("crud_uuid")]
        [DatabaseColumnProperty("crud_uuid", MySqlDbType.String)]
        public Guid CrudUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; }

        #region Ctor & Dtor
        public HttpMethodModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
