using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;

using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Application.Model.Database.MySql.Entity
{
    [Serializable]
    public class UserModel : AbstractModel
    {
        #region Private
        private RegisterDataTransferModel _registerDataTransferModel;
        #endregion Private
        #region Public
        [JsonPropertyName("user_type_uuid")]
        [DatabaseColumnPropertyAttribute("user_type_uuid", MySqlDbType.String)]
        public Guid UserTypeUuid { get; set; } = Guid.Empty;

        [JsonPropertyName("account_uuid")]
        [DatabaseColumnPropertyAttribute("account_uuid", MySqlDbType.String)]
        public Guid AccountUuid { get; set; } = Guid.Empty;

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

        [JsonPropertyName("max_auth_token")]
        [DatabaseColumnPropertyAttribute("max_auth_token", MySqlDbType.Int32)]
        public int MaxCurrentActiveTokens { get; set; }

        [JsonPropertyName("api_access_granted")]
        [DatabaseColumnPropertyAttribute("api_access_granted", MySqlDbType.Bit)]
        public bool ApiAccessGranted { get; set; }

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

        [JsonPropertyName("last_api_call_ticks")]
        public long LastApiCall { get; set; }

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
            mailHandler.Enqueue(fromMail, mail);
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

            foreach (RoleModel role in AvaibleRoles)
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
