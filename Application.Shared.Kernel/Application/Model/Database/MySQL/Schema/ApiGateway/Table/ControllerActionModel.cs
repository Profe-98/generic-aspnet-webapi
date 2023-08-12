using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
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
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [JsonPropertyName("controller_uuid")]
        [DatabaseColumnProperty("controller_uuid", MySqlDbType.String)]
        public Guid ControllerUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("name", MySqlDbType.String)]
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
