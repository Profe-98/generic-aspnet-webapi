using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View
{
    [Serializable]
    public class NodeHealthEndpointViewModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("node_name")]
        [DatabaseColumnProperty("node_name", MySqlDbType.String)]
        public string NodeName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("ip")]
        [DatabaseColumnProperty("ip", MySqlDbType.String)]
        public string IPAddr { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("port")]
        [DatabaseColumnProperty("port", MySqlDbType.String)]
        public int Port { get; set; } = 0;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("last_keep_alive")]
        [DatabaseColumnProperty("last_keep_alive", MySqlDbType.DateTime)]
        public DateTime LastKeepAlive { get; set; } = DateTime.MinValue;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("node_type_name")]
        [DatabaseColumnProperty("node_type_name", MySqlDbType.String)]
        public string NodeTypeName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("node_type_uuid")]
        [DatabaseColumnProperty("node_type_uuid", MySqlDbType.String)]
        public Guid NodeTypeUuid { get; set; } = Guid.Empty;


        #region Ctor & Dtor
        public NodeHealthEndpointViewModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
