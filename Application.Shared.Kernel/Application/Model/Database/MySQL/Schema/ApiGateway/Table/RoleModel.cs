using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class RoleModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public virtual string Name { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("description")]
        [DatabaseColumnProperty("description", MySqlDbType.String)]
        public virtual string Description { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("max_request_per_hour", MySqlDbType.Int32)]
        public virtual int MaxRequestPerHour { get; set; } = GeneralDefs.NotFoundResponseValue;

        [JsonIgnore]
        [DatabaseColumnProperty("max_time_after_request_in_ms", MySqlDbType.Int32)]
        public virtual int MaxTimeAfterRequestInMs { get; set; } = GeneralDefs.NotFoundResponseValue;

        #region Ctor & Dtor
        public RoleModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
