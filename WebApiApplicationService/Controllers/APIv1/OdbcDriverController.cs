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
    public class OdbcDriverController : CustomApiControllerBase<OdbcDriverModel, Modules.OdbcDriverModule, DriverMediaUploadFormData>
    {
        public OdbcDriverController(ILogger<OdbcDriverController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IFileHandler fileHandler,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler) :
            base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration, fileHandler,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler)
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
