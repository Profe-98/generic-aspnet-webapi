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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Controllers.APIv1
{
    
    //Nur DriverController, da OdbcDriverController nicht vom Routing her von ASP.NET erkannt wird, warum auch immer ....
    public class CommunicationMediumController : CustomApiV1ControllerBase<CommunicationMediumModel, Modules.CommunicationMediumModule>
    {
        public CommunicationMediumController(ILogger<CommunicationMediumController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler) :
            base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler)
        {

        }


    }
}
