using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class ControllerActionRelationToHttpMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("controller_action_uuid")]
        [DatabaseColumnProperty("controller_action_uuid", MySqlDbType.String)]
        public Guid ControllerActionUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("http_method_uuid")]
        [DatabaseColumnProperty("http_method_uuid", MySqlDbType.String)]
        public Guid HttpMethodUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("action_route", MySqlDbType.String)]
        public string ActionRoute { get; set; }


        #region Ctor & Dtor
        public ControllerActionRelationToHttpMethodModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
