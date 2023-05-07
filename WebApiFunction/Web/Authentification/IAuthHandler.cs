using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;

namespace WebApiFunction.Web.Authentification
{

    public interface IAuthHandler
    {
        public JWTModel DecodeJWT(string token);
        public string EncodeJWT(UserModel userModel, DateTime expiresToken, bool forRegistration);
        public Task<bool> CheckLogin(HttpContext httpContext, string token);
        public Task<AuthHandler.CheckLoginResponse> CheckLogin(HttpContext httpContext, string token, bool checkAuthorisation);
        public Task<bool> Logout(HttpContext httpContext, string token);
        public Task<AuthModel> Login(HttpContext httpContext, string token);
        public Task<AuthModel> Login(HttpContext httpContext, UserDataTransferModel userModel);
        public Task<AuthModel> Refresh(HttpContext httpContext, string refresh_token, string token);
        public Task<AuthModel> GetSession(string token);
        public UserModel PredictedCurrentUser { get; set; }
        public Task<AuthModel> CreateSessionRecord(HttpContext httpContext, UserModel userModel);
    }
}
