using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class OdbcDriverVersionRelationToConnectionstringValuesModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("odbc_driver_version_uuid")]
        [DatabaseColumnPropertyAttribute("odbc_driver_version_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OdbcDriverVersionUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("connectionstring_values_uuid")]
        [DatabaseColumnPropertyAttribute("connectionstring_values_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ConnectionStringValuesUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public OdbcDriverVersionRelationToConnectionstringValuesModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
