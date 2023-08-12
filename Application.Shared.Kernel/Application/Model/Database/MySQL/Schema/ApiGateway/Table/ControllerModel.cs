using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class ControllerModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnProperty("api_uuid", MySqlDbType.String)]
        public Guid ApiUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("is_authcontroller", MySqlDbType.Bit)]
        public bool IsAuthcontroller { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("is_errorcontroller", MySqlDbType.Bit)]
        public bool IsErrorController { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("is_registered", MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("node_uuid", MySqlDbType.String)]
        public Guid NodeUuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        public List<RoleToControllerViewModel> Roles = new List<RoleToControllerViewModel>();

        [JsonIgnore]
        public ApiModel Api = null;

        #region Ctor & Dtor
        public ControllerModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        public string GetControllerRouteActionPattern()
        {
            return Api.RouterPattern.Replace(BackendAPIDefinitionsProperties.AreaWildcard, Api.Name).
                Replace(BackendAPIDefinitionsProperties.ControllerWildcard, Name);
        }
        public string GetControllerRoute()
        {
            return Api.Name + "/" + Name.ToLower();
        }
        public override string ToString()
        {
            return Name;
        }
        #endregion Methods
    }
}
