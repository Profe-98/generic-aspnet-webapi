using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Net;
using WebApiApplicationService.Models;
using WebApiApplicationService;
using System.Reflection;

namespace WebApiApplicationService.InternalModels
{
    public class CustomConsumesFilter : System.Attribute, IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<CustomConsumesFilter> _logger;
        public int Order { get; set; } = int.MinValue+1;
        public readonly string ContentType = null;
        public readonly string[] OtherContentTypes = null;
        public CustomConsumesFilter()
        {

        }
        public CustomConsumesFilter(ILogger<CustomConsumesFilter> logger)
        {

            _logger = logger;
        }
        public CustomConsumesFilter(string contentType,params string[] otherTypes) : this(new LoggerFactory().CreateLogger<CustomConsumesFilter>())
        {
            ContentType = contentType;
            
            OtherContentTypes = otherTypes;
        }
        //Bevor Action ausgeführt wird, pre-request-execution um Content-Type zu prüfen
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, this.GetType().Name);
            bool returnStarted = context.HttpContext.Response.HasStarted;
            if (!returnStarted)
            {
                string contentType = context.HttpContext.Request.ContentType;
                if (String.IsNullOrEmpty(contentType))
                {

                    var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_MEDIA_TYPE_NOT_SUPPORTED,
                 Detail = BackendAPIDefinitionsProperties.HttpRequestWrongContentType
                }
                }, System.Net.HttpStatusCode.UnsupportedMediaType, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestWrongContentType, null);
                    context.Result = response;
                }
                else
                {
                    contentType = contentType.ToLower();
                    if (!contentType.StartsWith(ContentType.ToLower()) || OtherContentTypes.Length != 0 && OtherContentTypes.ToList().IndexOf(contentType) == GeneralDefs.NotFoundResponseValue)
                    {
                        var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_MEDIA_TYPE_NOT_SUPPORTED,
                 Detail = BackendAPIDefinitionsProperties.HttpRequestWrongContentType
                }
                }, System.Net.HttpStatusCode.UnsupportedMediaType, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestWrongContentType, null);
                        context.Result = response;
                    }
                }
            }
            if(context.Result == null)
            {
                return next();

            }
            return Task.CompletedTask;
        }
    }
}
