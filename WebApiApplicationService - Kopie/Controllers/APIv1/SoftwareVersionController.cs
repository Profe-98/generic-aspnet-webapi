using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
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

namespace WebApiApplicationService.Controllers.APIv1
{
    
    public class SoftwareVersionController : CustomApiControllerBase<SoftwareVersionModel, SoftwareVersionModule,SoftwareVersionUploadFormData>
    {
        public SoftwareVersionController(ILogger<SoftwareVersionController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler,ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IFileHandler fileHandler,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler,WebApiFunction.Application.Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext) :
           base(logger, vulnerablityHandler,mailHandler, authHandler,databaseHandler,jsonApiHandler,queue,jsonHandler,cache, actionDescriptorCollectionProvider, env,configuration,fileHandler,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler,mysqlDapperContext)
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
