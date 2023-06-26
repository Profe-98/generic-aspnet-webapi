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
    public class OdbcDriverVersionModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("odbc_driver_uuid")]
        [DatabaseColumnProperty("odbc_driver_uuid", MySqlDbType.String)]
        public Guid OdbcDriverUuid { get; set; } = Guid.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(90, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_vendor")]
        [DatabaseColumnProperty("version_vendor", MySqlDbType.String)]
        public string VendorVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_internal")]
        [DatabaseColumnProperty("version_internal", MySqlDbType.String)]
        public string InternalVersion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("version_subversion_internal")]
        [DatabaseColumnProperty("version_subversion_internal", MySqlDbType.String)]
        public string InternalSubVersion { get; set; }

        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("encoding")]
        [DatabaseColumnProperty("encoding", MySqlDbType.String)]
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
