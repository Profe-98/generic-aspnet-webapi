using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Models;
using WebApiApplicationService.Modules;
using WebApiApplicationService.Attribute;
using System.Net;
using WebApiApplicationService.Handler;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;

namespace WebApiApplicationService.Controllers.APIv2
{
    [Controller]
    [ApiController]
    [Area(GeneralDefs.ApiAreaV1)]
    [Route("[area]/"+BackendAPIDefinitionsProperties.ErrorControllerName+"")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : CustomControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IScopedDatabaseHandler _databaseHandler;
        private readonly IJsonApiDataHandler _jsonApiHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ErrorController(IWebHostEnvironment env, ILogger<ErrorController> logger,IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiDataHandler,ICachingHandler caching, Microsoft.Extensions.Configuration.IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig) : 
            base(env,caching,configuration,rabbitMqHandler)
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
                ExceptionHandler.ReportException(exception2.Source, exception2, AppManager.MESSAGE_LEVEL.LEVEL_CRITICAL);
            }
            else
            {
                ExceptionHandler.ReportException(exception1.Source,exception1, AppManager.MESSAGE_LEVEL.LEVEL_CRITICAL);
            }
            var response = JsonApiErrorResult(new List<ApiErrorModel>
            {
                new ApiErrorModel{
                 Code = errCode,
                 Detail = preDefinedMessage
                }
            }, code, "an error occurred", debugMsg,debugRespObj);
            return response; 
        }

        [Route(BackendAPIDefinitionsProperties.DebugErrorController)]
        public async Task<ActionResult> Error()
        {
            return await Get();
        }


    }
}
