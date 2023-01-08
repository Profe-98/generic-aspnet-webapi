using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class OdbcDriverVersionModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("odbc_driver_uuid")]
        [DatabaseColumnPropertyAttribute("odbc_driver_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OdbcDriverUuid { get; set; } = Guid.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(90, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_vendor")]
        [DatabaseColumnPropertyAttribute("version_vendor", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string VendorVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_internal")]
        [DatabaseColumnPropertyAttribute("version_internal", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string InternalVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_subversion_internal")]
        [DatabaseColumnPropertyAttribute("version_subversion_internal", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string InternalSubVersion { get; set; }

        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("encoding")]
        [DatabaseColumnPropertyAttribute("encoding", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Encoding { get; set; }


        #region Ctor & Dtor
        public OdbcDriverVersionModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
