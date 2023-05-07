using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
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
    public class RoleToControllerViewModel : AbstractModel
    {
        #region Private
        private string _route = null;
        private string[] _routeSegments = null;
        private int[] _routeSegmentsIndexOfValues = null;
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnPropertyAttribute("role_uuid", MySqlDbType.String)]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("role")]
        [DatabaseColumnPropertyAttribute("role", MySqlDbType.String)]
        public string Role { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("http_method")]
        [DatabaseColumnPropertyAttribute("http_method", MySqlDbType.String)]
        public string HttpMethod { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("crud")]
        [DatabaseColumnPropertyAttribute("crud", MySqlDbType.String)]
        public string Crud { get; set; }

        [JsonPropertyName("flag")]
        [DatabaseColumnPropertyAttribute("flag", MySqlDbType.Int32)]
        public int Flag { get; set; } = GeneralDefs.NotFoundResponseValue;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("route")]
        [DatabaseColumnPropertyAttribute("route", MySqlDbType.String)]
        public string Route
        {
            get
            {
                return _route;
            }
            set
            {
                if (value != null)
                {

                    _route = value.ToLower();

                    _routeSegments = _route.Split(new string[] { "/" }, StringSplitOptions.None);
                    List<int> matchIndexes = new List<int>();
                    for (int i = 0; i < _routeSegments.Length; i++)
                    {
                        string valueTmp = _routeSegments[i];
                        Match match = Regex.Match(valueTmp, BackendAPIDefinitionsProperties.UriValueWildCardExtractRegEx);
                        if (match.Success)
                        {
                            matchIndexes.Add(i);
                        }
                    }
                    _routeSegmentsIndexOfValues = matchIndexes.ToArray();
                }

            }
        }


        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnPropertyAttribute("controller_uuid", MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("controller")]
        [DatabaseColumnPropertyAttribute("controller", MySqlDbType.String)]
        public string Controller { get; set; }

        [JsonPropertyName("max_request_per_hour")]
        [DatabaseColumnPropertyAttribute("max_request_per_hour", MySqlDbType.Int32)]
        public int MaxRequestPerHour { get; set; } = GeneralDefs.NotFoundResponseValue;

        [JsonPropertyName("max_time_after_request_in_ms")]
        [DatabaseColumnPropertyAttribute("max_time_after_request_in_ms", MySqlDbType.Int32)]
        public int MaxTimeAfterRequestInMs { get; set; } = GeneralDefs.NotFoundResponseValue;

        [JsonIgnore]
        public string[] RouteSegments
        {
            get
            {
                return _routeSegments;
            }
        }

        [JsonIgnore]
        public int[] RouteSegmentsValueIndexer
        {
            get
            {
                return _routeSegmentsIndexOfValues;
            }
        }



        #region Ctor & Dtor
        public RoleToControllerViewModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
