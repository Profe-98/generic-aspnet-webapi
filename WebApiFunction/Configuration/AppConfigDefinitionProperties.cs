using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Configuration
{
    public struct AppConfigDefinitionProperties
    {
        public struct PathDictKeys
        {
            public struct Log
            {
                public const string LocalPath = "log+local";
            }
            public struct Static
            {
                public const string StaticContentPath = "static";
            }
            public struct Controller
            {

                public const string FileAttachmentsPath = "controller+file+attachments";
            }
            public struct File
            {

                public const string UserProfilePath = "file+user+profile";
                public const string UserPath = "file+user";
                public const string NClamCheckPath = "file+nclam+check";
                public const string NClamQuarantinePath = "file+nclam+quarantine";
            }
            public struct Mail
            {

                public const string MailLogPath = "mail+log";
                public const string MailAttachmentPath = "mail+attachment";
            }
        }
    }
}
