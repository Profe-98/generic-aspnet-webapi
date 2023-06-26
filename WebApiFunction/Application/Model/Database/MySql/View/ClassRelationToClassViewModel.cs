using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using WebApiFunction.Application.Model.Database.MySQL.Table;

namespace WebApiFunction.Application.Model.Database.MySQL.View
{
    [Serializable]
    public class ClassRelationToClassViewModel : ClassRelationModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("assembly")]
        [DatabaseColumnProperty("assembly", MySqlDbType.String)]
        public string Assembly { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("namespace")]
        [DatabaseColumnProperty("namespace", MySqlDbType.String)]
        public string Namespace { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("net_name")]
        [DatabaseColumnProperty("net_name", MySqlDbType.String)]
        public string NetName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("table_name")]
        [DatabaseColumnProperty("table_name", MySqlDbType.String)]
        public string TableName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("has_controller")]
        [DatabaseColumnProperty("has_controller", MySqlDbType.Byte)]
        public bool HasController { get; set; } = false;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("controller_name")]
        [DatabaseColumnProperty("controller_name", MySqlDbType.Byte)]
        public string ControllerName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnProperty("controller_uuid", MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;


        #region Ctor & Dtor
        public ClassRelationToClassViewModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
