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
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Controllers.APIv1
{
    
    public class SoftwareVersionController : CustomApiControllerBase<SoftwareVersionModel, SoftwareVersionModule,SoftwareVersionUploadFormData>
    {
        public SoftwareVersionController(ILogger<SoftwareVersionController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IFileHandler fileHandler,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler) :
           base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration,fileHandler,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler)
        {

        }

        
        [CustomConsumesFilter(GeneralDefs.MultipartFormData)]
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [HttpPut(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFilePutRoute)]
        public override async Task<ActionResult<ApiRootNodeModel>> PutFile(string id, [FromForm] SoftwareVersionUploadFormData file)
        {
            Func<SoftwareVersionModel, List<IFormFile>, System.Threading.Tasks.Task<ActionResult<ApiRootNodeModel>>> f = (x, y) => Utils.CallAsyncFunc<SoftwareVersionModel, List<IFormFile>, ActionResult<ApiRootNodeModel>>(x, y, async (x, y) =>
            {
                List<FileLocationDescriptor> storeFiles = new List<FileLocationDescriptor>();

                foreach (IFormFile formFile in y)
                {

                    FileLocationDescriptor fileObj = new FileLocationDescriptor();
                    fileObj.FileName = x.Uuid + System.IO.Path.GetExtension(formFile.FileName.ToLower());
                    fileObj.RootDirPath = FileSystemAttachmentStorePath;
                    fileObj.Content = file.ReadIFormFile(formFile);

                    storeFiles.Add(fileObj);
                }
                ActionResult actionResult = await PutFilesOnFileSys(storeFiles);
                if (actionResult.GetType() == typeof(OkResult))
                {
                    OkResult okResult = (OkResult)actionResult;
                    if (okResult.StatusCode == 200)
                    {
                        x.FileExtension = System.IO.Path.GetExtension(file.FileNameSetup);
                        using (EncryptionHandler encryptionHandler = new EncryptionHandler())
                        {
                            x.Hash = await encryptionHandler.MD5Async(file.GetStream(file.FileSetup));

                        }
                        var response = await _backendModule.Update(x, new SoftwareVersionModel { Uuid = x.Uuid });
                        if (response.HasSuccess)
                        {
                            return await this.Get(id);
                        }
                        else
                        {
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.INTERNAL,
                    Detail = "internal server error"
                } }, HttpStatusCode.Forbidden, "an error occurred", response.Message);
                        }
                    }
                }
                return actionResult;
            });
            return await ExecBodyMethodForPutFile(id, file, new List<IFormFile> { file.FileSetup }, f);
        }

    }
}
