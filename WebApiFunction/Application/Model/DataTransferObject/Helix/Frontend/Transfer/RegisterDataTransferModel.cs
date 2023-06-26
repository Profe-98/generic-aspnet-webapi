using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApiFunction.Application.Model.Internal;


using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
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

namespace WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer
{
    public class RegisterDataTransferModel : DataTransferModelAbstract
    {
        private string _email = null;
        private string _firstname = null;
        private string _lastname = null;
        private string _phone = null;

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("first_name")]
        public string FirstName { get => _firstname; set => _firstname = TransformString(value); }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("last_name")]
        public string LastName { get => _lastname; set => _lastname = TransformString(value); }

        [DataType(DataType.EmailAddress, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("email")]
        public string EMail { get => _email; set => _email = TransformString(value); }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(20, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("phone")]
        public string Phone { get => _phone; set => _phone = TransformString(value); }

        [DataType(DataType.Password, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("activation_token")]
        public string ActivateTokenBase64 { get; set; } = string.Empty;

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [JsonPropertyName("resend_code")]
        public bool ResendActivationCode { get; set; }


    }
}
