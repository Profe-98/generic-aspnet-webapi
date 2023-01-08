using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models;
using Microsoft.AspNetCore.Http;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Models.DataTransferObject;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService
{

    public interface IAuthHandler
    {
        public JWTModel DecodeJWT(string token);
        public string EncodeJWT(UserModel userModel, DateTime expiresToken, bool forRegistration);
        public Task<bool> CheckLogin(string token);
        public Task<AuthHandler.CheckLoginResponse> CheckLogin(string token, bool checkAuthorisation);
        public Task<bool> Logout(string token);
        public Task<AuthModel> Login(string token);
        public Task<AuthModel> Login(UserDataTransferModel userModel);
        public Task<AuthModel> Refresh(string refresh_token,string token);
        public Task<AuthModel> GetSession(string token);
        public UserModel PredictedCurrentUser { get; set; }
        public Task<AuthModel> CreateSessionRecord(UserModel userModel);
    }
}
