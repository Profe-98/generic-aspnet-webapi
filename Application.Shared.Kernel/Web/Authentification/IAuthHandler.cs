using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Application.Shared.Kernel.Web.Authentification.JWT;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Web.Authentification
{
    public interface IAuthHandler
    {
        public JWTModel DecodeJWT(string token);
        public string EncodeJWT(UserModel userModel, DateTime expiresToken, bool forRegistration);
        public Task<CheckLoginResponse<AuthModel>> CheckLogin(HttpContext httpContext, string token);
        public Task<bool> Logout(HttpContext httpContext, string token);
        public Task<AuthModel> Login(HttpContext httpContext, UserDataTransferModel userModel);
        public Task<AuthModel> Refresh(HttpContext httpContext, string refresh_token, string token);
        public Task<AuthModel> GetSession(string token);
        public Task<AuthModel> CreateSessionRecord(HttpContext httpContext, UserModel userModel);
    }
}
