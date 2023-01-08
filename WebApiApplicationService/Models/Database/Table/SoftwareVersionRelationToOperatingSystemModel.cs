using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class SoftwareVersionRelationToOperatingSystemModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("software_version_uuid")]
        [DatabaseColumnPropertyAttribute("software_version_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid SoftwareVersionUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("operating_system_uuid")]
        [DatabaseColumnPropertyAttribute("operating_system_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid OperatingSystemUuid { get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public SoftwareVersionRelationToOperatingSystemModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
