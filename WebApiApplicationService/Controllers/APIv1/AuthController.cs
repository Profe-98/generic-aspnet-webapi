using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Models;
using WebApiApplicationService.Modules;
using WebApiApplicationService.Attribute;
using WebApiApplicationService.Handler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Controllers.APIv1
{
    
    public class AuthController : CustomApiV1ControllerBase<AuthModel, AuthModule>
    {
        public AuthController(ILogger<AuthController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler) :
           base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler)
        {

        }


        //prüft bei request ob user in role 
        //[AuthorizationFilter("root", AuthorizationFilter.CRUD.Create | AuthorizationFilter.CRUD.Read | AuthorizationFilter.CRUD.Update | AuthorizationFilter.CRUD.Delete)]
        [HttpPost("test-mit-static-role")]
        public ActionResult TestS()
        {

            return NotFound("tests");
        }

        ////prüft bei request ob user role an der route verfügbar ist
        [HttpPost("test-mit-dyn-role")]
        public ActionResult TestD()
        {

            return NotFound("testd");
        }

    }
}
