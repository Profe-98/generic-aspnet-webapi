using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
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
using WebApiFunction.Application.Model.Database.MySQL.View;

namespace WebApiFunction.Application.Model.Database.MySQL.Table
{
    [Serializable]
    public class ControllerModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnProperty("api_uuid", MySqlDbType.String)]
        public Guid ApiUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("is_authcontroller", MySqlDbType.Bit)]
        public bool IsAuthcontroller { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("is_errorcontroller", MySqlDbType.Bit)]
        public bool IsErrorController { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("is_registered", MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("node_uuid", MySqlDbType.String)]
        public Guid NodeUuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        public List<RoleToControllerViewModel> Roles = new List<RoleToControllerViewModel>();

        [JsonIgnore]
        public ApiModel Api = null;

        #region Ctor & Dtor
        public ControllerModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        public string GetControllerRouteActionPattern()
        {
            return Api.RouterPattern.Replace(BackendAPIDefinitionsProperties.AreaWildcard, Api.Name).
                Replace(BackendAPIDefinitionsProperties.ControllerWildcard, Name);
        }
        public string GetControllerRoute()
        {
            return Api.Name + "/" + Name.ToLower();
        }
        public override string ToString()
        {
            return Name;
        }
        #endregion Methods
    }
}
