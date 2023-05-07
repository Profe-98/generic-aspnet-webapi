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
using System.Net;

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    public class ControllerRelationToRoleView
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("api_name")]
        [DatabaseColumnPropertyAttribute("api_name", MySqlDbType.String)]
        public string ApiName { get; set; }


        [JsonPropertyName("api_uuid")]
        [DatabaseColumnPropertyAttribute("api_uuid", MySqlDbType.String)]
        public Guid ApiUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("controller_name")]
        [DatabaseColumnPropertyAttribute("controller_name", MySqlDbType.String)]
        public string ControllerName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnPropertyAttribute("controller_uuid", MySqlDbType.String)]
        public string ControllerUuid { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("action_route")]
        [DatabaseColumnPropertyAttribute("action_route", MySqlDbType.String)]
        public string ActionRoute { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("http_methods_concatted")]
        [DatabaseColumnPropertyAttribute("http_methods_concatted", MySqlDbType.String)]
        public string HttpMethods { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("role_concatted")]
        [DatabaseColumnPropertyAttribute("role_concatted", MySqlDbType.String)]
        public string Roles { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("available_node_sockets")]
        [DatabaseColumnPropertyAttribute("available_node_sockets", MySqlDbType.String)]
        public string AvailableNodeSockets { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("available_nodes_str")]
        [DatabaseColumnPropertyAttribute("available_nodes_str", MySqlDbType.String)]
        public string AvailableNodesString { get; set; }

        [JsonPropertyName("is_authcontroller")]
        [DatabaseColumnPropertyAttribute("is_authcontroller", MySqlDbType.Bit)]
        public bool IsAuthController { get; set; }

        [JsonPropertyName("is_errorcontroller")]
        [DatabaseColumnPropertyAttribute("is_errorcontroller", MySqlDbType.Bit)]
        public bool IsErrorController { get; set; }

        [JsonIgnore]
        public Dictionary<IPEndPoint, DateTime> AvailableNodes
        {
            get
            {
                var response = new Dictionary<IPEndPoint, DateTime>();
                if (AvailableNodeSockets != null)
                {

                    foreach (string entry in AvailableNodeSockets?.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] socketProperty = entry.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                        if (socketProperty.Length != 3)
                            continue;

                        IPAddress ip = null;
                        int port = GeneralDefs.NotFoundResponseValue;

                        if (!IPAddress.TryParse(socketProperty[0], out ip))
                        {
                            var hostEntry = Dns.GetHostEntry(socketProperty[0]);
                            if (hostEntry != null)
                            {
                                for (int i = 0; i < hostEntry.AddressList.Length; i++)
                                {
                                    IPAddress ipTmp = hostEntry.AddressList[i];
                                    if (ipTmp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        ip = ipTmp;
                                        break;
                                    }
                                }
                            }

                            if (ip == null)
                                continue;
                        }

                        if (!int.TryParse(socketProperty[1], out port))
                            continue;
                        var endPoint = new IPEndPoint(ip, port);
                        if (!response.ContainsKey(endPoint))
                        {
                            response.Add(endPoint, DateTime.TryParse(socketProperty[2], out DateTime dateTime) ? dateTime : DateTime.MinValue);

                        }
                        else
                        {

                        }
                    }
                }
                else
                {

                }
                return response;
            }
        }
    }
}
