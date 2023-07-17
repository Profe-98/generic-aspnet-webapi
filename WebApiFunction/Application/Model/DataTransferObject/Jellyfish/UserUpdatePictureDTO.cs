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
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Mail;
using System.Xml.Linq;
using WebApiFunction.Application.Model.DataTransferObject;

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class UserUpdatePictureDTO : DataTransferModelAbstract
    {
        #region Private
        #endregion Private
        #region Public
        /// <summary>
        /// Base64 String that would be decoded to byte array and stored as blob in mysql
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        [JsonPropertyName("picture")]
        [DatabaseColumnProperty("picture", MySqlDbType.Text)]
        public virtual string Picture { get; set; }


        #endregion

        #region Ctor & Dtor
        public UserUpdatePictureDTO()
        {

        }

        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
