using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class DriverVersionRelationCommandModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("odbc_driver_version_uuid")]
        [DatabaseColumnPropertyAttribute("odbc_driver_version_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OdbcDriverVersionUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("schema_data_command_uuid")]
        [DatabaseColumnPropertyAttribute("schema_data_command_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid SchemaDataCommandUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(1024, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("sql_statement_template")]
        [DatabaseColumnPropertyAttribute("sql_statement_template", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string SqlStatementTemplate { get; set; }


        #region Ctor & Dtor
        public DriverVersionRelationCommandModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
