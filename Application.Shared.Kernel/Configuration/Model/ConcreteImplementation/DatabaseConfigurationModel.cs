using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class DatabaseConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public uint Port { get; set; }
        [JsonPropertyName("timeout_s")]
        public uint Timeout { get; set; }
        [JsonPropertyName("user")]
        public string User { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("database")]
        public string Database { get; set; }
        [JsonPropertyName("old_guids")]
        public bool OldGuids { get; set; }
        [JsonPropertyName("convert_zero_datetime")]
        public bool ConvertZeroDateTime { get; set; }
        [JsonPropertyName("auto_commit")]
        public bool AutoCommit { get; set; }

        /*[JsonIgnore]
        public DateTimeFormatInfo MySqlDateTimeFormat
        {
            get
            {
                return _mysqlDateTimeFormat;
            }
        }*/
        [JsonIgnore]
        public MySqlConnectionStringBuilder MysqlConnectionString
        {
            get
            {
                return new MySqlConnectionStringBuilder()
                {
                    Server = Host,
                    Port = Port,
                    UserID = User,
                    Password = Password,
                    Database = Database,
                    OldGuids = OldGuids,
                    ConvertZeroDateTime = ConvertZeroDateTime,
                    ConnectionTimeout = Timeout,

                };
            }
        }
    }
}
