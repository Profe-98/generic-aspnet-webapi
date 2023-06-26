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

        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg), MinLength(1, ErrorMessage = DataValidationMessageStruct.StringMinLengthExceededMsg), MaxLength(255, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("signalr_connection_id")]
        public string SignalRConnectionId { get; set; }

        #endregion

        #region Ctor & Dtor
        public UserDTO()
        {

        }
        
        #endregion Ctor & Dtor
        #region Methods

        #endregion Methods
    }
}
