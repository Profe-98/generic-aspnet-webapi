using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ClassRelationToClassViewModel : ClassRelationModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

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

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("has_controller")]
        [DatabaseColumnPropertyAttribute("has_controller", MySql.Data.MySqlClient.MySqlDbType.Byte)]
        public bool HasController { get; set; } = false;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("controller_name")]
        [DatabaseColumnPropertyAttribute("controller_name", MySql.Data.MySqlClient.MySqlDbType.Byte)]
        public string ControllerName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnPropertyAttribute("controller_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;


        #region Ctor & Dtor
        public ClassRelationToClassViewModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
