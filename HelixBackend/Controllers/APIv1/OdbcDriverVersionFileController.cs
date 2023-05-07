using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
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
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySql.Helix;
using WebApiFunction.Application.Controller.Modules.Helix;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;

namespace HelixBackend.Controllers.APIv1
{

    public class OdbcDriverVersionFileController : AbstractController<OdbcDriverVersionFileModel, OdbcDriverVersionFileModule, DriverVersionUploadFormData>
    {
        public OdbcDriverVersionFileController(ILogger<OdbcDriverVersionFileController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env,Microsoft.Extensions.Configuration.IConfiguration configuration,IFileHandler fileHandler,IRabbitMqHandler rabbitMqHandler,IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler,MysqlDapperContext mysqlDapperContext) :
            base(logger, vulnerablityHandler,mailHandler,authHandler,databaseHandler,jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env,configuration,fileHandler,rabbitMqHandler,appConfig,nodeManagerHandler,scopedEncryptionHandler, mysqlDapperContext)
        {
            
        }

        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [HttpPost]
        public override async Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body)
        {

            ActionResult<ApiRootNodeModel> result = await base.Create(body);

            return result;
        }

        [CustomConsumesFilter(GeneralDefs.MultipartFormData)]
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [HttpPut(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFilePutRoute)]
        public override async Task<ActionResult<ApiRootNodeModel>> PutFile(string id, [FromForm] DriverVersionUploadFormData file)
        {
            Func<OdbcDriverVersionFileModel, List<IFormFile>, System.Threading.Tasks.Task<ActionResult<ApiRootNodeModel>>> f = (x, y) => Utils.CallAsyncFunc<OdbcDriverVersionFileModel, List<IFormFile>, ActionResult<ApiRootNodeModel>>(x, y, async (x, y) =>
            {
                List<FileLocationDescriptor> storeFiles = new List<FileLocationDescriptor>();

                foreach (IFormFile formFile in y)
                {

                    FileLocationDescriptor fileObj = new FileLocationDescriptor();
                    fileObj.FileName = x.Uuid + "_" + formFile.FileName.ToLower();
                    fileObj.RootDirPath = FileSystemAttachmentStorePath;
                    fileObj.Content = file.ReadIFormFile(formFile);

                    storeFiles.Add(fileObj);
                }
                ActionResult actionResult = await PutFilesOnFileSys(storeFiles);
                if(actionResult.GetType() == typeof(OkResult))
                {
                    OkResult okResult = (OkResult)actionResult;
                    if(okResult.StatusCode == 200)
                    {
                        x.LibraryFile = file.FileNameLibrary;
                        x.SetupFile = file.FileNameSetup;
                        using(EncryptionHandler encryptionHandler = new EncryptionHandler())
                        {
                            x.LibFileHash = await encryptionHandler.MD5Async(file.GetStream(file.FileLibrary));
                            x.SetupFileHash = await encryptionHandler.MD5Async(file.GetStream(file.FileSetup));

                        }
                        var response = await _backendModule.Update(x,new OdbcDriverVersionFileModel { Uuid = x.Uuid });
                        if(response.HasSuccess)
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
            return await ExecBodyMethodForPutFile(id,file,new List<IFormFile> { file.FileLibrary,file.FileSetup },f);
        }


        //is attributed to generate two types of response, json for error reporting and image/jpeg for content delivery
        [CustomConsumesFilter(GeneralDefs.ZipContentType)]
        [CustomProducesFilter(GeneralDefs.ZipContentType, GeneralDefs.ApiContentType)]
        [HttpGet(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFileGetRoute)]
        public override async Task<ActionResult> GetFile(string id, string file = null)
        {
            Func<OdbcDriverVersionFileModel, string, System.Threading.Tasks.Task<ActionResult>> f = (x,y) => Utils.CallAsyncFunc<OdbcDriverVersionFileModel, string, ActionResult>(x,y, async (x,y) =>
            {
                string zipArchivPath = Path.Combine(FileSystemAttachmentStorePath);
                List<FileLocationDescriptor> fileToZip = new List<FileLocationDescriptor>();
                if (y == null)
                {
                    FileLocationDescriptor setupFile = new FileLocationDescriptor();
                    setupFile.FileName = x.Uuid + "_" + x.SetupFile;
                    setupFile.RootDirPath = zipArchivPath;
                    FileLocationDescriptor libFile = new FileLocationDescriptor();
                    libFile.FileName = x.Uuid + "_" + x.LibraryFile;
                    libFile.RootDirPath = zipArchivPath;

                    fileToZip.Add(setupFile);
                    fileToZip.Add(libFile);
                }
                else
                {
                    fileToZip.Add(new FileLocationDescriptor { FileName = x.Uuid + "_" + file, RootDirPath = zipArchivPath });
                }
                return await GetArchivedFile(x, file, zipArchivPath, fileToZip.ToArray());
            });

            return await ExecBodyMethodForGetFile(id,f, file);
        }

    }
}
