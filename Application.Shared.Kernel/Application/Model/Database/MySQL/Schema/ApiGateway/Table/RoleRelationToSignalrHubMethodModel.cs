using System.Text.Json.Serialization;
using Application.Shared.Kernel.Application.Model.Database.MySQL;
using MySql.Data.MySqlClient;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class RoleRelationToSignalrHubMethodModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public

        [JsonPropertyName("role_uuid")]
        [DatabaseColumnProperty("role_uuid", MySqlDbType.String)]
        public Guid RoleUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("signalr_hub_method_uuid")]
        [DatabaseColumnProperty("signalr_hub_method_uuid", MySqlDbType.String)]
        public Guid SignalrHubMethodUuid { get; set; } = Guid.Empty;
        #endregion Public


        #region Ctor & Dtor
        public RoleRelationToSignalrHubMethodModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
