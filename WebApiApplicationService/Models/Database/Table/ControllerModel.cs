using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ControllerModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("api_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ApiUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("is_authcontroller", MySql.Data.MySqlClient.MySqlDbType.Bit)]
        public bool IsAuthcontroller { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("is_errorcontroller", MySql.Data.MySqlClient.MySqlDbType.Bit)]
        public bool IsErrorController { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("is_registered", MySql.Data.MySqlClient.MySqlDbType.Bit)]
        public bool IsRegistered { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("node_type_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid NodeTypeUuid { get; set; } = Guid.Empty;

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
                Replace(BackendAPIDefinitionsProperties.ControllerWildcard, this.Name);
        }
        public string GetControllerRoute()
        {
            return Api.Name + "/" + this.Name.ToLower();
        }
        public override string ToString()
        {
            return this.Name;
        }
        #endregion Methods
    }
}
