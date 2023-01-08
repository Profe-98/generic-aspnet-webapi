using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ApiModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        //db füllen
        //relation klassen intern sollen nicht json serializable sein, tabellen und column attribute mit db abgleichen und auf gleichen stand bringen + auth token caching mit lifetime + alles von CustomAuthorizationMiddleware in cache 


        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [DataType(DataType.Text)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("controller_route_pattern", MySqlDbType.String)]
        public string RouterPattern { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(128, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [DataType(DataType.Text)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("controller_area_pattern", MySqlDbType.String)]
        public string AreaPattern { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [DataType(DataType.Text)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("name", MySqlDbType.String)]
        public string Name { get; set; }


        [JsonIgnore]
        public List<ControllerModel> AvaibleControllers = new List<ControllerModel>();

        [JsonIgnore]
        public ControllerModel AuthController
        {
            get
            {
                List<ControllerModel> controllers = AvaibleControllers.FindAll(x => x.IsAuthcontroller);
                if (controllers.Count > 1)
                {
                    throw new NotSupportedException("you cant host more than 1 auth controllers for an api-area");
                }

                return controllers?.Count != 0 ?
                    controllers[0] : null;
            }
        }
        [JsonIgnore]
        public ControllerModel ErrController
        {
            get
            {
                List<ControllerModel> controllers = AvaibleControllers.FindAll(x => x.IsErrorController);
                if (controllers.Count > 1)
                {
                    throw new NotSupportedException("you cant host more than 1 error controllers for an api-area");
                }

                return controllers?.Count != 0 ?
                    controllers[0] : null;
            }
        }
        #region Ctor & Dtor
        public ApiModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        public Uri GetRouteToController(ControllerModel controller)
        {
            if (controller == null)
                return null;

            return new Uri(("/" + Name + "/" + controller.Name + "/").ToLower(), UriKind.Relative);
        }
        #endregion Methods
    }
}
