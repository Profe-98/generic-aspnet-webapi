using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json.Serialization;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Security;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.IO;
using WebApiApplicationService.Handler;
using MySql.Data.MySqlClient;
using System.Reflection.Metadata.Ecma335;
using WebApiApplicationService;

namespace WebApiApplicationService.Handler
{
    public static class AspConfigurationBuilderHandler
    {
        public static IConfigurationBuilder AddCustomWebApiConfig<T>(this IConfigurationBuilder configurationBuilder, string fileRootDir, T configurationToAppend = null)
            where T : AbstractConfigurationModel
        {
            T tmp = null;
            if(configurationToAppend == null)
            {
                tmp = Activator.CreateInstance<T>();
            }
            string instanceName = configurationToAppend != null?
                configurationToAppend.GetType().Name:tmp.GetType().Name;

            string fileName = instanceName.ToLower().Replace("model", "") + ".json";
            string path = Path.Combine(fileRootDir, fileName);
            if (!File.Exists(path) && configurationToAppend != null)
            {
                string content = null;
                using (JsonHandler jsonHandler = new JsonHandler())
                {
                    content = jsonHandler.JsonSerialize<T>(configurationToAppend);
                }
                File.WriteAllText(path, content);
            }
            configurationBuilder.AddJsonFile(path, optional: false, reloadOnChange: true);
            return configurationBuilder;
        }
    }
    public abstract class AbstractConfigurationModel
    {
    }
    public class AppServiceConfigurationModel : AbstractConfigurationModel
    {
        [JsonPropertyName("web_api_configuration")]
        public WebApiConfigurationModel WebApiConfigurationModel { get; set; }
        [JsonPropertyName("api_security_configuration")]
        public ApiSecurityConfigurationModel ApiSecurityConfigurationModel { get; set; }
        [JsonPropertyName("logging_configuration")]
        public LogConfigurationModel LogConfigurationModel { get; set; }
        [JsonPropertyName("mail_configuration")]
        public MailConfigurationModel MailConfigurationModel { get; set; }
        [JsonPropertyName("anti_virus_configuration")]
        public AntivirusConfigurationModel AntivirusConfigurationModel { get; set; }
        [JsonPropertyName("database_configuration")]
        public DatabaseConfigurationModel DatabaseConfigurationModel { get; set; }
        [JsonPropertyName("gateway_routes_configuration")]
        public RoutesConfigurationModel RoutesConfigurationModel { get; set; }
        [JsonPropertyName("cache_configuration")]
        public CacheConfigurationModel CacheConfigurationModel { get; set; }
        [JsonPropertyName("ampq_rabbitmq_configuration")]
        public AmpqConfigurationModel RabbitMqConfigurationModel { get; set; }
    }
    public class WebApiConfigurationModel : AbstractConfigurationModel
    {
        [JsonPropertyName("encoding")]
        public string Encoding { get; set; } = System.Text.Encoding.UTF8.EncodingName;
        [JsonPropertyName("node_uuid")]
        public Guid NodeUuid { get; set; } = Guid.Empty;

    }
    public class ApiSecurityConfigurationModel : AbstractConfigurationModel
    {
        public class JsonWebTokenModel
        {
            [JsonPropertyName("jwt_bearer_secret_string")]
            public string JwtBearerSecretStr { get; set; } = "this is my custom Secret key for authnetication";
            [JsonIgnore]
            public byte[] JwtBearerSecretByteArr
            {
                get
                {
                    return JwtBearerSecretStr != null ? System.Text.Encoding.UTF8.GetBytes(JwtBearerSecretStr) : null;
                }
            }
        }
        [JsonPropertyName("api_content_type")]
        public string ApiContentType { get; set; } = GeneralDefs.ApiContentType;
        public class SiteProtectModel
        {
            [JsonPropertyName("max_http_request_uri_len")]
            public int MaxHttpRequUriLen { get; set; }
            [JsonPropertyName("max_http_header_field_len")]
            public int MaxHttpHeaderFieldLen { get; set; }
            [JsonPropertyName("max_http_header_field_value_len")]
            public int MaxHttpHeaderFieldValueLen { get; set; }
            [JsonPropertyName("max_http_content_len")]
            public int MaxHttpContentLen { get; set; }
        }
        [JsonPropertyName("jwt")]
        public JsonWebTokenModel Jwt { get; set; } = new JsonWebTokenModel();
        [JsonPropertyName("site_protect")]
        public SiteProtectModel SiteProtect { get; set; }
    }
    public class LogConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("log_level")]
        public AppManager.MESSAGE_LEVEL LogLevel { get; set; } = AppManager.MESSAGE_LEVEL.LEVEL_INFO;
        [JsonPropertyName("log_date_format")]
        public string LogdateFormat { get; set; } = "yyyy-MM-dd";
        [JsonPropertyName("log_time_format")]
        public string LogtimeFormat { get; set; } = "HH:mm:ss";
        [JsonPropertyName("userinterface_date_format")]
        public string UserInterfaceDateFormat { get; set; } = "yyyy-MM-dd";
        [JsonPropertyName("userinterface_time_format")]
        public string UserInterfaceTimeFormat { get; set; } = "HH:mm:ss";
    }
    public class MailConfigurationModel : AbstractConfigurationModel
    {
        public class MailSettingsModel
        {

            [JsonPropertyName("server")]
            public string Server { get; set; } = null;
            [JsonPropertyName("port")]
            public int Port { get; set; } = 0;
            [JsonPropertyName("secure_socket_options")]
            public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;
            [JsonPropertyName("timeout_ms")]
            public int Timeout { get; set; } = 10000;
            [JsonPropertyName("logger_file_name")]
            public string LoggerFile { get; set; } = "./MailLogs/mail.log";
        }
        [JsonPropertyName("imap_settings")]
        public MailSettingsModel ImapSettings { get; set; } = new MailSettingsModel
        {
            Server = "imap.strato.de",
            Port = 993,
            SecureSocketOptions = SecureSocketOptions.Auto,
            /*public string UserServiceImap = "service@helixdb.org";
            public string UserDatenschutzImap = "datenschutz@helixdb.org";
            public string UserSupportImap = "support@helixdb.org";
            public string PasswordImap = "g9iMFhYnxGMbzs9";*/
            Timeout = 10000,
            LoggerFile = "MailLogs/imap/imap.log"
        };
        [JsonPropertyName("smtp_settings")]
        public MailSettingsModel SmtpSettings { get; set; } = new MailSettingsModel
        {
            Server = "smtp.strato.de",
            Port = 465,
            SecureSocketOptions = SecureSocketOptions.Auto,
            /*public string UserServiceImap = "service@helixdb.org";
            public string UserDatenschutzImap = "datenschutz@helixdb.org";
            public string UserSupportImap = "support@helixdb.org";
            public string PasswordImap = "g9iMFhYnxGMbzs9";*/
            Timeout = 10000,
            LoggerFile = "MailLogs/smtp/smtp.log"
        };
        [JsonPropertyName("email_attachment_path")]
        public string EmailAttachmentPath { get; set; } = System.IO.Path.Combine(AppPaths.RootDir, "mail_attachment");
    }
    public class AntivirusConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public int Port { get; set; }
        [JsonPropertyName("delete_infected_file_permantly")]
        public bool DeleteInfectedFilesPermantly { get; set; }
    }

    public class AmpqConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public uint Port { get; set; }
        [JsonPropertyName("user")]
        public string User { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("virtual_host")]
        public string VirtualHost { get; set; }
        [JsonPropertyName("heartbeat_ms")]
        public int HeartBeatMs { get; set; } = 30000;
    }
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
                return new MySqlConnectionStringBuilder() {
                    Server =this.Host,
                    Port=this.Port,
                    UserID=this.User,
                    Password=this.Password,
                    Database=this.Database,
                    OldGuids=this.OldGuids,
                    ConvertZeroDateTime=this.ConvertZeroDateTime,
                    ConnectionTimeout = this.Timeout ,
                     
                };
            }
        }
    }
    public class RoutesConfigurationModel : AbstractConfigurationModel
    {
        public class DownstreamHostAndPortModel
        {
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public class AuthenticationOptionsModel
        {
            public string AuthenticationProviderKey { get; set; }
        }

        public class RouteModel
        {
            public string DownstreamPathTemplate { get; set; }
            public string DownstreamScheme { get; set; }
            public List<DownstreamHostAndPortModel> DownstreamHostAndPorts { get; set; }
            public string UpstreamPathTemplate { get; set; }
            public List<string> UpstreamHttpMethod { get; set; }
            public AuthenticationOptionsModel AuthenticationOptions { get; set; }
        }

        public class GlobalConfigurationModel
        {
            public string BaseUrl { get; set; }
            public List<string> DelegatingHandlers { get; set; }
        }

        public List<RouteModel> Routes { get; set; }
        public GlobalConfigurationModel GlobalConfiguration { get; set; }

    }
    public class CacheConfigurationModel : AbstractConfigurationModel
    {

        [JsonPropertyName("hosts")]
        public CacheHostConfigurationModel[] Hosts { get; set; }

    }
    public class CacheHostConfigurationModel : AbstractConfigurationModel
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

        [JsonIgnore]
        public IPAddress IPAddr
        {
            get
            {
                return Host != null? Utils.ParseEx(Host) : null;
            }
        }
        [JsonIgnore]
        public EndPoint EndPoint
        {
            get
            {
                return Port != 0 && IPAddr != null?new IPEndPoint(IPAddr, (int)Port) : null;
            }
        }
        
    }
}
