using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
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

namespace WebApiMailParser.Controllers.APIv2
{
    [Controller]
    [ApiController]
    [Area(GeneralDefs.ApiAreaV1)]
    [Route("[area]/" + BackendAPIDefinitionsProperties.ErrorControllerName + "")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : CustomControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IScopedDatabaseHandler _databaseHandler;
        private readonly IJsonApiDataHandler _jsonApiHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ErrorController(IWebHostEnvironment env, ILogger<ErrorController> logger, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiDataHandler, ICachingHandler caching, Microsoft.Extensions.Configuration.IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig) :
            base(env, caching, configuration, rabbitMqHandler)
        {
            _logger = logger;
            _databaseHandler = databaseHandler;
            _jsonApiHandler = jsonApiDataHandler;
            _webHostEnvironment = env;
        }

        [Route(BackendAPIDefinitionsProperties.ProductiveErrorController)]
        public async Task<ActionResult> Get()
        {
            //INFO: Exceptions cant be serialized by asp, common error comes over by trying: Serialization and deserialization of 'System.Type' instances are not supported and should be avoided since they can lead to security issues
            IExceptionHandlerFeature context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            object exception = context.Error;
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            string preDefinedMessage = "Internal Server Error";
            string debugMsg = null;
            Exception exception1 = (Exception)exception;
            object debugRespObj = exception1.StackTrace;
            ApiErrorModel.ERROR_CODES errCode = ApiErrorModel.ERROR_CODES.ERROR_OCCURRED;
            if (exception is HttpStatusException)
            {
                HttpStatusException exception2 = (HttpStatusException)exception;
                debugMsg = exception2.ResponseDetailMsg;
                code = exception2.Status;
                preDefinedMessage = exception2.Message;
            }
            else
            {
            }
            var response = JsonApiErrorResult(new List<ApiErrorModel>
            {
                new ApiErrorModel{
                 Code = errCode,
                 Detail = preDefinedMessage
                }
            }, code, "an error occurred", debugMsg, debugRespObj);
            return response;
        }

        [Route(BackendAPIDefinitionsProperties.DebugErrorController)]
        public async Task<ActionResult> Error()
        {
            return await Get();
        }


    }
}


