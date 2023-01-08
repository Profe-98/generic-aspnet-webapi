using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace WebApiApplicationService.Models.Database
{
    [Serializable]
    public class AuthModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("user_uuid", MySql.Data.MySqlClient.MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv4_remote", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Ipv4 { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv6_remote", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Ipv6 { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("remote_port", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int RemotePort { get; set; }

        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("local_port", MySql.Data.MySqlClient.MySqlDbType.Int32)]
        public int LocalPort { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv4_local", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Ipv4Local { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("ip_addrv6_local", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Ipv6Local { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(2000, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("token")]
        [DatabaseColumnPropertyAttribute("token", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string Token { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("token_expires")]
        [DatabaseColumnPropertyAttribute("token_expires_in", MySql.Data.MySqlClient.MySqlDbType.DateTime)]
        public DateTime TokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(2000, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("refresh_token")]
        [DatabaseColumnPropertyAttribute("refresh_token", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string RefreshToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("refresh_token_expires")]
        [DatabaseColumnPropertyAttribute("refresh_token_expires_in", MySql.Data.MySqlClient.MySqlDbType.DateTime)]
        public DateTime RefreshTokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(1024, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnPropertyAttribute("user_agent", MySql.Data.MySqlClient.MySqlDbType.String)]
        public string UserAgent { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("logout_datetime")]
        [DatabaseColumnPropertyAttribute("logout_datetime", MySql.Data.MySqlClient.MySqlDbType.DateTime)]
        public DateTime LogoutTime { get; set; }

        [JsonConverter(typeof(JsonConverter.JsonBoolConverter))]
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        [JsonIgnore]
        public IPAddress IPv4RemoteObject
        {
            get
            {
                return ConvertStringToIp(this.Ipv4);
            }
        }

        [JsonIgnore]
        public IPAddress IPv4LocalObject
        {
            get
            {
                return ConvertStringToIp(this.Ipv4);
            }
        }

        [JsonIgnore]
        public IPAddress IPv6RemoteObject
        {
            get
            {
                return ConvertStringToIp(this.Ipv6);
            }
        }

        [JsonIgnore]
        public IPAddress IPv6LocalObject
        {
            get
            {
                return ConvertStringToIp(this.Ipv6Local);
            }
        }

        [JsonIgnore]
        public bool IsTokenExpired
        {
            get
            {
                return DateTime.Now >= this.TokenExpires ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsRefreshTokenExpired
        {
            get
            {
                return DateTime.Now >= this.RefreshTokenExpires ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsLoggedIn
        {
            get
            {
                return this.Token == null ? false : true;
            }
        }
        [JsonIgnore]
        public UserModel UserModel { get; set; }

        #region Ctor & Dtor
        public AuthModel()
        {

        }
        #endregion Ctor & Dtor
        #region Methods
        private IPAddress ConvertStringToIp(string ipStr)
        {
            if (IPAddress.TryParse(this.Ipv4Local, out IPAddress address))
            {
                return address;
            }
            return null;
        }
        #endregion Methods
    }
}
