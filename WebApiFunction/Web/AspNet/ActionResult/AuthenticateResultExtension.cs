using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.Healthcheck;
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
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Web.AspNet.ActionResult
{
    public static class AuthenticateResultExtension
    {
        public static AuthenticateResult FailEx(string message, IJsonHandler jsonHandler)
        {

            return CreateErrorAuthentificateResult(message, jsonHandler);
        }
        private static AuthenticateResult CreateErrorAuthentificateResult(string message, IJsonHandler jsonHandler)
        {
            var model = new ApiRootNodeModel()
            {
                Data = null,
                Errors = new List<ApiErrorModel> { new ApiErrorModel { Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNAUTHORIZED, Detail = message } },
                Meta = new ApiMetaModel
                {
                    Count = 1,
                    OptionalMessage = message,
                },
                Jsonapi = ApiRootNodeModel.GetApiInformation()
            };
            string json = jsonHandler.JsonSerialize(model);
            return AuthenticateResult.Fail(json);
        }
    }
}
