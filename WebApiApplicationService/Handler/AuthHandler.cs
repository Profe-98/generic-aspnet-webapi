using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Data;
using System.Net;
using WebApiApplicationService.Models;
using WebApiApplicationService.Handler;
using WebApiApplicationService.InternalModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using WebApiApplicationService.Models.DataTransferObject;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Handler
{
    public class AuthHandler : AuthenticationSchemeOptions, IAuthHandler
    {
        #region Private
        private readonly IScopedDatabaseHandler _databaseHandler;
        private readonly IHttpContextHandler _httpContextHandler = null;
        private readonly IJWTHandler _jwtHandler = null;
        private readonly IScopedEncryptionHandler _encryptionHandler = null;
        private readonly IAppconfig _appConfig = null;
        #endregion
        #region Public
        public UserModel PredictedCurrentUser { get; set; }
        #endregion
        #region Ctor & Dtor
        public AuthHandler(IAppconfig appconfig,IScopedDatabaseHandler databaseHandler,IHttpContextHandler httpContextHandler, IJWTHandler jwtHandler, IScopedEncryptionHandler encryptionHandler)
        {
            _databaseHandler = databaseHandler;
            _httpContextHandler = httpContextHandler;
            _jwtHandler = jwtHandler;
            _encryptionHandler = encryptionHandler;
            _appConfig = appconfig;
            SetPredictedUser();
        }
        #endregion
        #region Methods
        private async void SetPredictedUser()
        {
            UserModel userModel = null;
            string token = _httpContextHandler.CurrentContext().GetRequestJWTFromHeader();
            if (token != null)
            {
                JWTModel data = DecodeJWT(token);
                userModel = (UserModel)data.UserModel;
                if(userModel != null)
                {
                    userModel = await GetUser(userModel.Uuid);
                }
            }
            PredictedCurrentUser = userModel;
        }
        private async Task<string> GenerateRefreshToken(UserModel userModel)
        {
            QueryResponseData queryResponseData = await _databaseHandler.GetUUID();

            return queryResponseData.HasData?
                queryResponseData.Data.Rows[0].ItemArray[0].ToString():null;
        }
        public string EncodeJWT(UserModel userModel, DateTime expiresToken,bool forRegistration)
        {
            return _jwtHandler.GenerateJwtToken(userModel, _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr, expiresToken, forRegistration);
        }
        
        public JWTModel DecodeJWT(string token)
        {
            JWTModel data = _jwtHandler.Decode(token, _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr);

            return data;
        }
        public async Task<AuthModel> Login(string token)
        {
            AuthModel authModel = null;

            JWTModel data = _jwtHandler.Decode(token, _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr);

            UserModel userObject = (UserModel)data.UserModel;


            if (userObject != null)
            {
                UserModel userModel = userObject;
                authModel = await this.Login(new UserDataTransferModel(userModel));
            }
            return authModel;
        }
        public async Task<AuthModel> Login(UserDataTransferModel userModel)
        {

            AuthModel authModel = null;
            HttpContext context = _httpContextHandler.CurrentContext();
            bool userAgentExists = context.Request.Headers.ContainsKey("User-Agent");
            string passwordHash = await _encryptionHandler.MD5Async(userModel.Password);
            if (userAgentExists)
            {
                QueryResponseData<UserModel> data = await _databaseHandler.ExecuteQueryWithMap<UserModel>("SELECT * FROM user WHERE user = @user AND password = LOWER(@password) AND active = @active LIMIT 1;", new UserModel { User = userModel.User,Password = passwordHash, Active= true });
                if (data.HasStorageData)
                {
                    try
                    {

                        UserModel userObj = await GetUser(data.DataStorage[0].Uuid);
                        AuthModel tmp = await CreateSessionRecord(userObj);
                        authModel = tmp;
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }
            }
            return authModel;
        }
        public async Task<AuthModel> Refresh(string refresh_token, string token)
        {
            AuthModel authModel = null;

            CheckLoginResponse checkLoginResponse = await CheckLogin(token, true);
            if(checkLoginResponse.IsCorrectUserAgent && checkLoginResponse.IsCorrectIp && checkLoginResponse.IsCorrectToken && !checkLoginResponse.TokenExpired && !checkLoginResponse.RefreshTokenExpired)
            {
                HttpContext context = _httpContextHandler.CurrentContext();

                JWTModel data = _jwtHandler.Decode(token, _appConfig.AppServiceConfiguration.ApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr);

                UserModel userObject = (UserModel)data.UserModel;

                if (userObject != null)
                {
                    UserModel userModel = userObject;

                    authModel = await CreateSessionRecord(userObject);
                }
            }
            return authModel;
        }

        public bool IsUserTypeAdminType(UserTypeModel userTypeModel)
        {
            return userTypeModel.Name =="Administrator" || userTypeModel.Name == BackendAPIDefinitionsProperties.RootName ? true:false;
        }

        public async Task<UserModel> GetUser(Guid userId)
        {
            UserModel userModel = new UserModel { Uuid = userId };
            QueryResponseData<UserModel> data = await _databaseHandler.ExecuteQueryWithMap<UserModel>("SELECT * FROM user_active_view WHERE uuid = @uuid LIMIT 1;", userModel);
            if (data.HasStorageData)
            {
                userModel = data.DataStorage[0];
                userModel.AvaibleRoles = await GetUserRoles(userId);
                userModel.UserType = await GetUserType(userModel.UserTypeUuid);
                userModel.IsAdmin = IsUserTypeAdminType(userModel.UserType);

                return userModel;
            }
            return null;
        }
        public async Task<List<RoleModel>> GetUserRoles(Guid userId)
        {
            List<RoleModel> responseValues = new List<RoleModel>();
            QueryResponseData<RoleModel> data = await _databaseHandler.ExecuteQuery<RoleModel>("SELECT * FROM user_relation_to_role as ur inner join role as r on(r.uuid = ur.role_uuid) WHERE ur.user_uuid = '" + userId + "';");
            if (data.HasStorageData)
            {
                responseValues = data.DataStorage;
            }
            return responseValues;
        }
        public async Task<UserTypeModel> GetUserType(Guid typeId)
        {
            QueryResponseData<UserTypeModel> data = await _databaseHandler.ExecuteQuery<UserTypeModel>("SELECT * FROM user_type WHERE uuid = '" + typeId + "';");
            if (data.HasStorageData)
            {
                return data.DataStorage[0];
            }
            return null;
        }
        public async Task<AuthModel> GetSession(string token)
        {
            QueryResponseData<AuthModel> data = await _databaseHandler.ExecuteQueryWithMap<AuthModel>("SELECT * FROM auth WHERE token = @token LIMIT 1;", new AuthModel { Token = token });
            if (data.HasStorageData)
            {
                AuthModel authModel = data.DataStorage[0];

                UserModel tmp = await GetUser(authModel.UserUuid);
                if (tmp != null)
                {
                    authModel.UserModel = tmp;
                }

                return authModel;
            }
            return null;
        }
        public async Task<List<AuthModel>> GetSessionFromUser(Guid userId)
        {
            QueryResponseData<AuthModel> data = await _databaseHandler.ExecuteQueryWithMap<AuthModel>("SELECT * FROM auth WHERE user_uuid = @user_id;", new AuthModel { UserUuid = userId });
            if (data.HasStorageData)
            {
                return data.DataStorage;
            }
            return new List<AuthModel>();
        }
        public async Task<bool> Logout(string token)
        {
            CheckLoginResponse checkLoginResponse = await CheckLogin(token, true);
            if(checkLoginResponse.IsCorrectIp && checkLoginResponse.IsCorrectToken && checkLoginResponse.IsCorrectUserAgent && !checkLoginResponse.TokenExpired)
            {
                QueryResponseData response = await _databaseHandler.ExecuteQuery<AuthModel>("UPDATE auth SET logout_datetime = NOW() WHERE token = @token", new AuthModel { Token = token });
                if (!response.HasErrors)
                    return true;
            }
            return false;
        }
        public async Task<bool> CheckLogin(string token)
        {
            CheckLoginResponse tmp = await CheckLogin(token, false);
            return tmp.IsCorrectToken;
        }
        public async Task<CheckLoginResponse> CheckLogin(string token,bool checkAuthorisation = false)
        {
            HttpContext context = _httpContextHandler.CurrentContext();
            bool validToken = false;
            bool validUserAgent = false;
            bool validIp = false;
            bool isAuthorizedForUri = false;
            bool tokenExpired = false;
            bool refreshTokenExpired = false;
            bool apiAccessGrant = false;
            bool maxRequPerHour = false;
            bool tomuchRequInShortTime = false;

            //maxRequPerHour & tomuchRequInShortTime wenn TimeBetweenLastRequestViolationCount >= BackendAPIDefinitionsProperties.MaxTimeBetweenRequestViolations dann Bann + bei MaxRequestPerHour
            //bann über globale list im cache und bei Middleware für Author. abfragen vor allen rechen-intensiven tasks um resourcen zu schon vom server

            AuthModel oAuthModel = await GetSession(token);

            if(oAuthModel != null)
            {
                validToken = true;
                tokenExpired = oAuthModel.IsTokenExpired;
                refreshTokenExpired = oAuthModel.IsRefreshTokenExpired;
                if (!oAuthModel.IsTokenExpired && !oAuthModel.IsRefreshTokenExpired)
                {

                    string userAgentCurrentRequest = context.HeaderValueGet("user-agent");
                    if (userAgentCurrentRequest != null && userAgentCurrentRequest == oAuthModel.UserAgent)
                    {
                        validUserAgent = true;
                        bool localEqual = false;
                        bool remoteEqual = false;
                        switch (context.Connection.RemoteIpAddress.AddressFamily)
                        {
                            case System.Net.Sockets.AddressFamily.InterNetwork:
                                remoteEqual = context.Connection.RemoteIpAddress != oAuthModel.IPv4RemoteObject;
                                break;
                            case System.Net.Sockets.AddressFamily.InterNetworkV6:
                                remoteEqual = context.Connection.RemoteIpAddress != oAuthModel.IPv6RemoteObject;
                                break;
                        }
                        switch (context.Connection.LocalIpAddress.AddressFamily)
                        {
                            case System.Net.Sockets.AddressFamily.InterNetwork:
                                localEqual = context.Connection.RemoteIpAddress != oAuthModel.IPv4LocalObject;
                                break;
                            case System.Net.Sockets.AddressFamily.InterNetworkV6:
                                localEqual = context.Connection.RemoteIpAddress != oAuthModel.IPv6LocalObject;
                                break;
                        }
                        if (localEqual && remoteEqual)
                        {

                            validIp = true;
                            if (checkAuthorisation)
                            {
                                UserModel userModel = oAuthModel.UserModel;

                                if (userModel != null)
                                {

                                    apiAccessGrant = userModel.ApiAccessGranted;
                                    string pathUri = _httpContextHandler.GetRoute();
                                    if(userModel.ApiAccessGranted)
                                    {
                                        //if (userModel.IsAuthorizedPath(pathUri, context.Request.Method))
                                        {
                                            isAuthorizedForUri = true;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

            }
            return new CheckLoginResponse(validUserAgent,validIp,isAuthorizedForUri,validToken,tokenExpired,refreshTokenExpired,apiAccessGrant,oAuthModel);
        }

        public async Task<AuthModel> CreateSessionRecord(UserModel userModel)
        {
            AuthModel authModel = new AuthModel();
            DateTime now = DateTime.Now;
            DateTime expiresToken = now + BackendAPIDefinitionsProperties.ExpiresTokenTime;
            DateTime expiresRefreshToken = now+BackendAPIDefinitionsProperties.ExpiresRefreshTokenTime;
            IPAddress ipObj = _httpContextHandler.CurrentContext().Connection.RemoteIpAddress;
            int remotePort = _httpContextHandler.CurrentContext().Connection.RemotePort;
            int localPort = _httpContextHandler.CurrentContext().Connection.LocalPort;
            bool ipIsV4 = ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
            bool ipIsV6 = ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            IPAddress localIpObj = _httpContextHandler.CurrentContext().Connection.LocalIpAddress;
            bool iplocalIsV4 = ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
            bool iplocalIsV6 = ipObj.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            string userAgent = _httpContextHandler.CurrentContext().Request.Headers["User-Agent"].ToString();
            string ip = ipObj == null ? null : ipObj.ToString();
            string ipLocal = localIpObj == null ? null : localIpObj.ToString();
            string token = EncodeJWT(userModel, expiresToken, false);
            string refreshToken = await GenerateRefreshToken(userModel);
            
            authModel.Ipv4 = ipIsV4 ? ip : null;
            authModel.Ipv6 = ipIsV6 ? ip : null;
            authModel.Ipv4Local = iplocalIsV4 ? ipLocal : null;
            authModel.Ipv6Local = iplocalIsV6 ? ipLocal : null;
            authModel.RemotePort = remotePort;
            authModel.LocalPort = localPort;
            authModel.Token = token;
            authModel.UserUuid = userModel.Uuid;
            authModel.RefreshToken =refreshToken;
            authModel.TokenExpires = expiresToken;
            authModel.RefreshTokenExpires = expiresRefreshToken;
            authModel.CreationDateTime = now;
            authModel.UserAgent = userAgent;
            authModel.UserModel = userModel;
            authModel.UserModel.UserType = await GetUserType(userModel.UserTypeUuid);
            authModel.UserModel.IsAdmin = IsUserTypeAdminType(userModel.UserType);
            authModel.IsAdmin = authModel.UserModel.IsAdmin;

            QueryResponseData data = await _databaseHandler.ExecuteQuery<AuthModel>("INSERT INTO `auth` (`uuid`,`ip_addrv4_remote`,`ip_addrv6_remote`,`ip_addrv4_local`,`ip_addrv6_local`,`remote_port`,`local_port`,`token`,`user_uuid`,`token_expires_in`,`user_agent`,`refresh_token`,`refresh_token_expires_in`) VALUES (@uuid,@ip_addrv4_remote,@ip_addrv6_remote,@ip_addrv4_local,@ip_addrv6_local,@remote_port,@local_port,@token,@user_uuid,@token_expires_in,@user_agent,@refresh_token,@refresh_token_expires_in);", authModel);

            if (data.HasErrors)
                return null;

            return authModel;
        }
        #endregion
        public class CheckLoginResponse
        {
            #region Private
            private bool _tokenExpired = false;
            private bool _refreshTokenExpired = false;
            private bool _isCorrectUserAgent = false;
            private bool _isCorrectToken = false;
            private bool _isAuthorizedForPath = false;
            private bool _isCorrectIp = false;
            private bool _apiAccessGrant = false;
            #endregion
            #region Public
            public AuthModel AuthModel { get; private set; }
            public bool IsAuthorizatiOk
            {
                get
                {
                    return (_isAuthorizedForPath && _isCorrectIp && _isCorrectToken && _isCorrectUserAgent && !_tokenExpired && !_refreshTokenExpired);
                }
            }
            public bool IsCorrectUserAgent
            {
                get
                {
                    return _isCorrectUserAgent;
                }
            }
            public bool IsCorrectToken
            {
                get
                {
                    return _isCorrectToken;
                }
            }
            public bool IsAuthorizedForPath
            {
                get
                {
                    return _isAuthorizedForPath;
                }
            }
            public bool IsCorrectIp
            {
                get
                {
                    return _isCorrectIp;
                }
            }
            public bool TokenExpired
            {
                get
                {
                    return _tokenExpired;
                }
            }
            public bool HasApiAccessGrant
            {
                get
                {
                    return _apiAccessGrant;
                }
            }
            public bool RefreshTokenExpired
            {
                get
                {
                    return _refreshTokenExpired;
                }
            }
            #endregion
            #region Ctor
            public CheckLoginResponse(bool user_agent_correct,bool ip_correct,bool is_authorized, bool valid_token,bool token_expired, bool refresh_token_expired,bool apiAccessGrant, AuthModel authModel)
            {
                _tokenExpired = token_expired;
                _refreshTokenExpired = refresh_token_expired;
                _isCorrectUserAgent = user_agent_correct;
                _isCorrectIp = ip_correct;
                _isAuthorizedForPath = is_authorized;
                _isCorrectToken = valid_token;
                _apiAccessGrant = apiAccessGrant;
                AuthModel = authModel;
            }
            #endregion

        }
    }
}
