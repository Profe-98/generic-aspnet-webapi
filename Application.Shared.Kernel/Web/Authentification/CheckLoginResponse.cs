using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Web.Authentification
{

    public class CheckLoginResponse
    {
        #region Private
        private bool _tokenExpired = false;
        private bool _refreshTokenExpired = false;
        private bool _isCorrectUserAgent = false;
        private bool _isCorrectIp = false;
        private bool _apiAccessGrant = false;
        #endregion
        #region Public
        public bool IsAuthorizatiOk
        {
            get
            {
                return _isCorrectIp && _isCorrectUserAgent && !_tokenExpired && !_refreshTokenExpired;
            }
        }
        public bool IsCorrectUserAgent
        {
            get
            {
                return _isCorrectUserAgent;
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
        public CheckLoginResponse(bool user_agent_correct, bool ip_correct, bool token_expired, bool refresh_token_expired, bool apiAccessGrant)
        {
            _tokenExpired = token_expired;
            _refreshTokenExpired = refresh_token_expired;
            _isCorrectUserAgent = user_agent_correct;
            _isCorrectIp = ip_correct;
            _apiAccessGrant = apiAccessGrant;
        }
        #endregion
    }
    public class CheckLoginResponse<T> : CheckLoginResponse
        where T : AuthModel, new()
    {
        #region Private
        #endregion
        #region Public
        public T AuthModel { get; private set; }
        
        #endregion
        #region Ctor
        public CheckLoginResponse(bool user_agent_correct, bool ip_correct, bool token_expired, bool refresh_token_expired, bool apiAccessGrant, T authModel) : base(user_agent_correct, ip_correct, token_expired, refresh_token_expired, apiAccessGrant)
        {
        }
        #endregion

    }
}
