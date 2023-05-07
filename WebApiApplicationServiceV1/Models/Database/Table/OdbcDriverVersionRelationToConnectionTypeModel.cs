using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class OdbcDriverVersionRelationToConnectionTypeModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("odbc_driver_version_uuid")]
        [DatabaseColumnPropertyAttribute("odbc_driver_version_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OdbcDriverVersionUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("connection_type_uuid")]
        [DatabaseColumnPropertyAttribute("connection_type_uuid",MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ConnectionTypeUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public OdbcDriverVersionRelationToConnectionTypeModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
