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

namespace WebApiFunction.Application.Model.Database.MySql.Jellyfish
{
    [Serializable]
    public class UserModel : WebApiFunction.Application.Model.Database.MySql.Entity.UserModel
    {
        #region Private
        private RegisterDataTransferModel _registerDataTransferModel;
        #endregion Private
        #region Public
        [JsonPropertyName("user_type_uuid")]
        [DatabaseColumnPropertyAttribute("user_type_uuid", MySqlDbType.String)]
        public Guid UserTypeUuid { get; set; } = Guid.Empty;

        //users email is user
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user")]
        [DatabaseColumnPropertyAttribute("user", MySqlDbType.String)]
        public string User { get; set; }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        [DatabaseColumnPropertyAttribute("password", MySqlDbType.String)]
        public string Password { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("first_name")]
        [DatabaseColumnPropertyAttribute("first_name", MySqlDbType.String)]
        public string FirstName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("last_name")]
        [DatabaseColumnPropertyAttribute("last_name", MySqlDbType.String)]
        public string LastName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("phone")]
        [DatabaseColumnPropertyAttribute("phone", MySqlDbType.String)]
        public string Phone { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("date_of_birth")]
        [DatabaseColumnPropertyAttribute("date_of_birth", MySqlDbType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        //base64 like Bearer Token for OAuth: includes expirestime
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]

        [JsonPropertyName("activation_token")]
        [DatabaseColumnPropertyAttribute("activation_token", MySqlDbType.String)]
        public string ActivationToken { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(4, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(4, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("activation_code")]
        [DatabaseColumnPropertyAttribute("activation_code", MySqlDbType.String)]
        public string ActivationCode { get; set; }

        [JsonConverter(typeof(WebApiFunction.Converter.JsonConverter.JsonDateTimeToIsoConverter))]
        [JsonPropertyName("expires_time")]
        public DateTime ActivationTokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(5, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user_profile_pic_file_ext")]
        [DatabaseColumnPropertyAttribute("user_profile_pic_file_ext", MySqlDbType.String)]
        public string UserProfilePicFileExtension { get; set; }

        [JsonConverter(typeof(WebApiFunction.Converter.JsonConverter.JsonBoolConverter))]
        [JsonPropertyName("is_adm")]
        public bool IsAdmin { get; set; }


        [JsonIgnore]
        public UserTypeModel UserType { get; set; }

        [JsonIgnore]
        public bool HasProfilePicture
        {
            get
            {
                return !string.IsNullOrEmpty(UserProfilePicFileExtension);
            }
        }
        #endregion

        #region Ctor & Dtor
        public UserModel()
        {

        }
        public UserModel(UserDataTransferModel userDataTransferModel)
        {
            User = userDataTransferModel.User;
            Password = userDataTransferModel.Password;
        }
        public UserModel(RegisterDataTransferModel registerDataTransferModel)
        {
            User = registerDataTransferModel.EMail;
            Password = registerDataTransferModel.Password;
            FirstName = registerDataTransferModel.FirstName;
            LastName = registerDataTransferModel.LastName;
            Phone = registerDataTransferModel.Phone;
            DateOfBirth = registerDataTransferModel.DateOfBirth;
            _registerDataTransferModel = registerDataTransferModel;
        }
        #endregion Ctor & Dtor
        #region Methods
        public void SetRegisterDataTransferModel(RegisterDataTransferModel registerDataTransferModel)
        {
            _registerDataTransferModel = registerDataTransferModel;
        }
        public string GenerateCode()
        {
            string actCode = null;
            Random random = new Random();
            for (int i = 0; i < BackendAPIDefinitionsProperties.RegisterActivationCodeLen; i++)
            {
                actCode += random.Next(0, 9);
            }
            return actCode;
        }
        public bool SendActivationMail(IMailHandler mailHandler, string fromMail, string registrationEndpointUrl)
        {
            if (_registerDataTransferModel != null && mailHandler != null)
            {

                string body = GenerateActicationMailBody(registrationEndpointUrl);
                SendMail(mailHandler, fromMail, _registerDataTransferModel.ResendActivationCode ? "New Activation Link" : "Activation Link", body);
                return true;
            }
            return false;
        }
        public bool SendMail(IMailHandler mailHandler, string fromMail, string subject, string body)
        {
            var mail = mailHandler.CreateMimeMessage(new MimeKit.InternetAddressList() { MimeKit.InternetAddress.Parse(User) }, null, MimeKit.MailboxAddress.Parse(fromMail), subject, body);
            mailHandler.SendMail(mail);
            return true;
        }
        public string NameConcat
        {
            get => FirstName + " " + LastName;
        }
        public string GenerateActicationMailBody(string activationLinkUrl)
        {
            return "Hello " + NameConcat + ",\n\nto complete the user activation follow this link: " + activationLinkUrl + ActivationToken + "\nAnd enter the following code when you entered the link:\n" + ActivationCode + "\n\nEnsure that you complete you user activation till '" + ActivationTokenExpires.ToLongDateString() + "'.\nOtherwise your account will by deleted automatically.";
        }
        public string GenerateActivationCompleteMailBody()
        {
            return "Hello " + NameConcat + ",\n\nAnd welcome!\nYour registration is now completed.";
        }

        #endregion Methods
    }
}
