using System.Text.Json.Serialization;
using System.Net;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Data.Format.Converter;
using static Application.Shared.Kernel.Data.Format.Converter.JsonConverter;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table
{
    [Serializable]
    public class AuthModel : AbstractModel
    {
        #region Private
        #endregion Private
        #region Public
        #endregion Public

        [JsonIgnore]
        [DatabaseColumnProperty("uuid", MySqlDbType.String)]
        public override Guid Uuid { get; set; } = Guid.Empty;

        [JsonIgnore]
        [DatabaseColumnProperty("user_uuid", MySqlDbType.String)]
        public Guid UserUuid { get; set; } = Guid.Empty;

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("ip_addrv4_remote", MySqlDbType.String)]
        public string Ipv4 { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("ip_addrv6_remote", MySqlDbType.String)]
        public string Ipv6 { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("remote_port", MySqlDbType.Int32)]
        public int RemotePort { get; set; }

        [JsonIgnore]
        [DatabaseColumnProperty("local_port", MySqlDbType.Int32)]
        public int LocalPort { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(45, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("ip_addrv4_local", MySqlDbType.String)]
        public string Ipv4Local { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("ip_addrv6_local", MySqlDbType.String)]
        public string Ipv6Local { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(2000, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("token")]
        [DatabaseColumnProperty("token", MySqlDbType.String)]
        public string Token { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("token_expires")]
        [DatabaseColumnProperty("token_expires_in", MySqlDbType.DateTime)]
        public DateTime TokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(2000, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonPropertyName("refresh_token")]
        [DatabaseColumnProperty("refresh_token", MySqlDbType.String)]
        public string RefreshToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("refresh_token_expires")]
        [DatabaseColumnProperty("refresh_token_expires_in", MySqlDbType.DateTime)]
        public DateTime RefreshTokenExpires { get; set; }

        [DataType(DataType.Text, ErrorMessage = DataValidationMessageStruct.WrongDataTypeGivenMsg), MaxLength(1024, ErrorMessage = DataValidationMessageStruct.StringMaxLengthExceededMsg)]
        [JsonIgnore]
        [DatabaseColumnProperty("user_agent", MySqlDbType.String)]
        public string UserAgent { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = DataValidationMessageStruct.WrongDateTimeFormatMsg)]
        [JsonPropertyName("logout_datetime")]
        [DatabaseColumnProperty("logout_datetime", MySqlDbType.DateTime)]
        public DateTime LogoutTime { get; set; }

        [JsonConverter(typeof(JsonBoolConverter))]
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        [JsonIgnore]
        public IPAddress IPv4RemoteObject
        {
            get
            {
                return ConvertStringToIp(Ipv4);
            }
        }

        [JsonIgnore]
        public IPAddress IPv4LocalObject
        {
            get
            {
                return ConvertStringToIp(Ipv4);
            }
        }

        [JsonIgnore]
        public IPAddress IPv6RemoteObject
        {
            get
            {
                return ConvertStringToIp(Ipv6);
            }
        }

        [JsonIgnore]
        public IPAddress IPv6LocalObject
        {
            get
            {
                return ConvertStringToIp(Ipv6Local);
            }
        }

        [JsonIgnore]
        public bool IsTokenExpired
        {
            get
            {
                return DateTime.Now >= TokenExpires ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsRefreshTokenExpired
        {
            get
            {
                return DateTime.Now >= RefreshTokenExpires ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsLoggedIn
        {
            get
            {
                return Token == null ? false : true;
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
            if (IPAddress.TryParse(Ipv4Local, out IPAddress address))
            {
                return address;
            }
            return null;
        }
        #endregion Methods
    }
}
