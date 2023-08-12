using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{
    [Serializable]
    public class UserDTO : DataTransferModelAbstract
    {
        #region Private
        private RegisterDataTransferModel _registerDataTransferModel;
        #endregion Private
        #region Public
        [JsonPropertyName("user_type_uuid")]
        [DatabaseColumnProperty("user_type_uuid", MySqlDbType.String)]
        public virtual Guid UserTypeUuid { get; set; } = Guid.Empty;

        //users email is user
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user")]
        [DatabaseColumnProperty("user", MySqlDbType.String)]
        public virtual string User { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("first_name")]
        [DatabaseColumnProperty("first_name", MySqlDbType.String)]
        public virtual string FirstName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("last_name")]
        [DatabaseColumnProperty("last_name", MySqlDbType.String)]
        public virtual string LastName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("phone")]
        [DatabaseColumnProperty("phone", MySqlDbType.String)]
        public virtual string Phone { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("date_of_birth")]
        [DatabaseColumnProperty("date_of_birth", MySqlDbType.DateTime)]
        public virtual DateTime DateOfBirth { get; set; }


        [JsonPropertyName("last_api_call_ticks")]
        public virtual long LastApiCall { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(5, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user_profile_pic_file_ext")]
        [DatabaseColumnProperty("user_profile_pic_file_ext", MySqlDbType.String)]
        public virtual string UserProfilePicFileExtension { get; set; }

        /// <summary>
        /// Base64 String that would be decoded to byte array and stored as blob in mysql
        /// </summary>
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg)]
        [JsonPropertyName("picture")]
        [DatabaseColumnProperty("picture", MySqlDbType.Text)]
        public virtual string Picture { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(255, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("signalr_connection_id")]
        public string SignalRConnectionId { get; set; }

        #endregion

        #region Ctor & Dtor
        public UserDTO()
        {

        }
        public UserDTO(UserModel userModel)
        {
            User = userModel.User;
            FirstName = userModel.FirstName;
            LastName = userModel.LastName;
            Uuid = userModel.Uuid;
            Phone = userModel.Phone;
            DateOfBirth = userModel.DateOfBirth;
            UserTypeUuid = userModel.UserTypeUuid;

        }

        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
