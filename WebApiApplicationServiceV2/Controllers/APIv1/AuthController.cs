using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
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
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
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
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Application.Model.Database.MySQL.Table;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiApplicationService.Controllers.APIv1
{

    public class AuthController : CustomApiV1ControllerBase<AuthModel>
    {
        public AuthController(ILogger<AuthController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler,IAbstractBackendModule<AuthModel> abstractBackendModule, IServiceProvider serviceProvider) :
           base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {

        }

        public override AbstractBackendModule<AuthModel> GetConcreteModule()
        {
            throw new NotImplementedException();
        }
    }
}
