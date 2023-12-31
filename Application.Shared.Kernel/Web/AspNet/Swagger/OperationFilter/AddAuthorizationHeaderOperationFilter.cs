﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Reflection;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService;

namespace Application.Shared.Kernel.Web.AspNet.Swagger.OperationFilter
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
                var endpointAuthAttrs = context.MethodInfo.GetCustomAttributes<AuthorizeAttribute>();
                var  nonAuthhAttrs = context.MethodInfo.GetCustomAttributes<AllowAnonymousAttribute>();
                if (endpointAuthAttrs == null || endpointAuthAttrs.Count() == 0)
                {
                    endpointAuthAttrs = descriptor.ControllerTypeInfo.GetCustomAttributes<AuthorizeAttribute>();  
                }
                if (endpointAuthAttrs!=null && !nonAuthhAttrs.Any())
                {
                    foreach (var endpointAuthAttr in endpointAuthAttrs) 
                    {
                        string endpointAuthDesc = "Authorization Token for HTTP Requests.";
                        if (!String.IsNullOrEmpty(endpointAuthAttr.Policy))
                        {
                            endpointAuthDesc += "\nApplied Policy '" + endpointAuthAttr.Policy + "'";
                        }
                        if (!String.IsNullOrEmpty(endpointAuthAttr.Roles))
                        {
                            endpointAuthDesc += "\nApplied Roles '" + endpointAuthAttr.Roles + "'";
                        }
                        if (!String.IsNullOrEmpty(endpointAuthAttr.AuthenticationSchemes))
                        {
                            endpointAuthDesc += "\nApplied Authentication Schemes '" + endpointAuthAttr + "'";
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
}

      
    