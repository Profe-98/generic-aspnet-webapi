using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Extension;
using Application.Shared.Kernel.Configuration.Model.Abstraction;
using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;

namespace Application.Shared.Kernel.Configuration
{

    public class MainConfigurationModel : AbstractConfigurationModel, IMainConfigurationModel
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
        public ClamAvConfigurationModel AntivirusConfigurationModel { get; set; }
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

        public MainConfigurationModel()
        {

        }
        public MainConfigurationModel(string rootDir)
        {
            RootDir = rootDir;
            SetDefaults();
        }
        private void SetDefaults()
        {

            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Log.LocalPath, Path.Combine(RootDir, "log", "local"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Static.StaticContentPath, Path.Combine(RootDir, "static"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Controller.FileAttachmentsPath, Path.Combine(RootDir, "controller", "file", "attachments"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.File.UserProfilePath, Path.Combine(RootDir, "file", "user", "profile"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.File.NClamQuarantinePath, Path.Combine(RootDir, "file", "nclam", "quarantine"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.File.NClamCheckPath, Path.Combine(RootDir, "file", "nclam", "check"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Mail.MailAttachmentPath, Path.Combine(RootDir, "mail", "attachment"));
            AppPaths.Add(AppConfigDefinitionProperties.PathDictKeys.Mail.MailLogPath, Path.Combine(RootDir, "mail", "log"));
        }

    }
}
