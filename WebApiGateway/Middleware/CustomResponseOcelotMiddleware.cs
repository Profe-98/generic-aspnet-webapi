using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http;
using System.Threading;
using WebApiGateway;
using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Ocelot.Responses;
using Ocelot.Logging;
using Ocelot.Errors;
using Ocelot.Infrastructure.Extensions;
using Ocelot.Responder;
using Ocelot.Requester;

namespace WebApiGateway.Middleware
{
    public class CustomResponseOcelotMiddleware : Ocelot.Middleware.OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpRequester _httpRequester;
        private readonly IHttpResponder _responder;
        private readonly IErrorsToHttpStatusCodeMapper _codeMapper;

        public CustomResponseOcelotMiddleware(
            RequestDelegate next,
            IHttpRequester httpRequester,
            IHttpResponder responder,
            IErrorsToHttpStatusCodeMapper codeMapper,
            IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<CustomResponseOcelotMiddleware>())
        {
            _next = next;
            _responder = responder;
            _codeMapper = codeMapper;
            _httpRequester = httpRequester;
        }
    

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Response.HasStarted)
                return;

            var errors = httpContext.Items.Errors();
            if (errors.Count > 0)
            {
                Logger.LogWarning($"{errors.ToErrorString()} errors found in {MiddlewareName}. Setting error response for request path:{httpContext.Request.Path}, request method: {httpContext.Request.Method}");

                var statusCode = _codeMapper.Map(errors);
                var error = string.Join(",", errors.Select(x => x.Message));
                httpContext.Response.StatusCode = statusCode;
                // output error
                await httpContext.Response.WriteAsync(error);
            }
            else
            {
                Logger.LogDebug("no pipeline errors, setting and returning completed response");

                var downstreamResponse = httpContext.Items.DownstreamResponse();

                await _responder.SetResponseOnHttpContext(httpContext, downstreamResponse);
                await _next.Invoke(httpContext);
            }
        }
    }
}
