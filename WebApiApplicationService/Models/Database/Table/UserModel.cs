using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebApiApplicationService.Models.DataTransferObject;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Handler;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class UserModel : AbstractModel
    {
        #region Private
        private RegisterDataTransferModel _registerDataTransferModel;
        #endregion Private
        #region Public
        [JsonPropertyName("user_type_uuid")]
        [DatabaseColumnPropertyAttribute("user_type_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid UserTypeUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("account_uuid")]
        [DatabaseColumnPropertyAttribute("account_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid AccountUuid { get; set; } = Guid.Empty;

        //users email is user
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user")]
        [DatabaseColumnPropertyAttribute("user", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string User { get; set; }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        [DatabaseColumnPropertyAttribute("password", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Password { get; set; }

        [JsonPropertyName("max_auth_token")]
        [DatabaseColumnPropertyAttribute("max_auth_token", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int MaxCurrentActiveTokens { get; set; }

        [JsonPropertyName("api_access_granted")]
        [DatabaseColumnPropertyAttribute("api_access_granted", MySql.Data.MySqlClient.MySqlDbType.Bit)]
        public bool ApiAccessGranted { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("first_name")]
        [DatabaseColumnPropertyAttribute("first_name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string FirstName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("last_name")]
        [DatabaseColumnPropertyAttribute("last_name", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string LastName { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("phone")]
        [DatabaseColumnPropertyAttribute("phone", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Phone { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("date_of_birth")]
        [DatabaseColumnPropertyAttribute("date_of_birth", MySql.Data.MySqlClient.MySqlDbType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        //base64 like Bearer Token for OAuth: includes expirestime
        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]

        [JsonPropertyName("activation_token")]
        [DatabaseColumnPropertyAttribute("activation_token", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string ActivationToken { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(4, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(4, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("activation_code")]
        [DatabaseColumnPropertyAttribute("activation_code", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string ActivationCode { get; set; }

        [JsonConverter(typeof(JsonConverter.JsonDateTimeToIsoConverter))]
        [JsonPropertyName("expires_time")]
        public DateTime ActivationTokenExpires { get; set; }

        [JsonPropertyName("last_api_call_ticks")]
        public long LastApiCall { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(5, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("user_profile_pic_file_ext")]
        [DatabaseColumnPropertyAttribute("user_profile_pic_file_ext", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string UserProfilePicFileExtension { get; set; }

        [JsonConverter(typeof(JsonConverter.JsonBoolConverter))]
        [JsonPropertyName("is_adm")]
        public bool IsAdmin { get; set; }


        [JsonIgnore]
        public UserTypeModel UserType { get; set; }

        [JsonIgnore]
        public bool HasProfilePicture
        {
            get
            {
                return !String.IsNullOrEmpty(UserProfilePicFileExtension);
            }
        }

        /// <summary>
        /// Key is the Hour e.g. 4 and the Value of the Key is the count of request per hour (key)
        /// </summary>
        [JsonPropertyName("api_calls")]
        public Dictionary<int, int> ApiCalls { get; set; } = new Dictionary<int, int>();

        [JsonIgnore]
        public bool MaxRequestPerHourExceeded
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
        public bool HasTimeBetweenLastRequestViolation
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

                    if (diff.Milliseconds <= this.MaxGrantedTimeToNextRequest)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Count for User that counts upward for each TimeBetweenLastRequest Violation
        /// </summary>
        [JsonIgnore]
        public int TimeBetweenLastRequestViolationCount { get; set; }

        /// <summary>
        /// Select the max granted request count for user for api calls /backend calls
        /// </summary>
        [JsonIgnore]
        public int MaxGrantedRequestPerHour
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
        public int MaxGrantedTimeToNextRequest
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
            this.User = userDataTransferModel.User;
            this.Password = userDataTransferModel.Password;
        }
        public UserModel(RegisterDataTransferModel registerDataTransferModel)
        {
            this.User = registerDataTransferModel.EMail;
            this.Password = registerDataTransferModel.Password;
            this.FirstName = registerDataTransferModel.FirstName;
            this.LastName = registerDataTransferModel.LastName;
            this.Phone = registerDataTransferModel.Phone;
            this.DateOfBirth = registerDataTransferModel.DateOfBirth;
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
        public bool SendActivationMail(IMailHandler mailHandler, string fromMail, string frontEndUrl, string registerActivationEndpoint)
        {
            if (_registerDataTransferModel != null && mailHandler != null)
            {

                string body = this.GenerateActicationMailBody(frontEndUrl + registerActivationEndpoint);
                SendMail(mailHandler, fromMail, _registerDataTransferModel.ResendActivationCode ? "New Activation Link" : "Activation Link", body);
                return true;
            }
            return false;
        }
        public bool SendMail(IMailHandler mailHandler, string fromMail, string subject, string body)
        {
            var mail = mailHandler.CreateMimeMessage(new MimeKit.InternetAddressList() { MimeKit.InternetAddress.Parse(this.User) }, null, MimeKit.MailboxAddress.Parse(fromMail), subject, body);
            mailHandler.Enqueue(fromMail, mail);
            return true;
        }
        public string NameConcat
        {
            get => this.FirstName + " " + this.LastName;
        }
        public string GenerateActicationMailBody(string activationLinkUrl)
        {
            return "Hello " + NameConcat + ",\n\nto complete the user activation follow this link: " + activationLinkUrl + ActivationToken + "\nAnd enter the following code when you entered the link:\n" + this.ActivationCode + "\n\nEnsure that you complete you user activation till '" + this.ActivationTokenExpires.ToLongDateString() + "'.\nOtherwise your account will by deleted automatically.";
        }
        public string GenerateActivationCompleteMailBody()
        {
            return "Hello " + NameConcat + ",\n\nAnd welcome!\nYour registration is now completed.";
        }
        public bool IsAuthorizedPath(string pathUri, string httpMethod)
        {
            string routeFromRequest = pathUri.ToLower();//hier weiter machen, funktion wertet noch falsch aus

            string[] routeFromRequestSplitter = routeFromRequest.Split(new string[] { "/" }, StringSplitOptions.None);

            string apiName = routeFromRequestSplitter[0];
            string controllerName = routeFromRequestSplitter[1];

            var api = AppManager.Api.Find(x => x.Name == routeFromRequestSplitter[0]);
            var controller = api.AvaibleControllers.Find(x => x.Name == controllerName);
            if (controller.IsAuthcontroller || controller.IsErrorController)
                return true;

            foreach (RoleModel role in this.AvaibleRoles)
            {

                var routes = controller.Roles.FindAll(x => x.RouteSegments.Length == routeFromRequestSplitter.Length && x.Role == role.Name);
                foreach (RoleToControllerViewModel roleViewModel in routes)
                {
                    var tmp = routeFromRequestSplitter;
                    if (roleViewModel.RouteSegmentsValueIndexer != null)
                    {
                        for (int i = 0; i < routeFromRequestSplitter.Length; i++)
                        {
                            if (roleViewModel.RouteSegmentsValueIndexer.Contains(i))//nur value werden mit wildcard / value placeholder ersetzt für den compare unten
                            {
                                tmp[i] = roleViewModel.RouteSegments[i];
                            }
                        }
                    }

                    List<bool> check = new List<bool>(routeFromRequestSplitter.Length);
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        check.Add(false);
                        var val = tmp[i];
                        if (val == roleViewModel.RouteSegments[i])
                        {
                            check[i] = true;
                        }
                    }

                    bool permitted = check.FindAll(x => x).Count == routeFromRequestSplitter.Length;
                    if (permitted && roleViewModel.HttpMethod.ToLower().Equals(httpMethod.ToLower()))
                    {
                        return true;
                    }


                }
            }
            return false;
        }
        #endregion Methods
    }
}
