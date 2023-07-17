﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.Controller;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MailKit.Net.Smtp;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace WebApiFunction.Web.AspNet.Filter
{
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

                /*string json = null;
                using (JsonHandler jsonHandler = new JsonHandler())
                {
                    json = jsonHandler.JsonSerialize(newSettedObject);
                }
                byte[] data = Encoding.UTF8.GetBytes(json);
                await context.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);*/

                context.Result = objectResultFromController;

            }
            return next();
        }
    }
}
