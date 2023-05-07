using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    public class ClassModel : AbstractModel
    {

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("assembly")]
        [DatabaseColumnPropertyAttribute("assembly", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Assembly { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("namespace")]
        [DatabaseColumnPropertyAttribute("namespace", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Namespace { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("net_name")]
        [DatabaseColumnPropertyAttribute("net_name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string NetName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("table_name")]
        [DatabaseColumnPropertyAttribute("table_name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string TableName { get; set; } = null;

    }
}
