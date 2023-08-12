using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
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
