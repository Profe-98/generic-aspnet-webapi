using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Configuration;
using WebApiFunction.Controller;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Database;
using WebApiFunction.Mail;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading.Service;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;

namespace JellyFishBackend.Controller
{
    [Controller]
    [ApiController]
    [Area("jelly-api-1")]
    [Route("[area]/[controller]")]
    public abstract class AbstractController<T, T2> : CustomApiV1ControllerBase<T, T2> where T : AbstractModel
        where T2 : AbstractBackendModule<T>
    {
        protected AbstractController(ILogger<CustomApiControllerBase<T, T2>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, MysqlDapperContext mysqlDapperContext) : base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, mysqlDapperContext)
        {
        }
    }
}
