using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

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
}
