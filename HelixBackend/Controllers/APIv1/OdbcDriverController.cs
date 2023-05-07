using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.IO;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
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
using WebApiFunction.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Application.Model.Database.MySql.Helix;
using WebApiFunction.Application.Controller.Modules.Helix;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;

namespace HelixBackend.Controllers.APIv1
{

    //Nur DriverController, da OdbcDriverController nicht vom Routing her von ASP.NET erkannt wird, warum auch immer ....
    public class OdbcDriverController : AbstractController<OdbcDriverModel, OdbcDriverModule, DriverMediaUploadFormData>
    {
        public OdbcDriverController(ILogger<OdbcDriverController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IFileHandler fileHandler,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler,WebApiFunction.Application.Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext) :
            base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration, fileHandler,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler,mysqlDapperContext)
        {

        }

        //is attributed to generate two types of response, json for error reporting and image/jpeg for content delivery
        [CustomConsumesFilter(GeneralDefs.SvgXmlContentType)]
        [CustomProducesFilter(GeneralDefs.SvgXmlContentType, GeneralDefs.ApiContentType)]
        [HttpGet(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.DriverBannerRoute)]
        [HttpGet(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.DriverLogoRoute)]
        public override async Task<ActionResult> GetFile(string id,string file = null)
        {
            bool logo = HttpContext.Request.Path.Value.EndsWith(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.DriverLogoRoute.Replace(BackendAPIDefinitionsProperties.ActionParameterIdWildcard,id)) ;
            return await GetDriverMediaResources(id, logo);
        }

        private async Task<ActionResult> GetDriverMediaResources(string id,bool logo)
        {

            Func<OdbcDriverModel, string, System.Threading.Tasks.Task<ActionResult>> f = (x, y) => Utils.CallAsyncFunc<OdbcDriverModel, string, ActionResult>(x, y, async (x, y) =>
            {

                if (x.BannerExists && !logo || x.IconExists && logo)
                {
                    string fileName = x.Uuid + (x.BannerExists && !logo ? "_banner" : "_icon") + BackendAPIDefinitionsProperties.DriverMediaFilesFileExtension;
                    string resourcePath = Path.Combine(FileSystemAttachmentStorePath, fileName);
                    if (System.IO.File.Exists(resourcePath))
                    {
                        byte[] binary = System.IO.File.ReadAllBytes(resourcePath);
                        return new FileContentResult(binary, GeneralDefs.SvgXmlContentType);
                    }
                }

                return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel {
                                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND,
                                    Detail = "not found"
                                } }, HttpStatusCode.NotFound, "an error occurred", "resource not found");
            });
            return await ExecBodyMethodForGetFile(id, f,null);
        }

    }
}
