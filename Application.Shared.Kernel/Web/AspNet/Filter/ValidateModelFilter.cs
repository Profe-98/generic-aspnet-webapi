using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Web.AspNet.Filter
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
