using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiFunction.Configuration
{
    public static class BackendAPIDefinitionsProperties
    {

        public static string CacheApiRootNodeModelObjectRelationKeyDefine = ":relation-key:";
        public static string CacheApiRootNodeModelObjectKeyDefine = ":object";
        public static string CacheApiRootNodeModelKeyDefine = ":apirootnodemodel";
        public static TimeSpan NodeSendKeepAliveTime = new TimeSpan(0, 0, 30);
        public static readonly char[] LowerLetters = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static readonly char[] UpperLetters = null;

        public const int PaginationItemPerPage = 10;
        public static TimeSpan ExpiresRefreshTokenTime = ExpiresTokenTime.Add(new TimeSpan(0, 0, 60, 0, 0));//double of ExpiresTokenTime
        public static TimeSpan ExpiresTokenTime = new TimeSpan(0, 0, 30, 0, 0);
        public const string DateTimeStringFormat = "yyyy-MM-dd HH:mm:ss";
        public const string AreaWildcard = "{area}";
        public const string ControllerWildcard = "{controller}";
        public const string UriValueWildCardExtractRegEx = @"\{+[a-zA-Z0-9]+\}";
        public const string ActionParameterIdWildcard = "{id}";
        public const string ActionParameterOptionalIdWildcard = "{id?}";
        public const string ActionParameterOptionalIdSecondaryWildcard = "{relationid?}";
        public const string ActionParameterFileWildcard = "{file}";
        public const string ActionParameterFileOptWildcard = "{file?}";
        public const string RootRoleName = "root";
        public const string AnonymousRoleName = "anonymous";
        public const string RootName = "root";
        public const string RootUserPassword = "root";
        public const string AuthentificationControllerName = "authentification";
        public const string ErrorControllerName = "error";
        
        public const int RegisterActivationExpInDays = 7;
        public const int RegisterActivationCodeLen = 4;
        public const int PasswordResetCodeLen = 6;
        public const int PasswordResetExpInDays = 7;

        public const string NoReplyMailAddress = "no-reply@helixdb.org";
        public const string CmKeyFrontEndUser = "Front End User";
        public const string CmKeyExtSmtpService = "External SMTP Service";
        public const string CmKeyExtPop3Service = "External POP3 Service";
        public const string CmKeyLocalhost = "Localhost";
        public const string CmKeyDiscord = "Discord";
        public const string CmKeyExtImapService = "External IMAP Service";
        public const string CmKeySignal = "Signal";
        public const string CmKeyTelegramm = "Telegramm";
        public const string CmKeyWhatsappBusiness = "WhatsApp Business";
        public static readonly Dictionary<string, Guid> CommunicationMedium = new Dictionary<string, Guid>() {
            { CmKeyFrontEndUser,Guid.Empty } ,
            { CmKeyExtSmtpService,Guid.Empty } ,
            { CmKeyExtPop3Service,Guid.Empty } ,
            { CmKeyLocalhost,Guid.Empty } ,
            { CmKeyDiscord,Guid.Empty } ,
            { CmKeyExtImapService,Guid.Empty } ,
            { CmKeySignal,Guid.Empty } ,
            { CmKeyTelegramm,Guid.Empty } ,
            { CmKeyWhatsappBusiness,Guid.Empty }
        };

        public struct Claim
        {
            public static string ClaimTypeUserRole = "user_role";
            public static string ClaimTypeUserUuid = "user_uuid";
            public static string ClaimTypeUser = "user_name";
        }

        public struct NodeTypes
        {
            public static readonly Guid Gateway = new Guid("326ae438-6824-11ec-a0a5-d8bbc10f2ae0");
            public static readonly Guid Authentification_Authorization = new Guid("363f305d-6824-11ec-a0a5-d8bbc10f2ae0");
            public static readonly Guid Application = new Guid("396e95b7-6824-11ec-a0a5-d8bbc10f2ae0");
            public static readonly Guid Administration = new Guid("3c892d5b-6824-11ec-a0a5-d8bbc10f2ae0");
            public static readonly Guid MailParser = new Guid("854426e7-0524-11ed-a708-7085c294413b");
        }

        public struct PhysicalFileLocationRoutes
        {

            public const string UserProfilePictureRoute = "profile/picture/" + ActionParameterIdWildcard + "";

            public const string DriverBannerRoute = "media/banner/" + ActionParameterIdWildcard + "";
            public const string DriverLogoRoute = "media/logo/" + ActionParameterIdWildcard + "";

            public const string GeneralFileGetRoute = "file/" + ActionParameterIdWildcard + "/" + ActionParameterFileOptWildcard + "";
            public const string GeneralFilePutRoute = "file/" + ActionParameterIdWildcard;
        }
        public const string DriverMediaFilesFileExtension = ".svg";

        #region GeneralHttpStatusResponseMessages
        public const string HttpRequestHeaderKeyAuthorizationNotExisting = "bad request";
        public const string HttpRequestWrongContentType = "wrong content-type";
        public const string HttpRequestNotAuthorized = "forbidden";
        public const string HttpRequestNotFound = "resource not found";
        public const string HttpRequestBad = "bad request";
        public const string HttpUnproccessableEntity = "unproc. entity";
        #endregion GeneralHttpStatusResponseMessages
        #region ExceptionErrorHandling
        public const string ProductiveErrorController = "/error";
        public const string DebugErrorController = "/error-debug";
        #endregion ExceptionErrorHandling
        #region HealthEndpoint

        public const string HealthController = "/health";
        #endregion HealthEndpoint

        //Site Protector Consts
        #region SiteProtector
        public const int MaxTimeBetweenRequestViolations = 10;
        public const int MaxRequestPerHour = 600;//request count max value if exceed than ban the ip temp.
        public const int MaxGrantedTimeToNextRequest = 1000;//ms
        public const int MaxRequestWithoutUserAgent = 10;
        public static TimeSpan SiteProtectorBanTime = new TimeSpan(1, 0, 0);
        #endregion SiteProtector
        #region Ctor
        static BackendAPIDefinitionsProperties()
        {
            int i = 0;
            UpperLetters = new char[LowerLetters.Length];
            LowerLetters.ToList().ForEach(x =>
            {
                UpperLetters[i] = x.ToString().ToUpper()[0];
                i++;
            });
        }
        #endregion
    }
}
