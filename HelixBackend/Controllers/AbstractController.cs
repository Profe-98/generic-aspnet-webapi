using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
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
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Database;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Mail;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading.Service;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;

namespace HelixBackend.Controllers
{
    [Controller]
    [ApiController]
    [Area("helix-api-1")]
    [Route("[area]/[controller]")]
    public abstract class AbstractController<T, T2, T3> : CustomApiControllerBase<T, T3>
        where T : AbstractModel
        where T3 : GeneralMimeFileFormData
    {
        protected AbstractController(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IWebHostEnvironment env, IConfiguration configuration, IFileHandler fileHandler, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, MysqlDapperContext mysqlDapperContext) : base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, env, configuration, fileHandler, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, mysqlDapperContext)
        {
        }

        protected AbstractController(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IFileHandler fileHandler, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, MysqlDapperContext mysqlDapperContext) : base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, fileHandler, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, mysqlDapperContext)
        {
        }
    }

    [Controller]
    [ApiController]
    [Area("helix-api-1")]
    [Route("[area]/[controller]")]
    public abstract class AbstractController<T> : CustomApiControllerBase<T>
        where T : AbstractModel
    {
        protected AbstractController(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, MysqlDapperContext mysqlDapperContext) : base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, mysqlDapperContext)
        {
        }
    }
}
