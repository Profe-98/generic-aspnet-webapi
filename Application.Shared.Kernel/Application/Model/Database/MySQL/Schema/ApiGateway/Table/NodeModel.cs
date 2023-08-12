using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    public class NodeModel : AbstractModel
    {

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("name")]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; } = null;

        [JsonPropertyName("node_type_uuid")]
        [DatabaseColumnProperty("node_type_uuid", MySqlDbType.String)]
        virtual public Guid NodeTypeUuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("ip")]
        [DatabaseColumnProperty("ip", MySqlDbType.String)]
        public string Ip { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("net_id")]
        [DatabaseColumnProperty("net_id", MySqlDbType.String)]
        public string NetId { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("mask")]
        [DatabaseColumnProperty("mask", MySqlDbType.String)]
        public string Mask { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("gateway")]
        [DatabaseColumnProperty("gateway", MySqlDbType.String)]
        public string Gateway { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("dns_server")]
        [DatabaseColumnProperty("dns_server", MySqlDbType.String)]
        public string DnsServers { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("port ")]
        [DatabaseColumnProperty("port", MySqlDbType.Int32)]
        public int Port { get; set; } = 0;

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("last_keep_alive")]
        [DatabaseColumnProperty("last_keep_alive", MySqlDbType.DateTime)]
        public DateTime LastKeepAlive { get; set; }

        [JsonIgnore]
        public bool IsRegistered { get; set; } = false;

    }
}
