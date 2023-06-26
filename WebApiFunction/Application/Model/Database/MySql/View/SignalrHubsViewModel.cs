using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiFunction.Database;
using WebApiFunction.Configuration;
using MySql.Data.MySqlClient;
using System.Net;

namespace WebApiFunction.Application.Model.Database.MySQL.View
{
    [Serializable]
    public class SignalrHubsViewModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("hub_uuid")]
        [DatabaseColumnProperty("hub_uuid", MySqlDbType.String)]
        public Guid HubUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hub_name")]
        [DatabaseColumnProperty("hub_name", MySqlDbType.String)]
        public string HubName { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("route")]
        [DatabaseColumnProperty("route", MySqlDbType.String)]
        public string Route { get; set; }

        [JsonPropertyName("is_registered")]
        [DatabaseColumnProperty("is_registered", MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonPropertyName("node_uuid")]
        [DatabaseColumnProperty("node_type_uuid", MySqlDbType.String)]
        public Guid NodeUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("hub_method_uuid")]
        [DatabaseColumnProperty("hub_method_uuid", MySqlDbType.String)]
        public Guid HubMethodUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("class_location")]
        [DatabaseColumnProperty("class_location", MySqlDbType.String)]
        public string ClassLocation { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("access_modifyer")]
        [DatabaseColumnProperty("access_modifyer", MySqlDbType.String)]
        public string AccessModifiyer { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hub_method")]
        [DatabaseColumnProperty("hub_method", MySqlDbType.String)]
        public string HubMethod { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hub_method_params")]
        [DatabaseColumnProperty("hub_method_params", MySqlDbType.String)]
        public string HubMethodParams { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("http_methods_concatted")]
        [DatabaseColumnProperty("http_methods_concatted", MySqlDbType.String)]
        public string HttpMethods { get; set; }

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

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("role_concatted")]
        [DatabaseColumnProperty("role_concatted", MySqlDbType.String)]
        public string Roles { get; set; }

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
        #region Ctor & Dtor
        public SignalrHubsViewModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
