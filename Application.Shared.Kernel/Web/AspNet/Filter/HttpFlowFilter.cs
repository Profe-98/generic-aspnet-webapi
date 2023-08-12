using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using Application.Shared.Kernel.Web.AspNet.Controller;

namespace Application.Shared.Kernel.Web.AspNet.Filter
{
    public class HttpFlowFilter : IAsyncActionFilter, IResultFilter, IOrderedFilter
    {
        private readonly ILogger<ContextualResponseSerializerFilter> _logger;
        public int Order { get; } = int.MinValue;

        public HttpFlowFilter(ILogger<ContextualResponseSerializerFilter> logger)
        {
            _logger = logger;
        }
        //Bevor Action ausgeführt wird, pre-request-execution
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, GetType().Name);
            return next();
        }
        /// <summary>
        /// Wenn Action ausgeführt wurde, Result Callback
        /// </summary>
        /// <param name="context"></param>
        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, GetType().Name);
        }
        /// <summary>
        /// Während Execution
        /// </summary>
        /// <param name="context"></param>
        public void OnResultExecuting(ResultExecutingContext context)
        {

        }
    }
}
