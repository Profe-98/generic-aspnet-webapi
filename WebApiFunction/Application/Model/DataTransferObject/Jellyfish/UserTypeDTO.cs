using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MimeKit;
using WebApiFunction.Configuration;
using WebApiFunction.Database;
using MySql.Data.MySqlClient;
using WebApiFunction.Application.Model.DataTransferObject;

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class UserTypeDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = DataValidationMessageStruct.OnlyCharsInStringAllowedMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        #region Ctor & Dtor
        public UserTypeDTO()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        #endregion Methods
    }
}
