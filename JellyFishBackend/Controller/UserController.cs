using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using WebApiFunction.Application.Model.Database.MySql.Jellyfish;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Controller;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Application.Controller.Modules.Jellyfish;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Mail;
using WebApiFunction.Database;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Threading.Service;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Configuration;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;
using Microsoft.AspNetCore.Authorization;
using WebApiFunction.Filter;
using System.Data.Common;
using WebApiFunction.Web.Authentification.JWT;

namespace JellyFishBackend.Controller
{
    [Authorize]
    public class UserController : AbstractController<UserModel, UserModule>
    {
        private readonly IAuthHandler _auth;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionHandler _encryptionHandler;

        public UserController(ILogger<CustomApiControllerBase<UserModel, UserModule>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, MysqlDapperContext mysqlDapperContext) : base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, mysqlDapperContext)
        {
            _auth = authHandler;
            _configuration = configuration;
            _encryptionHandler = scopedEncryptionHandler;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterFrontEndUser([FromBody] RegisterDataTransferModel registerModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            string registrationActivationUrl = _configuration.GetValue<string>("RegistrationActivationUrl");
            string fromMail = _configuration.GetValue<string>("NoReplyEmail");
            var timeExp = DateTime.Now.AddDays(BackendAPIDefinitionsProperties.RegisterActivationExpInDays);
            var userSearchResponse = await _backendModule.Select(new UserModel { User = registerModel.EMail }, new UserModel { User = registerModel.EMail });
            if (userSearchResponse.HasData)//user existiert bereits mit email
            {
                if (!userSearchResponse.FirstRow.Active)//user bereits in db jedoch noch nicht aktiviert
                {
                    if (!registerModel.ResendActivationCode)
                    {

                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_GONE, Detail = "gone" }
            }, HttpStatusCode.Gone, "an error occurred", "action is still not longer available because, user must be activate", methodInfo);
                    }
                    else
                    {
                        UserModel userModel = userSearchResponse.FirstRow;
                        userModel.SetRegisterDataTransferModel(registerModel);

                        string actCode = userModel.GenerateCode();
                        string base64 = _auth.EncodeJWT(new UserModel() { User = registerModel.EMail, FirstName = registerModel.FirstName, LastName = registerModel.LastName }, timeExp, true);

                        userModel.ActivationCode = actCode;
                        userModel.ActivationToken = base64;
                        userModel.ActivationTokenExpires = timeExp;

                        userModel.SendActivationMail(_mailHandler, fromMail, registrationActivationUrl);
                        var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
                        if (queryResponseFromUpdate.HasSuccess)
                        {
                            return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = registerModel } }, HttpStatusCode.OK);
                        }
                        else
                        {
                            throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
                        }
                    }
                }

            }
            else//user existert nicht
            {
                DbTransaction transaction = await _backendModule.CreateTransaction();


                UserModel userModel = new UserModel(registerModel);
                string actCode = userModel.GenerateCode();

                string base64 = _auth.EncodeJWT(new UserModel() { User = registerModel.EMail, FirstName = registerModel.FirstName, LastName = registerModel.LastName }, timeExp, true);

                userModel.ActivationCode = actCode;
                userModel.ActivationToken = base64;
                userModel.ActivationTokenExpires = timeExp;
                userModel.UserTypeUuid = new Guid("7340425e-a5b5-11eb-bac0-309c2364fdb6");
                userModel.Active = false;
                userModel.Password = await _encryptionHandler.MD5Async(userModel.Password);
                userModel.Password = userModel.Password.ToLower();
                //object[] data = _auth.DecodeJWT(base64);
                var insertResponse = await _backendModule.Insert(userModel, transaction);
                if (insertResponse.HasErrors )
                {
                    _backendModule.Rollback(transaction);
                }
                else
                {
                    _backendModule.Commit(transaction);
                    userModel.SendActivationMail(_mailHandler, fromMail, registrationActivationUrl);
                    return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = new Guid((string)insertResponse.LastInsertedId), Attributes = registerModel } }, HttpStatusCode.OK);
                }

            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "unknown error", methodInfo);
        }
        [AllowAnonymous]
        [HttpGet("activation/{base64}")]
        public async Task<ActionResult> CheckActivation(string base64)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            UserModel userModel = new UserModel();
            userModel.Active = false;
            userModel.ActivationToken = base64;

            var userSearchResponse = await _backendModule.Select(userModel, userModel);
            if (!userSearchResponse.HasData)
            {

                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_GONE, Detail = "gone" }
            }, HttpStatusCode.Gone, "an error occurred", "action is still not longer available because, user is activated", methodInfo);
            }
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("activation/{base64}")]
        public async Task<ActionResult> Activation(string base64, [FromBody] UserActivationDataTransferModel userActivationDataTransferModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            UserModel userModel = new UserModel();
            userModel.Active = false;
            userModel.ActivationToken = base64;
            userModel.ActivationCode = userActivationDataTransferModel.ActivationCode;

            var userSearchResponse = await _backendModule.Select(userModel, userModel);
            if (userSearchResponse.HasData)
            {
                JWTModel data = _auth.DecodeJWT(base64);
                if (data != null)
                {
                    userModel = userSearchResponse.FirstRow;
                    userModel.ActivationTokenExpires = ((UserModel)data.UserModel).ActivationTokenExpires;
                    if (DateTime.Now >= userModel.ActivationTokenExpires)//token abgelaufen
                    {
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "DateTime.Now >= userModel.ActivationTokenExpires", methodInfo);
                    }
                    else//token nicht abgelaufen
                    {
                        if (userModel.Active)
                        {
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_GONE, Detail = "gone" }
            }, HttpStatusCode.Gone, "an error occurred", "action is still not longer available because, user is activated", methodInfo);
                        }

                        userModel.Active = true;
                        var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
                        if (queryResponseFromUpdate.HasSuccess)
                        {
                            string fromMail = _configuration.GetValue<string>("RegisterFromMail");
                            string name = _configuration.GetValue<string>("WebPageName");
                            string body = userModel.GenerateActivationCompleteMailBody();
                            userModel.SendMail(_mailHandler, fromMail, "Welcome to " + name + "", body);
                            return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = userModel } }, HttpStatusCode.OK);
                        }
                        else
                        {
                            throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
                        }
                    }
                }
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "userSearchResponse.HasData == false", methodInfo);
        }

    }
}
