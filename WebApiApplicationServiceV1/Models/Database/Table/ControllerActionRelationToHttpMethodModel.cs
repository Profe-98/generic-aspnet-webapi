using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ControllerActionRelationToHttpMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("controller_action_uuid")]
        [DatabaseColumnPropertyAttribute( "controller_action_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ControllerActionUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("http_method_uuid")]
        [DatabaseColumnPropertyAttribute("http_method_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid HttpMethodUuid{ get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("action_route", MySql.Data.MySqlClient.MySqlDbType.String)]
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
