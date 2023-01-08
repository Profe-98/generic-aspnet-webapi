using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    public class NodeModel : AbstractModel
    {

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnPropertyAttribute("name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Name { get; set; } = null;

        [JsonPropertyName("node_type_uuid")]
        [DatabaseColumnPropertyAttribute("node_type_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        virtual public Guid NodeTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("ip")]
        [DatabaseColumnPropertyAttribute("ip", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Ip { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("net_id")]
        [DatabaseColumnPropertyAttribute("net_id", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string NetId { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("mask")]
        [DatabaseColumnPropertyAttribute("mask", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Mask { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("gateway")]
        [DatabaseColumnPropertyAttribute("gateway", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Gateway { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("dns_server")]
        [DatabaseColumnPropertyAttribute("dns_server", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string DnsServers { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("port ")]
        [DatabaseColumnPropertyAttribute("port", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int Port { get; set; } = 0;

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("last_keep_alive")]
        [DatabaseColumnPropertyAttribute("last_keep_alive", MySql.Data.MySqlClient.MySqlDbType.DateTime)]
        public DateTime LastKeepAlive { get; set; }

        [JsonIgnore]
        public bool IsRegistered { get; set; } = false;

    }
}
