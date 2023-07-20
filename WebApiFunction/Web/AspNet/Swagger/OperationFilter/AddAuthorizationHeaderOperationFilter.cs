using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Reflection;
using WebApiFunction.Web.Websocket.SignalR.HubService;

namespace WebApiFunction.Web.AspNet.Swagger.OperationFilter
{
    public class AddAuthorizationHeaderOperationFilter : IOperationFilter
    {
        private readonly List<EndpointDataSource> _endpoints;
        public AddAuthorizationHeaderOperationFilter(IEnumerable<EndpointDataSource> endpointSources)
        {
            _endpoints = endpointSources.ToList();
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                var endpointAuthAttr = context.MethodInfo.GetCustomAttribute<AuthorizeAttribute>();
                if (endpointAuthAttr!=null)
                {
                    string endpointAuthDesc = "Authorization Token for HTTP Requests.";
                    if(!String.IsNullOrEmpty(endpointAuthAttr.Policy))
                    {
                        endpointAuthDesc += "\nApplied Policy '"+ endpointAuthAttr.Policy + "'";
                    }
                    if(!String.IsNullOrEmpty(endpointAuthAttr.Roles))
                    {
                        endpointAuthDesc += "\nApplied Roles '"+endpointAuthAttr.Roles+"'";
                    }
                    if(!String.IsNullOrEmpty(endpointAuthAttr.AuthenticationSchemes))
                    {
                        endpointAuthDesc += "\nApplied Authentication Schemes '"+endpointAuthAttr+"'";
                    }
                    operation.Parameters.Add(new OpenApiParameter()
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Description = endpointAuthDesc,
                        Required = true
                    });
                }
            }
        }
    }
}

      
    