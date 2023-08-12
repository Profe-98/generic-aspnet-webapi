using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Application.Shared.Kernel.Application.Controller.Modules;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Security.Encryption;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Infrastructure.Mail;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Infrastructure.Antivirus.nClam;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Web.Api.Shared.Controllers.APIv1
{

    public class AuthController : CustomApiV1ControllerBase<AuthModel>
    {
        public AuthController(ILogger<AuthController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<AuthModel> abstractBackendModule, IServiceProvider serviceProvider) :
           base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {

        }

        public override AbstractBackendModule<AuthModel> GetConcreteModule()
        {
            throw new NotImplementedException();
        }
    }
}
