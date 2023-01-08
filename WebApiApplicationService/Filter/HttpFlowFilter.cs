using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace WebApiApplicationService.InternalModels
{
    public class HttpFlowFilter : IAsyncActionFilter, IResultFilter, IOrderedFilter
    {
        private readonly ILogger<HttpFlowFilter> _logger;
        public int Order { get; } = int.MinValue;

        public HttpFlowFilter(ILogger<HttpFlowFilter> logger)
        {
            _logger = logger;
        }
        //Bevor Action ausgeführt wird, pre-request-execution
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, this.GetType().Name);
            return next();
        }
        /// <summary>
        /// Wenn Action ausgeführt wurde, Result Callback
        /// </summary>
        /// <param name="context"></param>
        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, this.GetType().Name);
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
