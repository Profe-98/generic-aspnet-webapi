using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View
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

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("has_controller")]
        [DatabaseColumnProperty("has_controller", MySqlDbType.Byte)]
        public bool HasController { get; set; } = false;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("controller_name")]
        [DatabaseColumnProperty("controller_name", MySqlDbType.Byte)]
        public string ControllerName { get; set; } = null;

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnProperty("controller_uuid", MySqlDbType.String)]
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
