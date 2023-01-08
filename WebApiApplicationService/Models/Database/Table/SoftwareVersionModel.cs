using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class SoftwareVersionModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("software_uuid")]
        [DatabaseColumnPropertyAttribute("software_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid SoftwareUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("version_type_uuid")]
        [DatabaseColumnPropertyAttribute("version_type_uuid",MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid VersionTypeUuid { get; set; } = Guid.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_internal")]
        [DatabaseColumnPropertyAttribute("version_internal", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string InternalVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_subversion_internal")]
        [DatabaseColumnPropertyAttribute("version_subversion_internal", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string InternalSubVersion { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(90, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("release_note")]
        [DatabaseColumnPropertyAttribute("release_note", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string ReleaseNote { get; set; }

        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hash")]
        [DatabaseColumnPropertyAttribute("hash", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Hash { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("file_extension")]
        [DatabaseColumnPropertyAttribute("file_extension", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string FileExtension { get; set; }

        /*[JsonPropertyName("setup_file_uri")]
        public string SetupFilePath
        {
            get
            {
                string route = GetControllerRoute();
                if (route != null && !String.IsNullOrEmpty(FileExtension))
                {
                    route += ActionUri(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFileGetRoute, this.Uuid+FileExtension);
                }
                return route;
            }
        }*/
        #region Ctor & Dtor
        public SoftwareVersionModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
