using System;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class ControllerRelationToClass : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnProperty("controller_uuid", MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("class_uuid")]
        [DatabaseColumnProperty("class_uuid", MySqlDbType.String)]
        public Guid ClassUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public ControllerRelationToClass()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
