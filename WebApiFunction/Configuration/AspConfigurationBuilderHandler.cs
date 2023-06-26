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
using MySql.Data.MySqlClient;
using System.Reflection.Metadata.Ecma335;
using WebApiFunction.Application.Model.Internal;


using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
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

namespace WebApiFunction.Configuration
{
    public static class AspConfigurationBuilderHandler
    {
        public static IConfigurationBuilder AddCustomWebApiConfig<T>(this IConfigurationBuilder configurationBuilder, string fileRootDir, T configurationToAppend = null)
            where T : AbstractConfigurationModel
        {
            T tmp = null;
            if (configurationToAppend == null)
            {
                tmp = Activator.CreateInstance<T>();
            }
            string instanceName = configurationToAppend != null ?
                configurationToAppend.GetType().Name : tmp.GetType().Name;

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
        [JsonPropertyName("root_dir")]
        public string RootDir { get; set; }
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
        [JsonPropertyName("nodemanager_database_configuration")]
        public DatabaseConfigurationModel NodeManagerDatabaseConfigurationModel { get; set; }
        [JsonPropertyName("gateway_routes_configuration")]
        public RoutesConfigurationModel RoutesConfigurationModel { get; set; }
        [JsonPropertyName("cache_configuration")]
        public CacheConfigurationModel CacheConfigurationModel { get; set; }
        [JsonPropertyName("ampq_rabbitmq_configuration")]
        public AmpqConfigurationModel RabbitMqConfigurationModel { get; set; }
        [JsonPropertyName("signalr_hub_configuration")]
        public SignalRConfigurationModel SignalRHubConfigurationModel { get; set; }
        [JsonPropertyName("app_paths")]
        public Dictionary<string, string> AppPaths { get; set; } = new Dictionary<string, string>();

        public AppServiceConfigurationModel()
        {

        }
        public AppServiceConfigurationModel(string rootDir)
        {
            this.RootDir = rootDir;
            SetDefaults();
        }

        private void SetDefaults()
        {

            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Log.LocalPath, Path.Combine(RootDir,"log","local"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Static.StaticContentPath, Path.Combine(RootDir, "static"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Controller.FileAttachmentsPath, Path.Combine(RootDir, "controller", "file","attachments"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.File.UserProfilePath, Path.Combine(RootDir, "file", "user","profile"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.File.NClamQuarantinePath, Path.Combine(RootDir, "file", "nclam", "quarantine"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.File.NClamCheckPath, Path.Combine(RootDir, "file", "nclam","check"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Mail.MailAttachmentPath, Path.Combine(RootDir, "mail", "attachment"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Mail.MailLogPath, Path.Combine(RootDir, "mail", "log"));
        }
    }
    public class WebApiConfigurationModel : AbstractConfigurationModel
    {
        [JsonPropertyName("encoding")]
        public string Encoding { get; set; } = System.Text.Encoding.UTF8.EncodingName;
        [JsonPropertyName("node_uuid")]
        public Guid NodeUuid { get; set; } = Guid.Empty;

    }
    public class SignalRConfigurationModel : AbstractConfigurationModel
    {
        [JsonPropertyName("use_local_hub")]
        public bool UseLocalHub { get; set; } = false;
        [JsonPropertyName("debug_errors_detailed_clientside")]
        public bool DebugErrorsDetailedClientside { get; set; } = false;
        [JsonPropertyName("timeout_sec")]
        public int TimoutTimeSec { get; set; } = 15;
        [JsonPropertyName("keepalive_timemout")]
        public int KeepaliveTimeout { get; set; } = 15;
        [JsonPropertyName("client_timeout_sec")]
        public int ClientTimeoutSec { get; set; } = 30;
        [JsonPropertyName("handshake_timeout")]
        public int HandshakeTimeout { get; set; } = 5;
        [JsonPropertyName("maximum_parallel_invocations_per_per_client")]
        public int MaximumParallelInvocationsPerClient { get; set; } = 1;



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
        public Log.General.MESSAGE_LEVEL LogLevel { get; set; } = Log.General.MESSAGE_LEVEL.LEVEL_INFO;
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

            [JsonPropertyName("user")]
            public string User { get; set; }
            [JsonPropertyName("password")]
            public string Password { get; set; }
            [JsonPropertyName("server")]
            public string Server { get; set; } = null;
            [JsonPropertyName("port")]
            public int Port { get; set; } = 0;
            [JsonPropertyName("secure_socket_options")]
            public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;
            [JsonPropertyName("timeout_ms")]
            public int Timeout { get; set; } = 10000;
            [JsonPropertyName("logger_file_name")]
            public string LoggerFolderPath { get; set; } = Environment.CurrentDirectory;
        }
        [JsonPropertyName("imap_settings")]
        public MailSettingsModel ImapSettings { get; set; } = new MailSettingsModel
        {
            
            Server = "imap.strato.de",
            Port = 993,
            SecureSocketOptions = SecureSocketOptions.Auto,
            Timeout = 10000,
            LoggerFolderPath = "MailLogs/imap/imap.log"
        };
        [JsonPropertyName("smtp_settings")]
        public MailSettingsModel SmtpSettings { get; set; } = new MailSettingsModel
        {
            Server = "smtp.strato.de",
            Port = 465,
            SecureSocketOptions = SecureSocketOptions.Auto,
            Timeout = 10000,
            LoggerFolderPath = "MailLogs/smtp/smtp.log"
        };
        [JsonPropertyName("email_attachment_path")]
        public string EmailAttachmentPath { get; set; }// = System.IO.Path.Combine(AppPaths.RootDir, "mail_attachment");
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
    public class RoutesConfigurationModel : AbstractConfigurationModel
    {
        public class DownstreamHostAndPortModel
        {
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public class LoadBalancerOptionsModel
        {
            public string Type { get; set; }
            public string Key { get; set; }
            public int? Expiry { get; set; }
        }
        public class AuthenticationOptionsModel
        {
            public string AuthenticationProviderKey { get; set; }
        }
        public class RouteClaimsRequirementModel
        {
            public List<string> Role { get; set; } = new List<string>();
        }
        public class RouteModel
        {
            public string DownstreamPathTemplate { get; set; }
            public string DownstreamScheme { get; set; }
            public List<DownstreamHostAndPortModel> DownstreamHostAndPorts { get; set; }
            public string UpstreamPathTemplate { get; set; }
            public List<string> UpstreamHttpMethod { get; set; }
            public AuthenticationOptionsModel AuthenticationOptions { get; set; }
            public RouteClaimsRequirementModel RouteClaimsRequirement { get; set; }
            public LoadBalancerOptionsModel LoadBalancerOptions { get; set; }
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
                return Host != null ? Utils.ParseEx(Host) : null;
            }
        }
        [JsonIgnore]
        public EndPoint EndPoint
        {
            get
            {
                return Port != 0 && IPAddr != null ? new IPEndPoint(IPAddr, (int)Port) : null;
            }
        }

    }
}
