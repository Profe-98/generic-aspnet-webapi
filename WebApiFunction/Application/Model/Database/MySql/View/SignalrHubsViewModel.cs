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

namespace WebApiFunction.Application.Model.Database.MySql.View
{
    [Serializable]
    public class SignalrHubsViewModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("hub_uuid")]
        [DatabaseColumnPropertyAttribute("hub_uuid", MySqlDbType.String)]
        public Guid HubUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hub_name")]
        [DatabaseColumnPropertyAttribute("hub_name", MySqlDbType.String)]
        public string HubName { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("route")]
        [DatabaseColumnPropertyAttribute("route", MySqlDbType.String)]
        public string Route { get; set; }

        [JsonPropertyName("is_registered")]
        [DatabaseColumnPropertyAttribute("is_registered", MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonPropertyName("node_uuid")]
        [DatabaseColumnPropertyAttribute("node_type_uuid", MySqlDbType.String)]
        public Guid NodeUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("hub_method_uuid")]
        [DatabaseColumnPropertyAttribute("hub_method_uuid", MySqlDbType.String)]
        public Guid HubMethodUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("class_location")]
        [DatabaseColumnPropertyAttribute("class_location", MySqlDbType.String)]
        public string ClassLocation { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("access_modifyer")]
        [DatabaseColumnPropertyAttribute("access_modifyer", MySqlDbType.String)]
        public string AccessModifiyer { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hub_method")]
        [DatabaseColumnPropertyAttribute("hub_method", MySqlDbType.String)]
        public string HubMethod { get; set; }

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("hub_method_params")]
        [DatabaseColumnPropertyAttribute("hub_method_params", MySqlDbType.String)]
        public string HubMethodParams { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("http_methods_concatted")]
        [DatabaseColumnPropertyAttribute("http_methods_concatted", MySqlDbType.String)]
        public string HttpMethods { get; set; }

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

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(256, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("role_concatted")]
        [DatabaseColumnPropertyAttribute("role_concatted", MySqlDbType.String)]
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
