using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Handler;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection;
using System.Net;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using WebApiFunction.Web.AspNet.ActionResult;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;

namespace WebApiApplicationService.Controllers.APIv1
{
    
    public class UserController : CustomApiV1ControllerBase<UserModel, UserModule>
    {
        private readonly IAuthHandler _auth;
        private readonly IScopedEncryptionHandler _encryptionHandler;
        private readonly IConfiguration _configuration;
        public UserController(ILogger<UserController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IActionSelector actionSelector, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env,IConfiguration configuration, IScopedEncryptionHandler encryptionHandler,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler,MysqlDapperContext mysqlDapperContext) :
            base(logger, vulnerablityHandler,mailHandler,authHandler,databaseHandler,jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler,mysqlDapperContext)
        {
            _auth = authHandler; _configuration = configuration;
            _encryptionHandler = encryptionHandler;
        }
        #region HttpMethods

        [HttpGet("{id}")]
        //[HttpGet("profile/{id}")]
        public override Task<ActionResult<ApiRootNodeModel>> Get(string id,int maxDepth=1)
        {

            return base.Get(id, maxDepth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [CustomConsumesFilter(GeneralDefs.ImageContentType)]
        //is attributed to generate two types of response, json for error reporting and image/jpeg for content delivery
        [CustomProducesFilter(GeneralDefs.ImageContentType,GeneralDefs.ApiContentType)]
        [HttpGet(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.UserProfilePictureRoute)]
        //[HttpGet("profile/picture/{id}")]
        public async Task<ActionResult> GetUserProfilePicture(string id)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (!CheckGuid(id))
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a valid id" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(id) == false", methodInfo);
            }
            Guid uuid = new Guid(id);

            ActionResult<ApiRootNodeModel> result = await Get(id);
            if (result.Result != null)
            {
                Type t = result.Result.GetType();

                if (t == typeof(OkObjectResult))
                {
                    OkObjectResult okObject = (OkObjectResult)result.Result;
                    switch (okObject.StatusCode)
                    {
                        case 200:
                            ApiRootNodeModel apiRootNodeModel = (ApiRootNodeModel)okObject.Value;
                            ApiDataModel apiDataModel = (ApiDataModel)apiRootNodeModel.Data;
                            UserModel userModel = (UserModel)apiDataModel.Attributes;
                            if(userModel.HasProfilePicture)
                            {
                                string fileName = userModel.Uuid+"_avatar"+userModel.UserProfilePicFileExtension;
                                string userProfilePicPath = Path.Combine(_appConfig.AppServiceConfiguration.AppPaths[AppConfigDefinitionProperties.PathDictKeys.File.UserProfilePath], fileName);
                                if(System.IO.File.Exists(userProfilePicPath))
                                {
                                    byte[] binary = System.IO.File.ReadAllBytes(userProfilePicPath);
                                    return new FileContentResult(binary, GeneralDefs.ImageContentType);
                                }
                            }
                            return JsonApiErrorResult(new List<ApiErrorModel> { 
                                new ApiErrorModel { 
                                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND,
                                    Detail = "not found"
                                } }, HttpStatusCode.NotFound,"an error occurred","user found but has no profile picture");
                            break;
                        default:
                            break;
                    }
                }
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                    Detail = "forbidden"
                } }, HttpStatusCode.Forbidden, "an error occurred", "no permissions or data not found") ;
        }
        #region Administrator things

        /// <summary>
        /// Action for Admin:
        /// - List all Users (Get(bool givenIdMustBeUserUuid = true))
        /// - Select User from Userlist and show Profile (Get(string id,bool givenIdMustBeUserUuid = true))
        /// - Add User to Role (HttpPatch: UpdateRelation(string id, string relationname, [FromBody] ApiRootNodeModel body))
        /// - Remove User from Role (HttpPost with empty collection in body: UpdateRelation(string id, string relationname, [FromBody] ApiRootNodeModel body))
        /// - Delete User (HttpDelete:  public virtual async Task<ActionResult<ApiRootNodeModel>> Delete(string id))
        /// - Add User (HttpPost: public virtual async Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body))
        /// - Resend Token for User
        /// - Reset Password for User
        /// - Activate User
        /// - Change properties for User
        /// </summary>
        [HttpGet]
        //[HttpGet("profile/{id}")]
        public override async Task<ActionResult<ApiRootNodeModel>> Get()
        {
            return await this.Get(Guid.Empty.ToString(),  1);
        }

        /*[CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpPost("{id}/relation/{relationname}")]//reset&clear relations of given id and add new collection from given body(json-object/array)
        [HttpPatch("{id}/relation/{relationname}")]//adding collection from given body(json-object/array)
        [AuthorizationFilter(BackendAPIDefinitionsProperties.RootRoleName)]
        public override async Task<ActionResult<ApiRootNodeModel>> UpdateRelation(string id, string relationname, [FromBody] ApiRootNodeModel body)
        {
            return await base.UpdateRelation(id,relationname,body);
        }*/
        #endregion Administrator things
        #region Register and Activation of Account

        [HttpPost("register")]
        public async Task<ActionResult> RegisterFrontEndUser([FromBody] RegisterDataTransferModel registerModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            string feUrl = _configuration.GetValue<string>("Frontend");
            string registerActivationEndpoint = _configuration.GetValue<string>("FrontEnd_Register");
            string fromMail = _configuration.GetValue<string>("RegisterFromMail");
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

                        userModel.SendActivationMail(_mailHandler, fromMail, feUrl, registerActivationEndpoint);
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
                bool accountInsertErrors = false;
                Guid accountUuid = Guid.Empty;

                using (CustomBackendModule<AccountModel> a = new CustomBackendModule<AccountModel>(_databaseHandler, _cacheHandler,_mysqlDapperContext))
                {
                    AccountModel data = new AccountModel { User = registerModel.EMail, CommunicationMediumUuid = BackendAPIDefinitionsProperties.CommunicationMedium[BackendAPIDefinitionsProperties.CmKeyFrontEndUser] };
                    var accountSearchResponse = await a.Select(data, data);

                    if (!accountSearchResponse.HasData)//account row existiert nicht, neuanlage
                    {
                        var accountInsertResponse = await a.Insert(data, transaction);

                        accountInsertErrors = accountInsertResponse.HasErrors;

                        accountUuid = new Guid((string)accountInsertResponse.LastInsertedId);
                    }
                    else
                    {
                        accountInsertErrors = accountSearchResponse.HasErrors;
                        accountUuid = accountSearchResponse.FirstRow.Uuid;
                    }
                }
                UserModel userModel = new UserModel(registerModel);
                string actCode = userModel.GenerateCode();

                string base64 = _auth.EncodeJWT(new UserModel() { User = registerModel.EMail, FirstName = registerModel.FirstName, LastName = registerModel.LastName }, timeExp, true);

                userModel.AccountUuid = accountUuid;//account uuid
                userModel.ActivationCode = actCode;
                userModel.ActivationToken = base64;
                userModel.ActivationTokenExpires = timeExp;
                userModel.UserTypeUuid = new Guid("7340425e-a5b5-11eb-bac0-309c2364fdb6");
                userModel.Active = false;
                userModel.Password = await _encryptionHandler.MD5Async(userModel.Password);
                userModel.Password = userModel.Password.ToLower();
                //object[] data = _auth.DecodeJWT(base64);
                var insertResponse = await _backendModule.Insert(userModel, transaction);
                if (insertResponse.HasErrors || accountInsertErrors)
                {
                    _backendModule.Rollback(transaction);
                }
                else
                {
                    _backendModule.Commit(transaction);
                    userModel.SendActivationMail(_mailHandler, fromMail, feUrl, registerActivationEndpoint);
                    return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = new Guid((string)insertResponse.LastInsertedId), Attributes = registerModel } }, HttpStatusCode.OK);
                }

            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "unknown error", methodInfo);
        }
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

        #endregion Register and Activation of Account
        #endregion HttpMethods


    }
}
