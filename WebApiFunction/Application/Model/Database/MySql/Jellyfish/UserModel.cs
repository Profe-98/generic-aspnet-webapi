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
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish.DataTransferObject;

namespace WebApiFunction.Application.Model.Database.MySQL.Jellyfish
{
    [Serializable]
    public class UserModel : Table.UserModel
    {
        #region Private
        private RegisterDataTransferModel _registerDataTransferModel;
        #endregion Private
        #region Public

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(255, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("signalr_connection_id")]
        [DatabaseColumnProperty("signalr_connection_id", MySqlDbType.String)]
        public string SignalRConnectionId { get; set; }

        #endregion

        #region Ctor & Dtor
        public UserModel()
        {

        }
        public UserModel(Table.UserModel userModel)
        {
            if (userModel == null)
                return;
            User = userModel.User;
            Password = userModel.Password;
            FirstName = userModel.FirstName;
            LastName = userModel.LastName;
            Phone = userModel.Phone;
            DateOfBirth = userModel.DateOfBirth;
            AvaibleRoles = userModel.AvaibleRoles;
            ActivationToken = userModel.ActivationToken;
            ActivationTokenExpires = userModel.ActivationTokenExpires;
            ActivationDateTime = userModel.ActivationDateTime;
            ActivationCode = userModel.ActivationCode;
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
        public UserModel(UserFriendshipUserModelDTO userFriendshipUserModel)
        {
            Uuid = userFriendshipUserModel.Uuid;
            User = userFriendshipUserModel.User;
            FirstName = userFriendshipUserModel.FirstName;
            LastName = userFriendshipUserModel.LastName;
            Phone = userFriendshipUserModel.Phone;
            DateOfBirth = userFriendshipUserModel.DateOfBirth;
        }
        #endregion Ctor & Dtor
        #region Methods
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
        public bool SendPasswordResetRequestMail(IMailHandler mailHandler, string fromMail, string passwordResetEndpointUrl)
        {
            if (mailHandler != null)
            {

                string body = GeneratePasswordResetMailBody(passwordResetEndpointUrl);
                SendMail(mailHandler, fromMail, "Password Reset Request", body);
                return true;
            }
            return false;
        }
        public bool SendPasswordResetComplededMail(IMailHandler mailHandler, string fromMail, string applicationName)
        {
            if (mailHandler == null)
                return false;

            string body = GeneratePasswordResetCompleteMailBody();
            string subject = "" + applicationName + ": Password Reset";
            SendMail(mailHandler, fromMail, subject, body);
            return true;
        }
        public bool SendActivationComplededMail(IMailHandler mailHandler, string fromMail, string applicationName)
        {
            if (mailHandler == null)
                return false;

            string body = GenerateActivationCompleteMailBody();
            string subject = "Welcome to " + applicationName + "";
            SendMail(mailHandler, fromMail, subject, body);
            return true;
        }
        public string GeneratePasswordResetMailBody(string resetLinkUrl)
        {
            return "Hello " + NameConcat + ",\n\nto complete the user activation follow this link: " + resetLinkUrl + PasswordResetToken + "\nOr enter the password reset code in the app.\nPassword reset code: " + PasswordResetCode + "\n\nEnsure that you complete you user activation till '" + ActivationTokenExpires.ToLongDateString() + "'.\nOtherwise your request is not longer available.";
        }
        public string GenerateActicationMailBody(string activationLinkUrl)
        {
            return "Hello " + NameConcat + ",\n\nto complete the user activation follow this link: " + activationLinkUrl + ActivationToken + "\nAnd enter the following code when you entered the link:\n" + ActivationCode + "\n\nEnsure that you complete you user activation till '" + ActivationTokenExpires.ToLongDateString() + "'.\nOtherwise your account will by deleted automatically.";
        }
        public string GenerateActivationCompleteMailBody()
        {
            return "Hello " + NameConcat + ",\n\nAnd welcome!\nYour registration is now completed.";
        }
        public string GeneratePasswordResetCompleteMailBody()
        {
            return "Hello " + NameConcat + ",\n\nyour password reset was successfull.\nYou can now login!";
        }

        #endregion Methods
    }
}
