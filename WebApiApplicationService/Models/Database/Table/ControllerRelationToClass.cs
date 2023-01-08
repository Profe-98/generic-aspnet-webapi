using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ControllerRelationToClass : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnPropertyAttribute( "controller_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("class_uuid")]
        [DatabaseColumnPropertyAttribute("class_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ClassUuid{ get; set; } = Guid.Empty;

        #region Ctor & Dtor
        public ControllerRelationToClass()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
