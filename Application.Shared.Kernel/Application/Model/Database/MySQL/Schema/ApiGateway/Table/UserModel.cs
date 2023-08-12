using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Data.Format.Converter;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;
using static Application.Shared.Kernel.Data.Format.Converter.JsonConverter;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class UserModel : AbstractModel
    {
        #region Private
        private RegisterDataTransferModel _registerDataTransferModel;
        #endregion Private
        #region Public
        [JsonPropertyName("user_type_uuid")]
        [DatabaseColumnProperty("user_type_uuid", MySqlDbType.String)]
        public virtual Guid UserTypeUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("account_uuid")]
        [DatabaseColumnProperty("account_uuid", MySqlDbType.String)]
        public virtual Guid AccountUuid { get; set; } = Guid.Empty;

        //users email is user
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user")]
        [DatabaseColumnProperty("user", MySqlDbType.String)]
        public virtual string User { get; set; }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        [DatabaseColumnProperty("password", MySqlDbType.String)]
        public virtual string Password { get; set; }

        [JsonPropertyName("max_auth_token")]
        [DatabaseColumnProperty("max_auth_token", MySqlDbType.Int32)]
        public virtual int MaxCurrentActiveTokens { get; set; }

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

        //base64 like Bearer Token for OAuth: includes expirestime
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]

        [JsonPropertyName("activation_token")]
        [DatabaseColumnProperty("activation_token", MySqlDbType.String)]
        public virtual string ActivationToken { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(BackendAPIDefinitionsProperties.RegisterActivationCodeLen, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(BackendAPIDefinitionsProperties.RegisterActivationCodeLen, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("activation_code")]
        [DatabaseColumnProperty("activation_code", MySqlDbType.String)]
        public virtual string ActivationCode { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password_reset_code")]
        [DatabaseColumnProperty("password_reset_code", MySqlDbType.String)]
        public virtual string PasswordResetCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(BackendAPIDefinitionsProperties.PasswordResetCodeLen, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password_reset_code_confirmation")]
        [DatabaseColumnProperty("password_reset_code_confirmation", MySqlDbType.Bit)]
        public virtual bool PasswordResetCodeConfirmation { get; set; }

        [JsonPropertyName("password_reset_token")]
        [DatabaseColumnProperty("password_reset_token", MySqlDbType.String)]
        public virtual string PasswordResetToken { get; set; }

        [JsonConverter(typeof(JsonDateTimeToIsoConverter))]
        [JsonPropertyName("expires_time")]
        public virtual DateTime ActivationTokenExpires { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("password_reset_expires_in")]
        [DatabaseColumnProperty("password_reset_expires_in", MySqlDbType.DateTime)]
        public DateTime PasswordResetExpiresIn { get; set; }

        [JsonPropertyName("last_api_call_ticks")]
        public virtual long LastApiCall { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(5, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user_profile_pic_file_ext")]
        [DatabaseColumnProperty("user_profile_pic_file_ext", MySqlDbType.String)]
        public virtual string UserProfilePicFileExtension { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("picture", MySqlDbType.Blob)]
        public virtual byte[] Picture { get; set; }

        [JsonPropertyName("picture")]
        public virtual string PictureBase64
        {
            get
            {
                return Picture != null ? Convert.ToBase64String(Picture) : null;
            }
        }

        [JsonConverter(typeof(JsonBoolConverter))]
        [JsonPropertyName("is_adm")]
        public virtual bool IsAdmin { get; set; }


        [JsonIgnore]
        public virtual UserTypeModel UserType { get; set; }

        [JsonIgnore]
        public virtual bool HasProfilePicture
        {
            get
            {
                return !string.IsNullOrEmpty(UserProfilePicFileExtension);
            }
        }

        /// <summary>
        /// Key is the Hour e.g. 4 and the Value of the Key is the count of request per hour (key)
        /// </summary>
        [JsonPropertyName("api_calls")]
        public virtual Dictionary<int, int> ApiCalls { get; set; } = new Dictionary<int, int>();

        [JsonIgnore]
        public virtual bool MaxRequestPerHourExceeded
        {
            get
            {
                return ApiCalls.TryGetValue(DateTime.Now.Hour, out int counter) ?
                    counter >= MaxGrantedRequestPerHour : false;
            }
        }

        /// <summary>
        /// True when the Time ago after last request is smaller than granted for user
        /// </summary>
        [JsonIgnore]
        public virtual bool HasTimeBetweenLastRequestViolation
        {
            get
            {

                if (ApiCalls.Count > 1)
                {
                    long callBefore = LastApiCall;
                    long callCurrent = DateTime.Now.Ticks;

                    TimeSpan timeSpan1 = new TimeSpan(callBefore);
                    TimeSpan timeSpan2 = new TimeSpan(callCurrent);

                    TimeSpan diff = timeSpan2 - timeSpan1;

                    if (diff.Milliseconds <= MaxGrantedTimeToNextRequest)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Count for User that counts upward for each TimeBetweenLastRequest Violation
        /// </summary>
        [JsonIgnore]
        public virtual int TimeBetweenLastRequestViolationCount { get; set; }

        /// <summary>
        /// Select the max granted request count for user for api calls /backend calls
        /// </summary>
        [JsonIgnore]
        public virtual int MaxGrantedRequestPerHour
        {
            get
            {
                int max = AvaibleRoles.Count != 0 ? AvaibleRoles.Max(x => x.MaxRequestPerHour) : 0;
                return max;
            }
        }
        /// <summary>
        /// Max granted time between two request
        /// </summary>
        [JsonIgnore]
        public virtual int MaxGrantedTimeToNextRequest
        {
            get
            {
                int max = AvaibleRoles.Count != 0 ? AvaibleRoles.Max(x => x.MaxTimeAfterRequestInMs) : 0;
                return max;
            }
        }

        [JsonIgnore]
        public List<RoleModel> AvaibleRoles = new List<RoleModel>();
        #endregion Public



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
        public string GenerateCode(int len)
        {
            string actCode = null;
            Random random = new Random();
            for (int i = 0; i < len; i++)
            {
                actCode += random.Next(0, 9);
            }
            return actCode;
        }
        public bool SendActivationMail(IMailHandler mailHandler, string fromMail, string frontEndUrl, string registerActivationEndpoint)
        {
            if (_registerDataTransferModel != null && mailHandler != null)
            {

                string body = GenerateActicationMailBody(frontEndUrl + registerActivationEndpoint);
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
