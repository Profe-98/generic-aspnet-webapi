using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Cors;
using WebApiAuthentificationService;
using WebApiFunction.Web.AspNet.ActionResult;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Controller;
using WebApiFunction.Configuration;
using WebApiFunction.Database;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq;
using System.IO;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;

namespace WebApiAuthentificationService.Controller
{
    [Controller]
    [ApiController]
    [Area("helix-api-1")]
    [Route("[area]/[controller]")]
    public class AuthentificationController : CustomControllerBase
    {

        private readonly ILogger<AuthentificationController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuthHandler _authHandler;
        public AuthentificationController(ILogger<AuthentificationController> logger, IWebHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration configuration, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IActionSelector actionSelector, IAuthHandler authHandler) :
            base(env, configuration, actionDescriptorCollectionProvider, actionSelector)
        {
            _webHostEnvironment = env;
            _logger = logger;
            _authHandler = authHandler;
        }
        [HttpGet("session/{token}")]
        public async Task<ActionResult<AuthModel>> GetSessionInformation(string token)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            AuthModel authModel = null;
            if (token != null)
            {
                authModel = await _authHandler.GetSession(token);
            }
            return authModel == null ? JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.BadRequest, "an error occurred", "authModel == null", methodInfo) : Ok(authModel);
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserDataTransferModel authUserModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            AuthModel authModel = null;
            bool jwtByAuthInit = false;
            if (jwtByAuthInit)
            {
                if (!this.ModelState.IsValid)
                {

                    string authHttpHeaderKey = HttpContext.Request.Headers.Keys.ToList().Find(x => x == "Authorization");
                    if (authHttpHeaderKey == null)
                    {
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Detail = "bad request" }
            }, HttpStatusCode.BadRequest, "an error occurred", "authHttpHeaderKey == null", methodInfo);
                    }
                    if (authHttpHeaderKey != null)
                    {
                        string token = null;
                        string authStr = HttpContext.Request.Headers[authHttpHeaderKey];
                        if (!String.IsNullOrEmpty(authStr))
                        {
                            string[] stringSplitter = authStr.Split(' ');
                            if (stringSplitter.Length == 2)
                            {
                                token = stringSplitter[1];
                            }
                        }
                        authModel = (!String.IsNullOrEmpty(token))
                            ?
                            await _authHandler.Login(HttpContext, token)
                            :
                            null;
                    }
                }
            }
            else
            {
                if (authUserModel != null)
                {
                    if (this.ModelState.IsValid)
                    {
                        authModel = await _authHandler.Login(HttpContext, authUserModel);
                    }
                }
            }
            AreaAttribute currentArea = this.GetArea();
            if (authModel != null)
            {
                return Ok(authModel).Cookie(HttpContext, "refresh_token", authModel.RefreshToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Path = "/" + currentArea?.RouteValue.ToLower(),
                    Secure = true,
                    Expires = authModel.RefreshTokenExpires,

                });
            }
            else
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authModel == null", methodInfo);
            }
        }

        [HttpPost("logout/{token}")]
        public async Task<ActionResult> Logout(string token)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (token != null)
            {
                bool response = await _authHandler.Logout(HttpContext, token);
                if (!response)
                {
                    return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
            }
        }
        [HttpPost("refresh/{refresh_token}")]
        public async Task<ActionResult<AuthModel>> Refresh(string refresh_token)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (refresh_token != null)
            {
                if (CheckGuid(refresh_token))
                {
                    string token = HttpContext.GetRequestJWTFromHeader();
                    if (token != null)
                    {
                        AuthModel authModel = await _authHandler.Refresh(HttpContext, refresh_token, token);
                        return Ok(authModel);
                    }
                }
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Detail = "bad request" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(refresh_token) == false", methodInfo);
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
        }
        [HttpPost("validate")]
        public async Task<ActionResult> Validate([FromBody] AuthentificationTokenModel authentificationTokenModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            var response = await _authHandler.CheckLogin(HttpContext,authentificationTokenModel.Token, true);
            if(response.IsAuthorizatiOk)
            {
                return Ok();
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
        }
    }
}
