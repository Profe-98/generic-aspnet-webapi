using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class OdbcDriverVersionFileModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("odbc_driver_version_uuid")]
        [DatabaseColumnPropertyAttribute("odbc_driver_version_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OdbcDriverVersionUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("operating_system_uuid")]
        [DatabaseColumnPropertyAttribute("operating_system_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OperatingSystemUuid { get; set; } = Guid.Empty;

        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("library_file_hash")]
        [DatabaseColumnPropertyAttribute("library_file_hash", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string LibFileHash { get; set; }

        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("setup_file_hash")]
        [DatabaseColumnPropertyAttribute("setup_file_hash", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string SetupFileHash { get; set; }

        [DataType(DataType.Text, ErrorMessage = "must be a filename"), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("library_file")]
        [DatabaseColumnPropertyAttribute("library_file", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string LibraryFile { get; set; }

        [DataType(DataType.Text, ErrorMessage = "must be a filename"), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("setup_file")]
        [DatabaseColumnPropertyAttribute("setup_file", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string SetupFile { get; set; }

        /*[JsonPropertyName("library_file_uri")]
        public string LibraryFilePath
        {
            get
            {
                string route = GetControllerRoute();
                if (route != null)
                {
                    route += ActionUri(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFileGetRoute, this.LibraryFile);
                }
                return route;
            }
        }

        [JsonPropertyName("setup_file_uri")]
        public string SetupFilePath
        {
            get
            {
                string route = GetControllerRoute();
                if(route != null)
                {
                    route += ActionUri(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFileGetRoute, this.SetupFile);
                }
                return route;
            }
        }*/



        #region Ctor & Dtor
        public OdbcDriverVersionFileModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
