using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Shared.Kernel.Web.AspNet.Filter
{
    /// <summary>
    /// Removes Properties in Object that are marked with the SensitiveDataAttribute when the User Identity Claims not contain a specific role that is defined in SensitiveDataAttribute
    /// </summary>
    public class ContextualResponseSerializerFilter : IAsyncResultFilter
    {
        private readonly ILogger<ContextualResponseSerializerFilter> _logger;
        public int Order { get; } = int.MinValue;

        public ContextualResponseSerializerFilter(ILogger<ContextualResponseSerializerFilter> logger)
        {
            _logger = logger;
        }
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var userClaims = context.HttpContext.User.Claims.ToList();
            var objectResultFromController = context.Result as ObjectResult;
            if (objectResultFromController != null)
            {
                var newSettedObject = objectResultFromController.Value.SetSensitivePropertiesToDefault(userClaims);
                objectResultFromController.Value = newSettedObject;


                context.Result = objectResultFromController;

            }
            return next();
        }
    }
}
