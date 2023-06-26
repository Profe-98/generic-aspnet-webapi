using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Application.Model.Database.MySQL.Helix
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
        [DatabaseColumnProperty("odbc_driver_version_uuid", MySqlDbType.String)]
        public Guid OdbcDriverVersionUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("operating_system_uuid")]
        [DatabaseColumnProperty("operating_system_uuid", MySqlDbType.String)]
        public Guid OperatingSystemUuid { get; set; } = Guid.Empty;

        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("library_file_hash")]
        [DatabaseColumnProperty("library_file_hash", MySqlDbType.String)]
        public string LibFileHash { get; set; }

        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("setup_file_hash")]
        [DatabaseColumnProperty("setup_file_hash", MySqlDbType.String)]
        public string SetupFileHash { get; set; }

        [DataType(DataType.Text, ErrorMessage = "must be a filename"), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("library_file")]
        [DatabaseColumnProperty("library_file", MySqlDbType.String)]
        public string LibraryFile { get; set; }

        [DataType(DataType.Text, ErrorMessage = "must be a filename"), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("setup_file")]
        [DatabaseColumnProperty("setup_file", MySqlDbType.String)]
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
