using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApiApplicationService.InternalModels
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

            WebApiApplicationService.Models.InternalModels.MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new WebApiApplicationService.Models.InternalModels.MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
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
                }, System.Net.HttpStatusCode.UnprocessableEntity, "an error occurred", "if (!context.ModelState.IsValid)", methodInfo);
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
