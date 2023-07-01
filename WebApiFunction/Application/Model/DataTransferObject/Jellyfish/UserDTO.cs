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
            this.User = userModel.User;
            this.FirstName = userModel.FirstName;
            this.LastName = userModel.LastName;
            this.Uuid = userModel.Uuid; 
            this.Phone = userModel.Phone;
            this.DateOfBirth = userModel.DateOfBirth;   
            this.UserTypeUuid = userModel.UserTypeUuid;
           
        }

        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
