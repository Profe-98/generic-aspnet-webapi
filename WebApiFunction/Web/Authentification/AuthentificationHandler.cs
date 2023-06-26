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
using Microsoft.AspNetCore;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.CustomActionResult;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Configuration;

namespace WebApiFunction.Web.Authentification
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
    }
    public class BasicRemoteAuthenticationOptions : RemoteAuthenticationOptions
    {
    }

    public class RemoteAuthentificationRequester : DelegatingHandler
    {
        private readonly string _headerValue;
        public RemoteAuthentificationRequester(HttpMessageHandler inner, string headerValue)
            : base(inner)
        {
            _headerValue = headerValue;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", _headerValue);
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class RemoteAuthentificationHandler : RemoteAuthenticationHandler<BasicRemoteAuthenticationOptions>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthHandler _authHandler;
        private readonly IOptionsMonitor<BasicRemoteAuthenticationOptions> _options;

        public RemoteAuthentificationHandler(IOptionsMonitor<BasicRemoteAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthHandler customAuthenticationManager, IHttpClientFactory httpClientFactory)
            : base(options, logger, encoder, clock)
        {

            _httpClientFactory = httpClientFactory;
            _authHandler = customAuthenticationManager;
            _options = options;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string token = authorizationHeader.Substring("bearer".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            try
            {

                return await ValidateTokenHandleRequestResult(token);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return HandleRequestResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return HandleRequestResult.NoResult();
            }

            if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return HandleRequestResult.Fail("Unauthorized");
            }

            string token = authorizationHeader.Substring("bearer".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return HandleRequestResult.Fail("Unauthorized");
            }

            try
            {
                return await ValidateTokenHandleRequestResult(token);
            }
            catch (Exception ex)
            {
                return HandleRequestResult.Fail(ex.Message);
            }
        }

        private AuthenticateResult ValidateTokenAuthenticateResult(string token)
        {

            var identity = new ClaimsIdentity(null, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        private async Task<HandleRequestResult> ValidateTokenHandleRequestResult(string token)
        {

            var client = _httpClientFactory.CreateClient("RemoteAuthentificationServiceClient");
            var response = await client.PostAsync(requestUri: client.BaseAddress, content: new StringContent(token));

            var identity = new ClaimsIdentity(null, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return HandleRequestResult.Success(ticket);
        }
    }


    public class AuthentificationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly IAuthHandler _authHandler;
        private readonly IScopedJsonHandler _jsonHandler;
        private readonly IHttpContextHandler _httpContextHandler;
        private readonly IOptionsMonitor<BasicAuthenticationOptions> _options;

        public AuthentificationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, IHttpContextHandler httpContextHandler, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthHandler customAuthenticationManager, /*IHttpClientFactory httpClientFactory, */IScopedJsonHandler jsonHandler)
            : base(options, logger, encoder, clock)
        {
            _jsonHandler = jsonHandler;
            _authHandler = customAuthenticationManager;
            _options = options;
            _httpContextHandler = httpContextHandler;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);
            }

            if (!authorizationHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);
            }

            string token = authorizationHeader.Substring("bearer".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);
            }

            var resp = AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);
            try
            {
                resp = await ValidateTokenAuthenticateResult(token);

            }
            catch (Exception ex)
            {
                return AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);
            }
            if(resp != null && !resp.Succeeded)
            {
                if(this.Context.WebSockets.IsWebSocketRequest)
                {
                    try
                    {
                        var webSocket = await this.Context.WebSockets.AcceptWebSocketAsync();
                        await webSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "forbidden", CancellationToken.None);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
            return resp;
        }


        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            return base.HandleForbiddenAsync(properties);
        }


        private async Task<AuthenticateResult> ValidateTokenAuthenticateResult(string token)
        {
            JWTModel data = _authHandler.DecodeJWT(token);
            var result = await _authHandler.CheckLogin(this.Context, token);
            if (!result.IsAuthorizatiOk)
            {
                return AuthenticateResultExtension.FailEx("unauthorized", _jsonHandler);
            }
            var claims = data.Payload.TokenInstance.Payload.Claims.ToList();
            if(claims != null)
            {

            }
            var identity = new ClaimsIdentity(claims, Scheme.Name);

            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
