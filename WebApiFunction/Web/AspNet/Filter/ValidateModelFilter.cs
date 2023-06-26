using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using WebApiFunction.Application.Model.Internal;


using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
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
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiFunction.Web.AspNet.Filter
{
    public class ValidateModelFilter : IAsyncActionFilter
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ValidateModelFilter(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            bool returnStarted = context.HttpContext.Response.HasStarted;
            if (!returnStarted)
            {
                if (!context.ModelState.IsValid)
                {
                    var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY,
                 Detail = BackendAPIDefinitionsProperties.HttpUnproccessableEntity
                }
                }, HttpStatusCode.UnprocessableEntity, "an error occurred", "if (!context.ModelState.IsValid)", methodInfo);
                    context.Result = response;

                }
            }
            else
            {

            }
            if (context.Result == null)
                return next();
            else
                return Task.CompletedTask;
        }
    }
}
