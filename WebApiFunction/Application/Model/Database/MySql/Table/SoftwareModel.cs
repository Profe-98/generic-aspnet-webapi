using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
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

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    [Serializable]
    public class SoftwareModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg), MaxLength(90, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySqlDbType.String)]
        public string SoftwareName { get; set; }


        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg)]
        [JsonPropertyName("description")]
        [DatabaseColumnPropertyAttribute("description", MySqlDbType.String)]
        public string SoftwareDescription { get; set; }

        #region Ctor & Dtor
        public SoftwareModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
