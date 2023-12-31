﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Internal;

namespace Application.Web.Gateway.Controller
{
    [Controller]
    [ApiController]
    [Area(GeneralDefs.ApiAreaV1)]
    [Route("[area]/" + BackendAPIDefinitionsProperties.ErrorControllerName + "")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : CustomControllerBase
    {
        private readonly ILogger<ErrorController> _logger;


        public ErrorController(ILogger<ErrorController> logger) :
            base()
        {
            _logger = logger;
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
