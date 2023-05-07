using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class ControllerActionModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [Required(ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("uuid")]
        [DatabaseColumnPropertyAttribute("uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnPropertyAttribute("controller_uuid",MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Name { get; set; }

        #region Ctor & Dtor
        public ControllerActionModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
