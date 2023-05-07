using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelixTicket.InternalModels;
using HelixTicket.Handler;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Controller;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Database;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Mail;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Threading.Service;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Configuration;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySql.Helix;
using WebApiFunction.Application.Controller.Modules.Helix;

namespace HelixTicket.Controllers.APIv1
{

    public class MessageController : CustomApiV1ControllerBase<MessageModel, MessageModule>
    {
        public MessageController(ILogger<MessageController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, WebApiFunction.Application.Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext) :
           base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, mysqlDapperContext)
        {
        }


        public override async Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body)
        {
            ActionResult<ApiRootNodeModel> result = await base.Create(body);
            if(result.Result != null)
            {
                if (result.Result.GetType() == typeof(CreatedResult))
                {
                    CreatedResult createdResult = (CreatedResult)result.Result;
                    switch(createdResult.StatusCode)
                    {
                        case 200:
                        case 201:
                            
                            break;
                    }
                }
            }
            return result;
        }

    }
}
