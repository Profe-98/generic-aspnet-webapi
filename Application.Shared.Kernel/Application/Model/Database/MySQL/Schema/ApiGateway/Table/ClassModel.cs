using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    public class ClassModel : AbstractModel
    {

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

    }
}
