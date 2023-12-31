﻿using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using System.Net;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View
{
    public class ControllerRelationToRoleView
    {

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("api_name")]
        [DatabaseColumnProperty("api_name", MySqlDbType.String)]
        public string ApiName { get; set; }


        [JsonPropertyName("api_uuid")]
        [DatabaseColumnProperty("api_uuid", MySqlDbType.String)]
        public Guid ApiUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("controller_name")]
        [DatabaseColumnProperty("controller_name", MySqlDbType.String)]
        public string ControllerName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnProperty("controller_uuid", MySqlDbType.String)]
        public string ControllerUuid { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("action_route")]
        [DatabaseColumnProperty("action_route", MySqlDbType.String)]
        public string ActionRoute { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("http_methods_concatted")]
        [DatabaseColumnProperty("http_methods_concatted", MySqlDbType.String)]
        public string HttpMethods { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("role_concatted")]
        [DatabaseColumnProperty("role_concatted", MySqlDbType.String)]
        public string Roles { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("available_node_sockets")]
        [DatabaseColumnProperty("available_node_sockets", MySqlDbType.String)]
        public string AvailableNodeSockets { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("available_nodes_str")]
        [DatabaseColumnProperty("available_nodes_str", MySqlDbType.String)]
        public string AvailableNodesString { get; set; }

        [JsonPropertyName("is_authcontroller")]
        [DatabaseColumnProperty("is_authcontroller", MySqlDbType.Bit)]
        public bool IsAuthController { get; set; }

        [JsonPropertyName("is_errorcontroller")]
        [DatabaseColumnProperty("is_errorcontroller", MySqlDbType.Bit)]
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
