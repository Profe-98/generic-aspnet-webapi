using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Reflection;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Web.AspNet.Filter
{
    public class CustomConsumesFilter : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<CustomConsumesFilter> _logger;
        public int Order { get; set; } = int.MinValue + 1;
        public readonly string ContentType = null;
        public readonly string[] OtherContentTypes = null;
        public CustomConsumesFilter()
        {

        }
        public CustomConsumesFilter(ILogger<CustomConsumesFilter> logger)
        {

            _logger = logger;
        }
        public CustomConsumesFilter(string contentType, params string[] otherTypes) : this(new LoggerFactory().CreateLogger<CustomConsumesFilter>())
        {
            ContentType = contentType;

            OtherContentTypes = otherTypes;
        }
        //Bevor Action ausgeführt wird, pre-request-execution um Content-Type zu prüfen
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, GetType().Name);
            bool returnStarted = context.HttpContext.Response.HasStarted;
            if (!returnStarted)
            {
                string contentType = context.HttpContext.Request.ContentType;
                if (string.IsNullOrEmpty(contentType))
                {

                    var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_MEDIA_TYPE_NOT_SUPPORTED,
                 Detail = BackendAPIDefinitionsProperties.HttpRequestWrongContentType
                }
                }, HttpStatusCode.UnsupportedMediaType, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestWrongContentType, null);
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
                }, HttpStatusCode.UnsupportedMediaType, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestWrongContentType, null);
                        context.Result = response;
                    }
                }
            }
            if (context.Result == null)
            {
                return next();

            }
            return Task.CompletedTask;
        }
    }
}
