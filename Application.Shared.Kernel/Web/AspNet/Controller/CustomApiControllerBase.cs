using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Net;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using Application.Shared.Kernel.Application.Controller.Modules;
using Application.Shared.Kernel.Web.AspNet.Filter;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Security.Encryption;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Threading.Task;
using Application.Shared.Kernel.Utility;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.AspNet.Swagger.Attribut;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq.Data;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Infrastructure.Antivirus.nClam;
using Application.Shared.Kernel.Data.Web.MIME;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;

namespace Application.Shared.Kernel.Web.AspNet.Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class CustomApiControllerBase<T, T3> : CustomApiControllerBase<T>
        where T : AbstractModel
        where T3 : GeneralMimeFileFormData
    {
        private readonly IAppconfig _appconfig = null;
        private readonly IFileHandler _fileHandler = null;
        public string FileSystemAttachmentStorePath
        {
            get
            {
                if (!_appconfig.AppServiceConfiguration.AppPaths.ContainsKey(AppConfigDefinitionProperties.PathDictKeys.Controller.FileAttachmentsPath))
                    return null;

                return Path.Combine(_appconfig.AppServiceConfiguration.AppPaths[AppConfigDefinitionProperties.PathDictKeys.Controller.FileAttachmentsPath], ControllerNameFully.ToLower());
            }
        }

        public CustomApiControllerBase(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IFileHandler fileHandler, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<T> abstractBackendModule, IServiceProvider serviceProvider) :
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
            _appconfig = appConfig;
            _fileHandler = fileHandler;
            if (_appconfig != null && _fileHandler != null)
                InitStorageFolder();
        }
        public CustomApiControllerBase(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IWebHostEnvironment env, IConfiguration configuration, IFileHandler fileHandler, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<T> abstractBackendModule, IServiceProvider serviceProvider) :
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
            _appconfig = appConfig;
            _fileHandler = fileHandler;
            if (_appconfig != null && _fileHandler != null)
                InitStorageFolder();
        }

        [NonAction]
        private void InitStorageFolder()
        {
            if (!Directory.Exists(FileSystemAttachmentStorePath))
            {
                var response = _fileHandler.CreateFolder(FileSystemAttachmentStorePath);
                if (response.ErrorWhileWriting)
                {
                    throw new Exception("internal error: folder '" + FileSystemAttachmentStorePath + "' cant be created");
                }
                else
                {
                    DirectoryInfo info = new DirectoryInfo(FileSystemAttachmentStorePath);

                }
            }
        }

        [CustomConsumesFilter(GeneralDefs.MultipartFormData)]
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [HttpPut(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFilePutRoute)]
        [NonAction]
        public virtual async Task<ActionResult<ApiRootNodeModel>> PutFile(string id, [FromForm] T3 file)
        {

            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                    Detail = "forbidden"
                } }, HttpStatusCode.Forbidden, "an error occurred", "not implemented");

        }


        //is attributed to generate two types of response, json for error reporting and image/jpeg for content delivery
        [CustomConsumesFilter(GeneralDefs.BinarayContentType)]
        [CustomProducesFilter(GeneralDefs.BinarayContentType, GeneralDefs.ApiContentType)]
        [HttpGet(BackendAPIDefinitionsProperties.PhysicalFileLocationRoutes.GeneralFileGetRoute)]
        [NonAction]
        public virtual async Task<ActionResult> GetFile(string id, string file)
        {
            Func<T, string, Task<ActionResult>> f = (x, y) => Utils.CallAsyncFunc<T, string, ActionResult>(x, y, async (x, y) =>
            {
                FileLocationDescriptor fileD = new FileLocationDescriptor();
                fileD.FileName = x.Uuid + "_" + file;
                fileD.RootDirPath = FileSystemAttachmentStorePath;
                fileD.Content = System.IO.File.ReadAllBytes(fileD.FullPath);
                return new FileContentResult(fileD.Content, GeneralDefs.BinarayContentType);
            });

            return await ExecBodyMethodForGetFile(id, f, file);
        }
        [NonAction]
        public async Task<ActionResult<ApiRootNodeModel>> ExecBodyMethodForPutFile(string id, T3 formBodyObject, List<IFormFile> formFiles, Func<T, List<IFormFile>, Task<ActionResult<ApiRootNodeModel>>> methodBodyExecMethod)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (methodBodyExecMethod == null)
            {

                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD,
                    Detail = "bad request"
                } }, HttpStatusCode.Forbidden, "an error occurred", "bad request for GetFile Method");
            }
            if (!CheckGuid(id))
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a valid id" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(id) == false", methodInfo);
            }
            Guid uuid = new Guid(id);
            ActionResult<ApiRootNodeModel> result = await Get(id);
            if (result.Result != null)
            {
                Type t = result.Result.GetType();

                if (t == typeof(OkObjectResult))
                {
                    OkObjectResult okObject = (OkObjectResult)result.Result;
                    switch (okObject.StatusCode)
                    {
                        case 200:
                            ApiRootNodeModel apiRootNodeModel = (ApiRootNodeModel)okObject.Value;
                            ApiDataModel apiDataModel = (ApiDataModel)apiRootNodeModel.Data;
                            T data = (T)apiDataModel.Attributes;
                            Dictionary<string, nClam.ClamScanResult> infectedFiles = new Dictionary<string, nClam.ClamScanResult>();
                            foreach (IFormFile formFile in formFiles)
                            {
                                byte[] buffer = formBodyObject.ReadIFormFile(formFile);
                                var scanResult = await _vulnerablityHandler.Scan(buffer);
                                if (scanResult.Result != nClam.ClamScanResults.Clean)
                                    infectedFiles.Add(formFile.FileName, scanResult);
                            }
                            if (infectedFiles.Keys.Count != 0)//viruslast vorhanden
                            {
                                List<ApiErrorModel> errors = new List<ApiErrorModel>();
                                foreach (string key in infectedFiles.Keys)
                                {
                                    nClam.ClamScanResult nresult = infectedFiles[key];
                                    errors.Add(new ApiErrorModel
                                    {
                                        Title = "virus detected",
                                        Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                                        Detail = "attachment: " + key + ", Result: " + nresult.RawResult + ""
                                    });
                                }
                                return JsonApiErrorResult(errors, HttpStatusCode.NotFound, "an error occurred", "resource not found");
                            }
                            return await methodBodyExecMethod.Invoke(data, formFiles);
                            break;
                        default:
                            break;
                    }
                    return okObject;
                }
            }

            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                    Detail = "forbidden"
                } }, HttpStatusCode.Forbidden, "an error occurred", "no permissions or data not found");
        }

        [NonAction]
        public async Task<ActionResult> ExecBodyMethodForGetFile(string id, Func<T, string, Task<ActionResult>> methodBodyExecMethod, string file = null)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (methodBodyExecMethod == null)
            {

                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD,
                    Detail = "bad request"
                } }, HttpStatusCode.Forbidden, "an error occurred", "bad request for GetFile Method");
            }
            if (!CheckGuid(id))
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a valid id" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(id) == false", methodInfo);
            }
            Guid uuid = new Guid(id);
            ActionResult<ApiRootNodeModel> result = await Get(id);
            if (result.Result != null)
            {
                Type t = result.Result.GetType();

                if (t == typeof(OkObjectResult))
                {
                    OkObjectResult okObject = (OkObjectResult)result.Result;
                    switch (okObject.StatusCode)
                    {
                        case 200:
                            ApiRootNodeModel apiRootNodeModel = (ApiRootNodeModel)okObject.Value;
                            ApiDataModel apiDataModel = (ApiDataModel)apiRootNodeModel.Data;
                            T data = (T)apiDataModel.Attributes;

                            return await methodBodyExecMethod.Invoke(data, file);
                            break;
                        default:
                            break;
                    }
                    return okObject;
                }
            }

            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel {
                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                    Detail = "forbidden"
                } }, HttpStatusCode.Forbidden, "an error occurred", "no permissions or data not found");
        }

        [NonAction]
        public async Task<ActionResult> PutFilesOnFileSys(List<FileLocationDescriptor> files)
        {
            List<ApiErrorModel> errors = new List<ApiErrorModel>();
            foreach (FileLocationDescriptor fileLocationDescriptor in files)
            {
                try
                {
                    System.IO.File.WriteAllBytes(fileLocationDescriptor.FullPath, fileLocationDescriptor.Content);
                }
                catch (Exception ex)
                {
                    errors.Add(new ApiErrorModel
                    {
                        Title = "save file error",
                        Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                        Detail = "" + ex.Message + ""
                    });
                }
            }
            if (errors.Count != 0)
            {
                return JsonApiErrorResult(errors, HttpStatusCode.NotFound, "an error occurred", "resource not found");
            }

            return Ok();
        }

        [NonAction]
        public async Task<ActionResult> GetArchivedFile(T data, string file, string zipArchivPath, FileLocationDescriptor[] zipContainingContentFiles)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            string zipArchiv = data.Uuid + (file == null ? "_all" : "_" + file) + ".zip";
            zipArchivPath = Path.Combine(zipArchivPath, zipArchiv);
            List<string> fileToZip = new List<string>();
            if (file == null)
            {
                foreach (FileLocationDescriptor fileLocationDescriptor in zipContainingContentFiles)
                {
                    if (fileLocationDescriptor.IsFullPathExistend)
                    {
                        fileToZip.Add(fileLocationDescriptor.FullPath);
                    }
                }
            }
            if (fileToZip.Count != 0)
            {
                if (!System.IO.File.Exists(zipArchivPath))
                {
                    using (ZipArchive archiv = ZipFile.Open(zipArchivPath, ZipArchiveMode.Create))
                    {
                        for (int i = 0; i < fileToZip.Count; i++)
                        {
                            string path = fileToZip[i];
                            string entryName = Path.GetFileName(path);
                            archiv.CreateEntryFromFile(path, entryName);
                        }
                    }
                }
                if (System.IO.File.Exists(zipArchivPath))//nochmal abfragen, file kann nämlich noch trotz zip erstellung nicht vorhanden sein wenn keine berechtigungen zum erstellen auf dem filesystem sind
                {
                    byte[] binaryZipArchiv = System.IO.File.ReadAllBytes(zipArchivPath);
                    var response = new FileContentResult(binaryZipArchiv, GeneralDefs.ZipContentType);
                    response.FileDownloadName = zipArchiv;
                    return response;
                }
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel {
                                    Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND,
                                    Detail = "not found"
                                } }, HttpStatusCode.NotFound, "an error occurred", "resource not found");

        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class CustomApiControllerBase<T> : CustomControllerBase
        where T : AbstractModel
    {
        #region Private

        public const string RabbitMqEntityCreateRoutingKey = "create";
        public const string RabbitMqEntityDeleteRoutingKey = "delete";
        public const string RabbitMqEntityUpdateRoutingKey = "update";
        public const string RabbitMqGeneralRoutingKey = "general";

        private Guid _controllerId = Guid.Empty;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly ITaskSchedulerBackgroundServiceQueuer _queue;
        protected readonly IScopedDatabaseHandler _databaseHandler = null;
        protected readonly IScopedJsonHandler _jsonHandler = null;
        protected readonly ICachingHandler _cacheHandler = null;
        protected T _genericAbstractModelType = default;
        //protected T2 _backendModule = default;
        protected readonly ILogger<CustomApiControllerBase<T>> _logger;
        protected readonly IJsonApiDataHandler _jsonApiHandler;
        protected readonly IAbstractBackendModule<T> _backendModule;
        protected readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        protected readonly IAuthHandler _authHandler;
        protected readonly IMailHandler _mailHandler;
        protected readonly IScopedVulnerablityHandler _vulnerablityHandler;
        protected readonly IRabbitMqHandler _rabbitMqHandler;
        protected readonly IAppconfig _appConfig;
        protected readonly INodeManagerHandler _nodeManagerHandler;
        protected readonly IScopedEncryptionHandler _scopedEncryptionHandler;
        private readonly Func<MessageModel, MessageModelResponse> _generalFunc = new Func<MessageModel, MessageModelResponse>((x) =>
        {
            return new MessageNOK();
        });
        private readonly Func<MessageModel, MessageModelResponse> _entityCreateFunc = new Func<MessageModel, MessageModelResponse>((x) =>
        {
            return new MessageNOK();
        });
        private readonly Func<MessageModel, MessageModelResponse> _entityUpdateFunc = new Func<MessageModel, MessageModelResponse>((x) =>
        {
            return new MessageNOK();
        });
        private readonly Func<MessageModel, MessageModelResponse> _entityDeleteFunc = new Func<MessageModel, MessageModelResponse>((x) =>
        {
            return new MessageNOK();
        });
        #endregion
        #region Public

        /// <summary>
        /// Controller Identifier for ResponseCaching
        /// </summary>
        
        internal Guid ControllerResponseCacheIdentifier
        {
            get
            {

                if (_controllerId == Guid.Empty)
                {
                    _controllerId = Utils.GenerateRequestGuid(typeof(T).Name.ToLower() + "-api-responses", Guid.Empty);
                }
                return _controllerId;
            }
        }
        internal ILogger<CustomApiControllerBase<T>> Logger
        {
            get
            {
                return _logger;
            }
        }
        internal IJsonApiDataHandler JsonApiHandler
        {
            get
            {
                return _jsonApiHandler;
            }
        }
        internal string ControllerName
        {
            get
            {
                return GetType().Name.Replace("Controller", "");
            }
        }
        internal string ControllerNameFully
        {
            get
            {
                return GetType().Name;
            }
        }
        #endregion
        #region Ctor
        public CustomApiControllerBase(ILogger<CustomApiControllerBase<T>> logger,
            IScopedVulnerablityHandler vulnerablityHandler,
            IMailHandler mailHandler,
            IAuthHandler authHandler,
            IScopedDatabaseHandler databaseHandler,
            IJsonApiDataHandler jsonApiHandler,
            ITaskSchedulerBackgroundServiceQueuer queue,
            IScopedJsonHandler jsonHandler,
            ICachingHandler cache,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            IWebHostEnvironment env,
            IConfiguration configuration,
            IRabbitMqHandler rabbitMqHandler,
            IAppconfig appConfig,
            INodeManagerHandler nodeManagerHandler,
            IScopedEncryptionHandler scopedEncryptionHandler,
            IAbstractBackendModule<T> abstractBackendModule,
            IServiceProvider serviceProvider) :
            base(env, cache, configuration, rabbitMqHandler)
        {
            _serviceProvider = serviceProvider;
            _mailHandler = mailHandler;
            _vulnerablityHandler = vulnerablityHandler;
            _authHandler = authHandler;
            _webHostEnvironment = env;
            _jsonHandler = jsonHandler;
            _queue = queue;
            _logger = logger;
            _databaseHandler = databaseHandler;
            _backendModule = abstractBackendModule;
            _genericAbstractModelType = Activator.CreateInstance<T>();
            _jsonApiHandler = jsonApiHandler;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _cacheHandler = cache;
            _appConfig = appConfig;
            _rabbitMqHandler = rabbitMqHandler;
            _nodeManagerHandler = nodeManagerHandler;
            _scopedEncryptionHandler = scopedEncryptionHandler;

            InitProcedure();
        }
        public CustomApiControllerBase(ILogger<CustomApiControllerBase<T>> logger,
            IScopedVulnerablityHandler vulnerablityHandler,
            IMailHandler mailHandler,
            IAuthHandler authHandler,
            IScopedDatabaseHandler databaseHandler,
            IJsonApiDataHandler jsonApiHandler,
            ITaskSchedulerBackgroundServiceQueuer queue,
            IScopedJsonHandler jsonHandler,
            ICachingHandler cache,
            IWebHostEnvironment env,
            IConfiguration configuration,
            IRabbitMqHandler rabbitMqHandler,
            IAppconfig appConfig,
            INodeManagerHandler nodeManagerHandler,
            IScopedEncryptionHandler scopedEncryptionHandler,
            IAbstractBackendModule<T> abstractBackendModule,
            IServiceProvider serviceProvider) :
            this(logger,
            vulnerablityHandler,
            mailHandler,
            authHandler,
            databaseHandler,
            jsonApiHandler,
            queue,
            jsonHandler,
            cache,
            null,
            env,
            configuration,
            rabbitMqHandler,
            appConfig,
            nodeManagerHandler,
            scopedEncryptionHandler,
            abstractBackendModule,
            serviceProvider)
        {

        }
        public CustomApiControllerBase()
        {
            
        }

        [NonAction]
        public abstract AbstractBackendModule<T> GetConcreteModule();

        [NonAction]
        private string GetExchangeDeclarationName()
        {
            return typeof(T).Name.ToLower();
        }
        [NonAction]
        private void InitProcedure()
        {
            if (_rabbitMqHandler != null && _rabbitMqHandler.Connection != null)
            {

                string exchangeName = GetExchangeDeclarationName();
                _rabbitMqHandler.SubscibeExchange(exchangeName, _generalFunc, null, RabbitMqGeneralRoutingKey);
                _rabbitMqHandler.SubscibeExchange(exchangeName, _entityCreateFunc, null, RabbitMqEntityCreateRoutingKey);
                _rabbitMqHandler.SubscibeExchange(exchangeName, _entityUpdateFunc, null, RabbitMqEntityUpdateRoutingKey);
                _rabbitMqHandler.SubscibeExchange(exchangeName, _entityDeleteFunc, null, RabbitMqEntityDeleteRoutingKey);
            }
        }

        #endregion

        #region HttpMethods
        /// <summary>
        /// Gets all objects with type of generic type T of the controller instance 
        /// </summary>
        /// <returns>
        ///         - 200: Ok, Resource found
        ///         - 404: Not Found, when the resource doesnt exist, e.g. the endpoint (controller) or id
        /// </returns>
        /// 
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpGet]
        [NonAction]
        public virtual async Task<ObjectResult> Get()
        {
            return await Get(Guid.Empty.ToString());
        }

        [NonAction]
        public async Task<JsonApiTreeSearchFilterModel> CreateSearchFilterModelChain(string[] x, int y, JsonApiTreeSearchFilterModel z)
        {

            var values = MySqlDefinitionProperties.BackendTablesEx.Values;
            if (y < x.Length)
            {

                string dbName = _genericAbstractModelType.fGenerateTableNameFromNetClassName(x[y].ToLower());
                var foundObj = values.ToList().Find(x => x.NetType.Name.ToLower() == dbName);
                if (foundObj != null)
                {
                    var m = foundObj.ClassModel;
                    var t = new JsonApiTreeSearchFilterModel { EntityName = x[y] };
                    if (z.FilterRelationNames == null)
                        z.FilterRelationNames = new List<JsonApiTreeSearchFilterModel>();
                    z.FilterRelationNames.Add(t);
                    if (x.Length > 0)
                    {

                        string[] a = new string[x.Length];
                        for (int i = y; i < x.Length; i++)
                        {
                            a[i] = x[i];
                        }
                        return await CreateSearchFilterModelChain(a, y + 1, t);
                    }
                    return z;
                }
            }
            return z;
        }
        /// <summary>
        /// Get a specific object with type of generic type T of the controller instance identified by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///         - 200: Ok, Resource found
        ///         - 404: Not Found, when the resource doesnt exist, e.g. the endpoint (controller) or id
        /// </returns>
        /// 

        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpGet(BackendAPIDefinitionsProperties.ActionParameterIdWildcard)]
        [NonAction]
        public virtual async Task<ObjectResult> Get(string id, int maxDepth = 0)
        {
            MethodDescriptor methodInfo = _webHostEnvironment == null || _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            Logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), HttpContext, ControllerName);

            T instance = Activator.CreateInstance<T>();

            instance.Uuid = CheckGuid(id) ?
                new Guid(id) : Guid.Empty;


            Guid CacheGuid = instance.Uuid;


            ApiRootNodeModel apiRootNodeModel = null;// await _cacheHandler.GetCacheEntry(CacheGuid.ToString());

            string requestQueryParameterCacheKey = null;// _cacheHandler.GetObjectKeyIdentifier(CacheGuid,typeof(T).Name.ToLower());

            Queue<SortingModel> sortingValues = null;
            int pageOffset = GeneralDefs.NotFoundResponseValue;
            int pageOffsetEnd = GeneralDefs.NotFoundResponseValue;
            IQueryCollection queryParamerters = HttpContext.Request.Query;

            bool pageParemeterInHttpRequQuery = false;
            bool maxItemsPerPageParemeterInHttpRequQuery = false;
            int maxItemsPerPageValue = GeneralDefs.NotFoundResponseValue;

            var searchFilter = new JsonApiTreeSearchFilterModel() { EntityName = _genericAbstractModelType.DisplayName };
            if (queryParamerters.Keys.Count != 0)
            {
                if (queryParamerters.ContainsKey("include"))//load related records of current entity, by given comma separated relations
                {
                    searchFilter.FilterRelationNames = new List<JsonApiTreeSearchFilterModel>();
                    string values = queryParamerters["include"];
                    if (!string.IsNullOrEmpty(values))
                    {
                        string[] valueCollection = values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (valueCollection.Length == 0)
                        {

                            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = CacheGuid, Detail = "try to sort without sort-values" }
                            }, HttpStatusCode.BadRequest, "an error occurred", "valueCollection.Length == 0", methodInfo);
                        }

                        valueCollection = valueCollection.Distinct().ToArray();
                        int calcMaxDepth = 1;
                        if (_genericAbstractModelType.DatabaseRelations != null)
                        {
                            foreach (var val in valueCollection)
                            {
                                string currentRelationLevel = null;
                                JsonApiTreeSearchFilterModel relation = new JsonApiTreeSearchFilterModel();
                                relation.FilterRelationNames = new List<JsonApiTreeSearchFilterModel>();
                                if (val.Contains("."))
                                {
                                    string[] split = val.Split(".");
                                    relation.FilterRelationNames = new List<JsonApiTreeSearchFilterModel>(split.Length);
                                    if (split.Length > calcMaxDepth)
                                    {
                                        calcMaxDepth = split.Length;
                                    }
                                    currentRelationLevel = split[0];
                                    string[] tmpArr = new string[split.Length];
                                    for (int i = 0; i < split.Length; i++)
                                    {
                                        tmpArr[i] = split[i];
                                    }

                                    relation = await CreateSearchFilterModelChain(tmpArr, 0, relation);
                                }
                                else
                                {
                                    currentRelationLevel = val;
                                }
                                relation.EntityName = currentRelationLevel;
                                if (relation.EntityName != null)
                                    searchFilter.FilterRelationNames.Add(relation);
                            }
                        }
                        maxDepth = calcMaxDepth;
                    }
                }
                if (queryParamerters.ContainsKey("sort"))//sort item collection
                {
                    string values = queryParamerters["sort"];
                    if (!string.IsNullOrEmpty(values))
                    {
                        string[] valueCollection = values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (valueCollection.Length == 0)
                        {

                            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = CacheGuid, Detail = "try to sort without sort-values" }
                            }, HttpStatusCode.BadRequest, "an error occurred", "valueCollection.Length == 0", methodInfo);
                        }

                        sortingValues = new Queue<SortingModel>();
                        foreach (string item in valueCollection)
                        {
                            string tmpItem = item;
                            if (!string.IsNullOrEmpty(tmpItem))
                            {
                                SortingModel sortingModel = new SortingModel();
                                sortingModel.Sort = tmpItem.StartsWith('-') ?
                                         SortingModel.SORTING_DIRECTION.DESC : SortingModel.SORTING_DIRECTION.ASC;
                                if (sortingModel.Sort == SortingModel.SORTING_DIRECTION.DESC)
                                {
                                    tmpItem = tmpItem.Remove(0, 1);//remove minus for sortingdirection
                                }
                                PropertyInfo prop = instance.GetMemberByJsonPropertyName(tmpItem);
                                if (prop != null)
                                {
                                    requestQueryParameterCacheKey += "-sort-" + (sortingModel.Sort == SortingModel.SORTING_DIRECTION.DESC ? "desc" : "asc");
                                    sortingModel.NodeName = tmpItem.ToLower();

                                    sortingModel.TargetMember = prop;
                                    sortingValues.Enqueue(sortingModel);
                                }
                                else
                                {
                                    return JsonApiErrorResult(new List<ApiErrorModel> {
                                        new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = CacheGuid, Detail = "sort value is not avaible in data" }
                                    }, HttpStatusCode.NotFound, "an error occurred", "prop == null", methodInfo);
                                }
                            }
                            else
                            {
                                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = CacheGuid, Detail = "sort value is empty" }
                                }, HttpStatusCode.BadRequest, "an error occurred", "String.IsNullOrEmpty(tmpItem)", methodInfo);
                            }
                        }
                    }
                    else
                    {
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                            new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = CacheGuid, Detail = "try to sort without sort-values" }
                        }, HttpStatusCode.BadRequest, "an error occurred", "String.IsNullOrEmpty(values)", methodInfo);
                    }
                }
                pageParemeterInHttpRequQuery = queryParamerters.ContainsKey("page");
                if (pageParemeterInHttpRequQuery)
                {
                    string values = queryParamerters["page"];
                    if (!string.IsNullOrEmpty(values))
                    {
                        string[] valueCollection = values.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (valueCollection.Length > 2)
                        {
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "more than 2 values for pagination" }
                            }, HttpStatusCode.BadRequest, "an error occurred", "valueCollection.Length > 1", methodInfo);
                        }
                        if (!int.TryParse(valueCollection[0], out pageOffset))
                        {
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "pageoffset[0] must be an integer" }
                            }, HttpStatusCode.BadRequest, "an error occurred", "int.TryParse(valueCollection[0], out int pageOffset) == false", methodInfo);
                        }

                        string pageOffSetStr = "" + pageOffset;
                        if (valueCollection.Length == 2)
                        {

                            if (!int.TryParse(valueCollection[1], out pageOffsetEnd))
                            {
                                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "pageoffset[1] must be an integer" }
                                }, HttpStatusCode.BadRequest, "an error occurred", "int.TryParse(valueCollection[1], out int pageOffset) == false", methodInfo);
                            }
                            pageOffSetStr += "-" + pageOffsetEnd;
                        }

                        requestQueryParameterCacheKey += "-pagination" + "-" + pageOffSetStr + "";

                    }
                }

                maxItemsPerPageParemeterInHttpRequQuery = queryParamerters.ContainsKey("max-items-per-page");
                if (maxItemsPerPageParemeterInHttpRequQuery)
                {
                    string values = queryParamerters["max-items-per-page"];
                    if (!string.IsNullOrEmpty(values))
                    {
                        if (!int.TryParse(values, out maxItemsPerPageValue))
                        {
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "max-items-per-page must be an integer" }
                            }, HttpStatusCode.BadRequest, "an error occurred", "int.TryParse(values, out maxItemsPerPageValue) == false", methodInfo);

                        }
                        else
                        {
                            if (maxItemsPerPageValue <= 0)
                            {
                                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "max-items-per-page must be greater than 0" }
                                }, HttpStatusCode.BadRequest, "an error occurred", "maxItemsPerPageValue <= 0", methodInfo);

                            }
                        }
                    }
                }
            }

            List<T> t = new List<T>();
            bool dataExists = false;
            bool foundInCache = false;

            AreaAttribute areaAttribute = GetArea();


            var resp = await _jsonApiHandler.GetorSetCacheData(instance);
            dataExists = !(resp == null || resp.Count == 0);
            if (dataExists)
            {
                foreach (var item in resp)
                {
                    t.Add((T)item);
                }
            }
            if (t.Count != 0)
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                apiRootNodeModel = await _jsonApiHandler.CreateApiRootNodeFromModel(areaAttribute.RouteValue, t, maxDepth, searchFilter.FilterRelationNames);

                stopwatch.Stop();

            }


            if (dataExists)
            {
                //1.erst filter aus http-get-query bearbeiten
                //2.(1.) dann gefilterte ergebnisse sortieren, wenn kein filter angewendet dann ergebnisse sortieren
                //3.(2.)als letztes ergebnisse von sort falls pagination gewünscht paginaten
                if (!foundInCache)
                {

                    if (sortingValues != null)
                    {

                        if (sortingValues.Count != 0 && apiRootNodeModel.RootNodes.Count > 1)
                        {

                            List<ApiDataModel> responseSort = Sort(apiRootNodeModel.RootNodes, ref sortingValues);
                            apiRootNodeModel.Data = responseSort;
                        }
                    }
                    bool pageOffSetRequest = pageOffset != GeneralDefs.NotFoundResponseValue && pageOffsetEnd == GeneralDefs.NotFoundResponseValue;
                    bool pageRangeRequested = pageOffset != GeneralDefs.NotFoundResponseValue && pageOffsetEnd != GeneralDefs.NotFoundResponseValue;

                    if (apiRootNodeModel.PaginatedDataList != null)
                    {
                        if (maxItemsPerPageParemeterInHttpRequQuery && maxItemsPerPageValue != GeneralDefs.NotFoundResponseValue)
                            apiRootNodeModel.MaxItemsPerPage = maxItemsPerPageValue;

                        var keysList = apiRootNodeModel.PaginatedDataList.Keys;
                        int minKeyVal = keysList.Min();
                        int maxKeyVal = keysList.Max();
                        if (pageOffSetRequest || pageRangeRequested)
                        {
                            if (pageRangeRequested && pageOffset == pageOffsetEnd)
                            {
                                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "pageoffset[0] and pageoffset[1] cant be equal" }
                                }, HttpStatusCode.BadRequest, "an error occurred", "pageRangeRequested && pageOffset == pageOffsetEnd == true", methodInfo);
                            }
                            if (pageOffset < minKeyVal || pageOffsetEnd > maxKeyVal && pageRangeRequested)
                            {
                                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "pageoffset must be in range from "+minKeyVal+" to "+maxKeyVal+"" }
                                }, HttpStatusCode.BadRequest, "an error occurred", "pageOffset < minKeyVal || (pageOffsetEnd > maxKeyVal && pageRangeRequested) == true", methodInfo);
                            }
                            else if (pageOffset > pageOffsetEnd && pageRangeRequested)
                            {
                                int tmpOffset = pageOffset;
                                pageOffset = pageOffsetEnd;
                                pageOffsetEnd = tmpOffset;
                            }

                            Dictionary<int, List<ApiDataModel>> tmpPaginated = apiRootNodeModel.PaginatedDataList;
                            if (tmpPaginated != null)
                            {
                                if (tmpPaginated.ContainsKey(pageOffset) && pageOffSetRequest)
                                {

                                    apiRootNodeModel.Data = tmpPaginated[pageOffset];
                                }
                                else if (tmpPaginated.ContainsKey(pageOffset) && tmpPaginated.ContainsKey(pageOffsetEnd) && pageRangeRequested)
                                {
                                    List<ApiDataModel> pages = new List<ApiDataModel>();
                                    for (int i = pageOffset; i < pageOffsetEnd; i++)
                                    {
                                        tmpPaginated[i].ForEach(x => pages.Add(x));
                                    }
                                    apiRootNodeModel.Data = pages;
                                }
                                else
                                {
                                    return JsonApiErrorResult(new List<ApiErrorModel> {
                                        new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = instance.Uuid, Detail = "target page not existing in data, avaible-page-range from "+minKeyVal+" to "+maxKeyVal+"" }
                                    }, HttpStatusCode.NotFound, "an error occurred", "tmpPaginated.ContainsKey(pageOffset) && tmpPaginated.ContainsKey(pageOffsetEnd) && pageRangeRequested == false", methodInfo);
                                }
                            }
                        }
                        else if (pageParemeterInHttpRequQuery)
                        {
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = instance.Uuid, Detail = "pageoffset must be in range from "+minKeyVal+" to "+maxKeyVal+"" }
                            }, HttpStatusCode.BadRequest, "an error occurred", "offset not in range, see details", methodInfo);
                        }
                    }
                }

                OkObjectResult tmp = Ok(apiRootNodeModel);

                _rabbitMqHandler.PublishObject(GetExchangeDeclarationName(), apiRootNodeModel, RabbitMqGeneralRoutingKey, "general-test", null, null);
                apiRootNodeModel.Dispose();
                return tmp;
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = instance.Uuid, Detail = "resource not found" }
            }, HttpStatusCode.NotFound, "an error occurred", "dataExists == false", methodInfo);
        }

        [NonAction]
        internal List<ApiDataModel> Sort(List<ApiDataModel> source, ref Queue<SortingModel> sortingProperty)
        {
            if (sortingProperty.Count != 0)
            {
                List<T> attributesOfSource = new List<T>();
                foreach (ApiDataModel model in source)
                {
                    attributesOfSource.Add((T)model.Attributes);
                }
                SortingModel currentIterationLevel = sortingProperty.Dequeue();
                PropertyInfo sortProperty = currentIterationLevel.TargetMember;
                if (sortProperty != null)
                {
                    switch (currentIterationLevel.Sort)
                    {
                        case SortingModel.SORTING_DIRECTION.ASC:
                            attributesOfSource = attributesOfSource.OrderBy(p => sortProperty.GetValue(p)).ToList();
                            break;
                        case SortingModel.SORTING_DIRECTION.DESC:
                            attributesOfSource = attributesOfSource.OrderByDescending(p => sortProperty.GetValue(p)).ToList();
                            break;
                    }
                }
                List<ApiDataModel> response = new List<ApiDataModel>();
                foreach (T item in attributesOfSource)
                {
                    response.Add(source.Find(x => item.Uuid == x.Id));
                }
                source = response;
                if (sortingProperty.Count != 0)
                {
                    source = Sort(source, ref sortingProperty);
                }
            }
            return source;
        }

        /// <summary>
        /// Respond all relations or a specific relation of current controller
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relationname"></param>
        /// <param name="relatedid"></param>
        /// <returns>
        ///         - 200: Ok, Resource found
        ///         - 404: Not Found, when the resource doesnt exist, e.g. the endpoint (controller) or id
        /// </returns>
        /// Example Requests:
        /// HTTP-GET for 1to1 Relation (without nested subclass, direct relation): http://localhost:5001/apiv1/softwareversion/c54d7b1c-a5bb-11eb-bac0-309c2364fdb6/relation/versiontype/78ee5d00-a5b5-11eb-bac0-309c2364fdb6
        /// 
        /// HTTP-GET for 1ton Relation (with nested subclass, relation over subclass, e.g. connectiontype is related with driverversion over driverversionrelationconnectiontypemodel): http://localhost:5001/apiv1/driverversion/1d5f864e-a5b7-11eb-bac0-309c2364fdb6/relation/connectiontype/989b5936-a5b4-11eb-bac0-309c2364fdb6
        /// or for all connectiontypes (without relatedid): http://localhost:5001/apiv1/driverversion/1d5f864e-a5b7-11eb-bac0-309c2364fdb6/relation/connectiontype/

        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpGet(BackendAPIDefinitionsProperties.ActionParameterIdWildcard + "/relation/{relationname}/" + BackendAPIDefinitionsProperties.ActionParameterOptionalIdSecondaryWildcard)]
        [NonAction]
        public virtual async Task<ObjectResult> GetRelation(string id, string relationname, string relationid = null)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (!CheckGuid(id) || !CheckGuid(relationid) && relationid != null)
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a valid id" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(id) == false", methodInfo);
            }

            Guid uuid = new Guid(id);
            Logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), HttpContext, ControllerName);
            if (!string.IsNullOrEmpty(relationname))
            {
                string genericTypeName = _genericAbstractModelType.GetType().Name.ToLower();
                relationname = relationname.ToLower();


                var relations = _genericAbstractModelType.DatabaseRelations;
                if (relations != null)
                {
                    AbstractModel relation = null;

                    relations.ForEach(r =>
                    {
                        AbstractModel abstractModelTmp = (AbstractModel)_jsonApiHandler.GetForeignObject(_genericAbstractModelType, r);
                        if (abstractModelTmp != null && relation == null)
                        {
                            if (abstractModelTmp.DisplayName.ToLower() == relationname.ToLower())
                            {
                                relation = abstractModelTmp;
                            }
                        }

                    });
                    if (relation == null)
                    {

                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.NotFound, "an error occurred", "parent == null", methodInfo);
                    }
                    string dictKey = relation.fGenerateNetClassNameFromTableName(relation.DatabaseTable).ToLower();
                    ObjectResult parent = await Get(id);
                    if (parent != null)
                    {
                        if (parent.Value != null)
                        {
                            Type t = parent.Value.GetType();

                            switch (parent.StatusCode)
                            {
                                case 200:
                                    if (parent.Value != null)
                                    {
                                        ApiRootNodeModel parentObject = (ApiRootNodeModel)parent.Value;
                                        ApiDataModel dataModel = (ApiDataModel)parentObject.Data;
                                        List<ApiDataModel> tmp = await dataModel.SetRelationMaxDepth(1, dataModel, new List<ApiDataModel>() { dataModel });

                                        if (dataModel != null)
                                        {
                                            ApiRelationshipModel responseRelations = new ApiRelationshipModel();
                                            responseRelations.Links = new ApiLinkModel()
                                            {
                                                Related = "/" + ControllerName + "/" + id + "/relation/" + relationname + "/" + (relationid == null ?
                                                "" : relationid),
                                                Self = "/" + ControllerName + "/" + id + "/"
                                            };
                                            if (dataModel.Relationships != null && dataModel.Relationships.ContainsKey(dictKey))
                                            {
                                                var data = relationid != null ?
                                                    dataModel.Relationships[dictKey].FindAll(x => ((ApiDataModel)x.Data).Id == new Guid(relationid)) : dataModel.Relationships[dictKey].ToList();
                                                if (data != null)
                                                {
                                                    List<ApiDataModel> relModels = new List<ApiDataModel>();
                                                    data.ForEach(x =>
                                                    {
                                                        var tmp = dataModel.Included.Find(x => x.Id == x.Id);
                                                        if (tmp != null)
                                                            tmp.AttributeVisibility = true;
                                                        relModels.Add(tmp);
                                                    });
                                                    if (relModels.Count > 0)
                                                    {

                                                        responseRelations.Data = data;

                                                        ApiRootNodeModel apiRootNodeModel = new ApiRootNodeModel();
                                                        apiRootNodeModel.Data = responseRelations;
                                                        apiRootNodeModel.Meta = new ApiMetaModel { Count = data != null ? data.Count : 0 };

                                                        return Ok(apiRootNodeModel);
                                                    }
                                                }
                                                else
                                                {

                                                }

                                            }
                                        }
                                    }
                                    break;
                            }

                        }
                        return parent;
                    }
                    return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.NotFound, "an error occurred", "parent == null", methodInfo);
                }

            }

            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "String.IsNullOrEmpty(relationname)", methodInfo);
        }

        /// <summary>
        /// Update an existing resource or n-resources
        /// </summary>
        /// <param name="id">Optional Parameter, when setted the common requ.-body is targeting on the given id ref.</param>
        /// <param name="body">http requ.-body</param>
        /// <returns>
        /// Status: 
        ///         - 200: Ok, Resource updated
        ///         - 202: Accepted, Resource is ordered in queuing to be processed
        ///         - 204: No Content, Created completed but no content is to respond
        ///         - 201: Created
        ///         - 404: Not Found, when the resource doesnt exist, e.g. the endpoint (controller)
        ///         - 403: Forbidden, Unsupported Request
        ///         - 409: Conflict, when the object that should be created already exist identified by id of the given resource or a resource that would violate server constraints e.g. a relation that doest not exist but is request to update
        /// </returns>
        /// 

        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpPatch(BackendAPIDefinitionsProperties.ActionParameterOptionalIdWildcard)]
        [NonAction]
        public virtual async Task<ObjectResult> Update(string? id, [FromBody] ApiRootNodeModel body)//model == null wenn formattierung fail, id == 0 wenn int nicht formattiert werden kann wie bsp bei angabe von einem string
        {

            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            Logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), HttpContext, ControllerName);
            ApiRootNodeModel bodyObject = body;

            if (bodyObject != null)
            {
                List<ApiDataModel> bodyValues = new List<ApiDataModel>();
                if (Utils.IsList<ApiDataModel>(bodyObject.Data))
                {
                    bodyValues = (List<ApiDataModel>)bodyObject.Data;
                }
                else
                {
                    if (bodyObject.Data != null)
                    {
                        bodyValues.Add((ApiDataModel)bodyObject.Data);
                    }
                }
                if (bodyValues.Count > 1 || bodyValues.Count == 0)
                {
                    return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "conflic or badrequest" }
            }, HttpStatusCode.BadRequest, "an error occurred", "bodyValues.Count > 1 || bodyValues.Count == 0", methodInfo);
                }
                List<ApiErrorModel> errors = new List<ApiErrorModel>();

                List<T> itemList = new List<T>();
                foreach (ApiDataModel item in bodyValues)
                {
                    T dataT = (T)item.Attributes;
                    itemList.Add(dataT);
                    bool validation = TryValidateModel((T)item.Attributes, nameof(T));
                    if (!validation)
                    {
                        List<string> modelErrors = new List<string>();
                        foreach (var member in ModelState.Keys)
                        {
                            var state = ModelState[member];
                            foreach (var error in state.Errors)
                            {
                                string jsonNodeName = dataT.GetJsonDisplayName(member.Replace("T.", "")) ?? member;
                                modelErrors.Add(jsonNodeName + " " + error.ErrorMessage);
                            }
                        }
                        foreach (string err in modelErrors)
                        {
                            ApiErrorModel errMod = new ApiErrorModel();
                            errMod.Title = "error";
                            errMod.Detail = err;
                            errMod.Id = item.Id;
                            errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                            errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                            errMod.Links = item.Links;
                            errors.Add(errMod);
                        }
                    }
                    else if ((dataT.Uuid == Guid.Empty || item.Id == Guid.Empty) && id == null)
                    {
                        ApiErrorModel errMod = new ApiErrorModel();
                        errMod.Title = "error";
                        errMod.Detail = "id and attribute uuid could not be null or " + Guid.Empty.ToString() + " when the http requ. query parameter id is null";
                        errMod.Id = item.Id;
                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                        errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                        errMod.Links = item.Links;
                        errors.Add(errMod);
                    }
                    else if (dataT.Uuid != item.Id && id == null)
                    {
                        ApiErrorModel errMod = new ApiErrorModel();
                        errMod.Title = "error";
                        errMod.Detail = "id and attribute uuid must be equal";
                        errMod.Id = item.Id;
                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                        errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                        errMod.Links = item.Links;
                        errors.Add(errMod);
                    }
                    else if (dataT.Uuid != item.Id && id != null)
                    {
                        ApiErrorModel errMod = new ApiErrorModel();
                        errMod.Title = "error";
                        errMod.Detail = "id and attribute uuid must be equal when http requ. query parameter id is given";
                        errMod.Id = item.Id;
                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                        errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                        errMod.Links = item.Links;
                        errors.Add(errMod);
                    }
                }
                ApiRootNodeModel response = await _jsonApiHandler.CreateApiRootNodeFromModel(GetArea().RouteKey, itemList);
                response.Errors = errors;
                if (response.HasErrors)
                {

                    return JsonApiErrorResult(response, HttpStatusCode.UnprocessableEntity, "an error occurred", "response.HasErrors == true", methodInfo);
                }
                if (bodyValues.Count > 1)//bei mehrfachanlage von nodes aus body
                {
                    if (id != null)//wenn n-resources gegeben und id angeben, macht keinen sinn 
                    {
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "conflic or badrequest" }
            }, HttpStatusCode.BadRequest, "an error occurred", "id != null && bodyValues.Count > 1", methodInfo);
                    }
                    foreach (ApiDataModel item in bodyValues)
                    {
                        _queue.Enqueue(new TaskObject(async () =>
                        {
                            HttpStatusCode status = await UpdateAbstractModel(item.Id, item);
                            switch (status)
                            {
                                case HttpStatusCode.OK:
                                    break;
                                default:
                                    ApiErrorModel errMod = new ApiErrorModel();
                                    errMod.Title = "error";
                                    errMod.Detail = "conflic or badrequest";
                                    errMod.Id = item.Id;
                                    errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD;
                                    errMod.HttpStatus = ((int)status).ToString();
                                    errMod.Links = item.Links;
                                    response.Errors.Add(errMod);
                                    break;
                            }
                        }));

                    }

                    string message = "entities-updated";
                    _rabbitMqHandler.PublishObject(GetExchangeDeclarationName(), response, RabbitMqEntityUpdateRoutingKey, message, null, null);
                    return Accepted(response);
                }
                else//bei single node in body
                {

                    if (!CheckGuid(id) && id != null)
                    {
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a valid id" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(id) == false", methodInfo);
                    }
                    foreach (ApiDataModel item in bodyValues)
                    {

                        HttpStatusCode status = await UpdateAbstractModel(id == null ? item.Id : new Guid(id), item);
                        switch (status)
                        {
                            case HttpStatusCode.OK:
                                var tmp = Activator.CreateInstance<T>();
                                await _cacheHandler.RemoveAsync(tmp.DatabaseTable);
                                tmp.Dispose();

                                string message = "entity-updated";
                                _rabbitMqHandler.PublishObject(GetExchangeDeclarationName(), response, RabbitMqEntityUpdateRoutingKey, message, null, null);
                                return Ok(response);
                            default:
                                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = item.Id, Detail = "conflic or badrequest" }
            }, status, "an error occurred", "status != 200", methodInfo);
                        }
                    }
                }
            }

            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "bodyObject == null", methodInfo);
        }

        [NonAction]
        private async Task<HttpStatusCode> UpdateAbstractModel(Guid id, ApiDataModel item)
        {
            T requAttr = (T)item.Attributes;
            T abstractModel = Activator.CreateInstance<T>();
            abstractModel.Uuid = id;

            QueryResponseData<T> existingData = await _backendModule.Select(abstractModel, abstractModel);
            if (existingData.HasData)
            {
                QueryResponseData queryResponseData = await _backendModule.Update(requAttr, abstractModel);
                if (queryResponseData.HasErrors)
                {
                    return HttpStatusCode.Forbidden;
                }
                else
                {

                    return HttpStatusCode.OK;
                }
            }
            else
            {
                return HttpStatusCode.Conflict;
            }

        }
        /// <summary>
        /// Update an existing resource relation
        /// HTTP Methods:
        ///     - POST: Add relation to collection
        ///     - PATCH: Clear all and add specific relations
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relationname"></param>
        /// <returns>
        /// Status: 
        ///         - 200: Ok, Resource updated
        ///         - 202: Accepted, Resource is ordered in queuing to be processed
        ///         - 204: No Content, Created completed but no content is to respond
        ///         - 201: Created
        ///         - 404: Not Found, when the resource doesnt exist, e.g. the endpoint (controller)
        ///         - 403: Forbidden, Unsupported Request
        ///         - 409: Conflict, when the object that should be created already exist identified by id of the given resource or a resource that would violate server constraints e.g. a relation that doest not exist but is request to update
        /// </returns>

        /*[CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpPost("{id}/relation/{relationname}")]//reset&clear relations of given id and add new collection from given body(json-object/array)
        [HttpPatch("{id}/relation/{relationname}")]//adding collection from given body(json-object/array)
        
        public virtual async Task<ActionResult<ApiRootNodeModel>> UpdateRelation(string id, string relationname, [FromBody] ApiRootNodeModel body)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            Logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), HttpContext, this.ControllerName);
            string method = this.HttpContext.Request.Method;
            ApiRootNodeModel response = new ApiRootNodeModel();

            Guid uuid = new Guid(id);
            if (body != null)
            {
                if (!String.IsNullOrEmpty(relationname))
                {
                    string genericTypeName = _genericAbstractModelType.GetType().Name.ToLower();
                    relationname = relationname.ToLower();


                    DatabaseTableAttribute currentModelAttr = _genericAbstractModelType.GetClassDatabaseTable();

                    var list = MySqlDefinitionProperties.BackendTables.Values.ToList();
                    BackendTableModel currentModel = list.Find(x => x.ModelName.ToLower().Equals(genericTypeName));

                    var relationChainUpwardsDict = await currentModel.GetParentRelations();
                    var relationChainDownwardsDict = await currentModel.GetChildRelations();

                    DatabaseAttributeManager databaseAttributeManager = new DatabaseAttributeManager();


                    KeyValuePair<string, BackendTableModel> relationConnectorValuePair = GetRelationConnector(list, currentModel, relationname);
                    bool foundRelationConnector = relationConnectorValuePair.Value != null;




                    if (!foundRelationConnector)//update relation nur bei 1toN
                    {
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = uuid, Detail = "update relation is only available on 1toN relations\nTo update a 1to1 relation, update the property from relation" }
            }, HttpStatusCode.NotFound, "an error occurred", "foundRelationConnector == false, expected true for action", methodInfo);
                    }
                    ActionResult<ApiRootNodeModel> parent = await this.GetRelation(id, relationname);//get current relation collection, without connector class this.class<->connector<->relation.class
                    if (parent != null)
                    {
                        if (parent.Result != null)
                        {
                            Type t = parent.Result.GetType();

                            if (t == typeof(OkObjectResult))
                            {
                                OkObjectResult okObject = (OkObjectResult)parent.Result;
                                ApiRootNodeModel targetModel = (ApiRootNodeModel)okObject.Value;
                                List<ApiRelationshipModel> targetModelData = new List<ApiRelationshipModel>();
                                var data = ((ApiRelationshipModel)targetModel.Data).Data;
                                if (Utils.IsList<ApiRelationshipModel>(data))
                                {
                                    targetModelData = (List<ApiRelationshipModel>)data;
                                }
                                else
                                {
                                    if (data != null)
                                    {
                                        targetModelData.Add((ApiRelationshipModel)data);
                                    }
                                }

                                switch (okObject.StatusCode)
                                {
                                    case 200:

                                        ApiRootNodeModel bodyObject = body;
                                        List<ApiDataModel> bodyValues = new List<ApiDataModel>();//body values are the collection that should be added

                                        if (Utils.IsList<ApiDataModel>(bodyObject.Data))
                                        {

                                            bodyValues = (List<ApiDataModel>)bodyObject.Data;
                                        }
                                        else
                                        {
                                            if (bodyObject.Data != null)
                                            {
                                                bodyValues.Add((ApiDataModel)bodyObject.Data);
                                            }
                                        }
                                        List<ApiErrorModel> errors = new List<ApiErrorModel>();
                                        if (bodyValues.Count == 0)
                                        {
                                            ApiErrorModel errMod = new ApiErrorModel();
                                            errMod.Title = "error";
                                            errMod.Detail = "body is empty";
                                            errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD;
                                            errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                            errors.Add(errMod);
                                        }
                                        else
                                        {
                                            foreach (ApiDataModel item in bodyValues)
                                            {
                                                AbstractModel dataT = (AbstractModel)item.Attributes;
                                                bool validation = TryValidateModel(dataT, dataT.GetType().Name);
                                                if (!validation)
                                                {
                                                    List<string> modelErrors = new List<string>();
                                                    foreach (var member in ModelState.Keys)
                                                    {
                                                        var state = ModelState[member];
                                                        foreach (var error in state.Errors)
                                                        {
                                                            string jsonNodeName = dataT.GetJsonDisplayName(member.Replace("T.", "")) ?? member;
                                                            modelErrors.Add(jsonNodeName + " " + error.ErrorMessage);
                                                        }
                                                    }
                                                    foreach (string err in modelErrors)
                                                    {
                                                        ApiErrorModel errMod = new ApiErrorModel();
                                                        errMod.Title = "error";
                                                        errMod.Detail = err;
                                                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                                                        errMod.Id = item.Id;
                                                        errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                        errMod.Links = item.Links;
                                                        errors.Add(errMod);
                                                    }
                                                }
                                                else if ((dataT.Uuid != Guid.Empty || item.Id != Guid.Empty))
                                                {
                                                    ApiErrorModel errMod = new ApiErrorModel();
                                                    errMod.Title = "error";
                                                    errMod.Detail = "id and attribute uuid must be null or " + Guid.Empty.ToString() + " (uuid & id would be generated auto.)";
                                                    errMod.Id = item.Id;
                                                    errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                                                    errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                    errMod.Links = item.Links;
                                                    errors.Add(errMod);
                                                }
                                                else if (dataT.Uuid != item.Id)
                                                {
                                                    ApiErrorModel errMod = new ApiErrorModel();
                                                    errMod.Title = "error";
                                                    errMod.Detail = "id and attribute uuid must be equal";
                                                    errMod.Id = item.Id;
                                                    errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                                                    errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                    errMod.Links = item.Links;
                                                    errors.Add(errMod);
                                                }
                                                else
                                                {
                                                    var responseSelect = await SqlOp(dataT, item.NetType, MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT);
                                                    if (!responseSelect.HasData)
                                                    {

                                                        ApiErrorModel errMod = new ApiErrorModel();
                                                        errMod.Title = "error";
                                                        errMod.Detail = "body item is not existing in database";
                                                        errMod.Id = item.Id;
                                                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                                                        errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                        errMod.Links = item.Links;
                                                        errors.Add(errMod);
                                                    }
                                                }
                                            }
                                        }


                                        response.Errors = errors;
                                        if (response.HasErrors)
                                        {

                                            return JsonApiErrorResult(response, HttpStatusCode.UnprocessableEntity, "an error occurred", "response.HasErrors == true", methodInfo);
                                        }
                                        //mapping von daten von currentModel Instance und body-data für relation object
                                        T currentModelInstance = (T)Activator.CreateInstance(typeof(T));
                                        currentModelInstance.Uuid = uuid;


                                        object relationModelWhereClause = null;

                                        List<object> relationCollectionFromBody = new List<object>();
                                        int[] fkIndex = new int[2] { GeneralDefs.NotFoundResponseValue, GeneralDefs.NotFoundResponseValue };

                                        foreach (ApiDataModel apiDataModel in bodyValues)
                                        {
                                            object relationModelInstance = Activator.CreateInstance(relationConnectorValuePair.Value.Model);// muss neu instanziert werden

                                            object bodyValueInstance = Activator.CreateInstance(apiDataModel.NetType);
                                            DatabaseTableAttribute databaseTableAttributeBodyValueInstance = bodyValueInstance.GetType().GetCustomAttribute<DatabaseTableAttribute>();
                                            List<PropertyInfo> propertyInfosApiDataModelAttr = bodyValueInstance.GetType().GetProperties().ToList();
                                            propertyInfosApiDataModelAttr.ForEach(x =>
                                            {
                                                if (x.Name.ToLower() == "uuid")
                                                {
                                                    int idx = propertyInfosApiDataModelAttr.IndexOf(x);
                                                    propertyInfosApiDataModelAttr[idx].SetValue(bodyValueInstance, apiDataModel.Id);
                                                }
                                            });

                                            List<PropertyInfo> propertyInfosRelationModel = relationModelInstance.GetType().GetProperties().ToList();
                                            //2. propertyInfosRelationModel properties mit daten von propertyInfosCurrentModel & propertyInfosApiDataModelAttr füllen
                                            for (int i = 0; i < propertyInfosRelationModel.Count; i++)
                                            {
                                                DatabaseColumnPropertyAttribute attr = propertyInfosRelationModel[i].GetCustomAttribute<DatabaseColumnPropertyAttribute>();
                                                object value = null;
                                                if (attr != null)
                                                {

                                                    if (attr.ForeignTable == currentModel.Table)
                                                    {
                                                        if (attr.ForeignColumnName == "uuid")
                                                        {

                                                            value = uuid;
                                                            if (fkIndex[0] == GeneralDefs.NotFoundResponseValue)
                                                                fkIndex[0] = i;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int j = 0; j < propertyInfosApiDataModelAttr.Count; j++)
                                                        {
                                                            var attrSL = propertyInfosApiDataModelAttr[j].GetCustomAttributes<DatabaseColumnPropertyAttribute>().ToList();
                                                            if (attrSL == null)
                                                                continue;
                                                            var attrS = attrSL.Find(x => x.ForeignTable == relationConnectorValuePair.Value.Table);
                                                            if (attrS != null)
                                                            {
                                                                if (attrS.ColumnName == attr.ForeignColumnName && databaseTableAttributeBodyValueInstance.TableMember == attr.ForeignTable)
                                                                {

                                                                    value = propertyInfosApiDataModelAttr[j].GetValue(bodyValueInstance);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (value != null)
                                                    {
                                                        propertyInfosRelationModel[i].SetValue(relationModelInstance, value);

                                                        if (fkIndex[1] == GeneralDefs.NotFoundResponseValue)
                                                            fkIndex[1] = i;

                                                        if (relationModelWhereClause == null)
                                                        {
                                                            relationModelWhereClause = Activator.CreateInstance(relationConnectorValuePair.Value.Model);
                                                            relationModelWhereClause.GetType().GetProperties()[i].SetValue(relationModelWhereClause, value);
                                                        }

                                                    }
                                                }
                                            }
                                            relationCollectionFromBody.Add(relationModelInstance);

                                        }


                                        //3. je nach HTTP Method aktionen ausführen (target: relationCollectionFromBody)
                                        //sadasdsad

                                        //hier nach Update + Create testen + Delete (HTTP Controller methoden) [x] nur noch error responses nachtragen [x] caching aufrüsten (db objekte + http api responses) [x] und testen []
                                        //relation types und maximale anzahl an relations type pro controller node/item prüfen über DatabaseTableAttribute von Abstractmodels []
                                        //ticket system vollenden + implementation für konversationsführung via react app (chat system) + dsgvo modul
                                        //cookie policy modal für react app front end + safe in db
                                        //ilogger sollen nicht nur in konsole schreiben sondern auch in db / logcontroller oder so erstellen
                                        //permission system ausgiebig testen 
                                        //Update+Create Post/Patch Body Parameter JsonDocument gegen ApiRootNodeModel tauschen[x] + DatenValidator Custom für eigenen Response Msg anstatt standard Net Msg[x] gelöst mit ValidateModelFilter
                                        //+ Upload + Download Functions generic bereitstellen über ApiControllerBase, bsp. in DriverVersionFileController->GetDriver(string id, string file) + PutFile (Upload Methode)
                                        //Generisches Schema für FileUpload + Download und Struktur auf Filesystem (bsp. jeder controller kriegt ein file verzeichnis), beispiel nach umsetzun: DriverVersionFileController [x]
                                        //apidatamodel muss page index beinhalten für pagination + max items für page sollte über http query setzbar sein
                                        //alle Controller Aktionen nochmal testen + Responses
                                        //Wird bei Session Logout der JWT Token aus Cache entfernt? Prüfen, Lifetime von Max-ResetToken Lifetime setzen für CacheEntry

                                        QueryResponseData<object> currentCollectionFromDatabase = await SqlOp((AbstractModel)relationModelWhereClause, relationModelWhereClause.GetType(), MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT);
                                        List<ApiDataModel> returnValues = new List<ApiDataModel>();
                                        List<AbstractModel> returnValuesAbstractModels = new List<AbstractModel>();
                                        switch (method)
                                        {
                                            case "POST":

                                                //reset all relations and set with new collection
                                                if (currentCollectionFromDatabase.DataStorage != null)
                                                {
                                                    foreach (object item in currentCollectionFromDatabase.DataStorage)
                                                    {
                                                        AbstractModel model = (AbstractModel)item;
                                                        var responseDelete = await SqlOp(model, item.GetType(), MySqlDefinitionProperties.SQL_STATEMENT_ART.DELETE);
                                                        if (responseDelete.HasErrors)
                                                        {
                                                            ApiErrorModel errMod = new ApiErrorModel();
                                                            errMod.Title = "error";
                                                            errMod.Detail = "deletion of item not possible";
                                                            errMod.Id = model.Uuid;
                                                            errMod.Code = ApiErrorModel.ERROR_CODES.INTERNAL;
                                                            errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                            errors.Add(errMod);
                                                        }
                                                    }
                                                }
                                                foreach (object item in relationCollectionFromBody)
                                                {
                                                    AbstractModel model = (AbstractModel)item;
                                                    var responseInsert = await SqlOp(model, item.GetType(), MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT);
                                                    if (responseInsert.HasErrors)
                                                    {
                                                        ApiErrorModel errMod = new ApiErrorModel();
                                                        errMod.Title = "error";
                                                        errMod.Detail = "insertion of item not possible";
                                                        errMod.Id = model.Uuid;
                                                        errMod.Code = ApiErrorModel.ERROR_CODES.INTERNAL;
                                                        errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                        errors.Add(errMod);
                                                    }
                                                    else
                                                    {

                                                        returnValuesAbstractModels.Add((AbstractModel)responseInsert.FirstRow);
                                                    }
                                                }


                                                break;
                                            case "PATCH":
                                                //add collection to current relations collaction

                                                foreach (object item in relationCollectionFromBody)
                                                {
                                                    AbstractModel model = (AbstractModel)item;
                                                    bool canAdd = false;
                                                    var responseSelect = await SqlOp(model, item.GetType(), MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT);
                                                    if (responseSelect.HasData)//prüfung ob model bereits vorhanden ist
                                                    {
                                                        if (responseSelect.DataStorage.Count > 1)
                                                        {
                                                            ApiErrorModel errMod = new ApiErrorModel();
                                                            errMod.Title = "error";
                                                            errMod.Detail = "model already in relation";
                                                            errMod.Id = model.Uuid;
                                                            errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                                                            errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                            errors.Add(errMod);
                                                        }
                                                        else//existiert nicht also add
                                                        {
                                                            canAdd = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        canAdd = true;

                                                    }
                                                    if (canAdd)
                                                    {

                                                        var responseInsert = await SqlOp(model, item.GetType(), MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT);
                                                        if (responseInsert.HasErrors)
                                                        {
                                                            ApiErrorModel errMod = new ApiErrorModel();
                                                            errMod.Title = "error";
                                                            errMod.Detail = responseInsert.ErrorCode + ": " + responseInsert.Message;
                                                            errMod.Id = model.Uuid;
                                                            errMod.Code = ApiErrorModel.ERROR_CODES.INTERNAL;
                                                            errMod.HttpStatus = ((int)System.Net.HttpStatusCode.UnprocessableEntity).ToString();
                                                            errors.Add(errMod);
                                                        }
                                                        else
                                                        {
                                                            returnValuesAbstractModels.Add((AbstractModel)responseInsert.FirstRow);
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                        if (response.HasErrors)
                                        {
                                            return JsonApiErrorResult(response, HttpStatusCode.UnprocessableEntity, "an error occurred", "response.HasErrors == true", methodInfo);

                                        }
                                        else
                                        {
                                            returnValues = await _jsonApiHandler.GetConvertedFromModel(this.GetArea().RouteValue, returnValuesAbstractModels, false, 0, false);
                                            response.Data = returnValues;
                                        }

                                        return Ok(response);
                                        break;
                                }
                                return parent;
                            }
                            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.NotFound, "an error occurred", "t != typeof(OkObjectResult)", methodInfo);
                        }
                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.NotFound, "an error occurred", "parent.Result == null", methodInfo);
                    }
                    return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.NotFound, "an error occurred", "parent == null", methodInfo);
                }

                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "String.IsNullOrEmpty(relationname) == false", methodInfo);
            }

            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = uuid, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "body == null", methodInfo);
        }*/

        /// <summary>
        /// Create a resource
        /// </summary>
        /// <param name="json"></param>
        /// <returns>
        /// Status: 
        ///         - 202: Accepted, Resource is ordered in queuing to be processed
        ///         - 204: No Content, Created completed but no content is to respond
        ///         - 201: Created
        ///         - 404: Not Found, when the resource doesnt exist, e.g. the endpoint (controller)
        ///         - 403: Forbidden, Unsupported Request
        ///         - 409: Conflict, when the object that should be created already exist identified by id of the given resource
        /// </returns>
        /// Example Postman Request:
        /// Method: POST
        /// URL: http://localhost:5001/apiv1/software/
        /// Header:
        ///     - Content-Type: application/json
        ///     - Authorization: [JWT Bearer]
        /// Body:
        ///{
        ///    "data": {
        ///        "type": "softwaremodel",
        ///        "attributes": {
        ///            "softwareName": "neuanlage2",
        ///            "softwareDescription": "testbeschreibung",
        ///            "active": true
        ///        }
        ///    },
        ///    "meta": {
        ///        "count": 1
        ///    },
        ///    "jsonapi": {
        ///        "version": "1.0",
        ///        "company": "HelixDBM GmbH",
        ///        "author": "Joel Mika Roos for HelixDBM GmbH",
        ///        "copyright": "HelixDBM GmbH",
        ///        "use": "Backend API for Helix DMBS Tool and the Official Helix DBM Website",
        ///        "rfc": "RFC 7159"
        ///    }
        ///}
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpPost]
        [NonAction]
        public virtual async Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body, [OpenApiIgnoreMethodParameter] bool allowDuplicates)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            Logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), HttpContext, ControllerName);
            ApiRootNodeModel bodyObject = body;

            if (bodyObject != null)
            {
                List<ApiDataModel> bodyValues = new List<ApiDataModel>();
                if (Utils.IsList<ApiDataModel>(bodyObject.Data))
                {
                    bodyValues = (List<ApiDataModel>)bodyObject.Data;
                }
                else
                {
                    if (bodyObject.Data != null)
                    {
                        bodyValues.Add((ApiDataModel)bodyObject.Data);
                    }
                }
                if (bodyValues.Count == 0)
                {
                    return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "bodyValues.Count == 0", methodInfo);
                }
                List<AbstractModel> itemList = new List<AbstractModel>();
                List<ApiErrorModel> errors = new List<ApiErrorModel>();
                List<string> modelErrors = new List<string>();
                foreach (ApiDataModel item in bodyValues)
                {
                    var currentItemType = item.Attributes?.GetType();
                    if (currentItemType?.BaseType == typeof(AbstractModel))
                    {

                        AbstractModel dataT = (AbstractModel)item.Attributes;
                        itemList.Add(dataT);
                        bool validation = TryValidateModel(item.Attributes, item.NetType.Name);
                        if (!validation)
                        {
                            foreach (var member in ModelState.Keys)
                            {
                                var state = ModelState[member];
                                foreach (var error in state.Errors)
                                {
                                    string jsonNodeName = dataT.GetJsonDisplayName(member.Replace("T.", "")) ?? member;
                                    modelErrors.Add(jsonNodeName + " " + error.ErrorMessage);
                                }
                            }
                            foreach (string err in modelErrors)
                            {
                                ApiErrorModel errMod = new ApiErrorModel();
                                errMod.Title = "error";
                                errMod.Detail = err;
                                errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                                errMod.Id = item.Id;
                                errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                                errMod.Links = item.Links;
                                errors.Add(errMod);
                            }
                        }
                        else if (dataT.Uuid != Guid.Empty || item.Id != Guid.Empty)
                        {
                            ApiErrorModel errMod = new ApiErrorModel();
                            errMod.Title = "error";
                            errMod.Detail = "id and attribute uuid must be null or " + Guid.Empty.ToString() + " (uuid & id would be generated auto.)";
                            errMod.Id = item.Id;
                            errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                            errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                            errMod.Links = item.Links;
                            errors.Add(errMod);
                        }
                    }
                    else//item in collection ist kein Object vom Type AbstractModel
                    {
                        ApiErrorModel errMod = new ApiErrorModel();
                        errMod.Title = "error";
                        errMod.Detail = "" + item.Type + " is not a valid model";
                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNPROCESSABLE_ENTITY;
                        errMod.Id = item.Id;
                        errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                        errMod.Links = item.Links;
                        errors.Add(errMod);
                    }
                }
                ApiRootNodeModel response = await _jsonApiHandler.CreateApiRootNodeFromModel(GetArea().RouteKey, itemList);
                response.Errors = errors;
                if (response.HasErrors)
                {
                    return JsonApiErrorResult(response, HttpStatusCode.UnprocessableEntity, "an error occurred", "response.HasErrors == true", methodInfo);
                }
                CreatedAbstractModelResponse<AbstractModel> lastCreatedObject = null;
                int index = 0;
                var transaction = await _databaseHandler.CreateTransaction();
                foreach (ApiDataModel item in bodyValues)
                {
                    if (lastCreatedObject != null)
                    {
                        if (lastCreatedObject.CreatedObject != null)
                        {
                            var abstractModel = (AbstractModel)item.Attributes;
                            string currentEntityType = abstractModel.DatabaseTable;
                            var relationObject = lastCreatedObject.CreatedObject.DatabaseRelations?.Find(x => x.EntityOne.ToLower() == currentEntityType || x.EntityTwo.ToLower() == currentEntityType);
                            if (relationObject != null)
                            {
                                object propertySetterValue = null;
                                string currentTableForeignColumnName = null;
                                string foreignTableColumnName = null;
                                if (relationObject.EntityOne == currentEntityType)
                                {
                                    foreignTableColumnName = relationObject.EntityOneKeyCol;
                                    currentTableForeignColumnName = relationObject.EntityTwoKeyCol;
                                }
                                else
                                {
                                    foreignTableColumnName = relationObject.EntityTwoKeyCol;
                                    currentTableForeignColumnName = relationObject.EntityOneKeyCol;
                                }
                                if (foreignTableColumnName != null && currentTableForeignColumnName != null)
                                {

                                    PropertyInfo propertyInfo = lastCreatedObject.CreatedObject.GetType().GetProperties().ToList().Find(x => x.GetCustomAttribute<DatabaseColumnPropertyAttribute>()?.ColumnName == currentTableForeignColumnName);
                                    if (propertyInfo != null)
                                    {
                                        propertySetterValue = propertyInfo.GetValue(lastCreatedObject.CreatedObject);
                                    }
                                    if (propertySetterValue != null)
                                    {

                                        PropertyInfo propertyInfoForeignTableColumn = item.Attributes.GetType().GetProperties().ToList().Find(x => x.GetCustomAttribute<DatabaseColumnPropertyAttribute>()?.ColumnName == foreignTableColumnName);
                                        propertyInfoForeignTableColumn.SetValue(item.Attributes, propertySetterValue);
                                    }
                                }
                            }
                        }
                    }
                    CreatedAbstractModelResponse<AbstractModel> createdResponse = await CreateAbstractModel(item, bodyObject, transaction, allowDuplicates);
                    HttpStatusCode status = createdResponse.HttpStatusCode;

                    switch (status)
                    {
                        case HttpStatusCode.Created:
                            item.Attributes = createdResponse.CreatedObject;
                            item.Id = createdResponse.CreatedObject.Uuid;
                            lastCreatedObject = createdResponse;
                            break;
                        default:
                            ApiErrorModel errMod = new ApiErrorModel();
                            errMod.Title = "error";
                            errMod.Detail = status == HttpStatusCode.Conflict ? "dataset for '" + item.Type + "' already exists, operation cancelled at this point" : "bad request";
                            errMod.Id = item.Id;
                            errMod.Code = (ApiErrorModel.ERROR_CODES)status;
                            errMod.HttpStatus = ((int)status).ToString();
                            errMod.Links = item.Links;
                            response.Errors.Add(errMod);
                            break;
                    }
                    if (response.HasErrors)
                    {
                        _databaseHandler.Rollback(transaction);
                        break;
                    }

                    index++;
                }
                if (response.Errors.Count > 0)
                {
                    return JsonApiErrorResult(response.Errors, HttpStatusCode.BadRequest, "an error occurred", "status != 200", methodInfo);
                }
                else
                {
                    var changedTypes = bodyValues.GroupBy(x => ((AbstractModel)x.Attributes).DatabaseTable).Select(x => x.Key).Distinct().ToList();
                    foreach (var v in changedTypes)
                    {
                        await _cacheHandler.RemoveAsync(v);
                    }
                    _databaseHandler.Commit(transaction);
                    response = await _jsonApiHandler.CreateApiRootNodeFromModel(GetArea().RouteKey, itemList);

                    string message = "entity-created";
                    _rabbitMqHandler.PublishObject(GetExchangeDeclarationName(), response, RabbitMqEntityCreateRoutingKey, message, null, null
                        );

                    return Created(HttpContext.Request.Path.Value, response);
                }
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "bodyObject == null", methodInfo);
        }

        [NonController]
        public class CreatedAbstractModelResponse<T3>
            where T3 : AbstractModel
        {
            public T3 CreatedObject = default;
            public QueryResponseData QueryResponse { get; set; }
            public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.BadRequest;
        }

        [NonAction]
        private async Task<CreatedAbstractModelResponse<AbstractModel>> CreateAbstractModel(ApiDataModel item, object httpCreatedResponseValue, System.Data.Common.DbTransaction transaction, bool allowDuplicates)
        {
            CreatedAbstractModelResponse<AbstractModel> responseValue = new CreatedAbstractModelResponse<AbstractModel>();
            AbstractModel dataT = (AbstractModel)item.Attributes;
            /*Type type = typeof(AbstractBackendModule<>).MakeGenericType(new Type[] { item.NetType });
            var backendModule= Activator.CreateInstance(type,args:new object[] { _databaseHandler,_cacheHandler });
            QueryResponseData<AbstractModel> existingData = await ((AbstractBackendModule<AbstractModel>)backendModule).Select(dataT, dataT);*/

            string selectQuery = dataT.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, dataT, dataT).ToString();
            QueryResponseData<object> existingData = await _databaseHandler.ExecuteQueryWithMap(selectQuery, dataT, item.NetType);


            if (!existingData.HasData || allowDuplicates)
            {
                string insertQuery = dataT.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                QueryResponseData data = await _databaseHandler.ExecuteQuery<AbstractModel>(insertQuery, dataT, transaction: transaction);
                if (data.HasSuccess)
                {
                    dataT.Uuid = new Guid((string)data.LastInsertedId);
                    selectQuery = dataT.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, dataT, dataT).ToString();
                    existingData = await _databaseHandler.ExecuteQueryWithMap(selectQuery, dataT, item.NetType);
                    if (existingData.HasData)
                    {

                        responseValue.QueryResponse = data;
                        responseValue.CreatedObject = (AbstractModel)existingData.FirstRow;
                        responseValue.HttpStatusCode = HttpStatusCode.Created;

                    }
                    else
                    {

                        responseValue.HttpStatusCode = HttpStatusCode.InternalServerError;
                    }
                }
            }
            else//conflict
            {
                responseValue.HttpStatusCode = HttpStatusCode.Conflict;
            }
            return responseValue;
        }


        /// <summary>
        /// Delete a resource and they foreign resources
        /// </summary>
        /// <param name="id"></param>
        /// Status: 
        ///         - 202: Accepted, Resource is ordered in queuing to be processed
        ///         - 204: No Content, Deletion completed but no content is to respond
        ///         - 200: Ok, Deleted
        ///         - 404: Not Found
        /// </returns>
        /// 
        [CustomProducesFilter(GeneralDefs.ApiContentType)]
        [CustomConsumesFilter(GeneralDefs.ApiContentType)]
        [HttpDelete(BackendAPIDefinitionsProperties.ActionParameterIdWildcard)]
        [NonAction]
        public virtual async Task<ObjectResult> Delete(string id)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (!CheckGuid(id))
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a valid id" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(id) == false", methodInfo);
            }

            Logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), HttpContext, ControllerName);
            T instance = Activator.CreateInstance<T>();
            instance.Uuid = new Guid(id);


            string genericTypeName = _genericAbstractModelType.GetType().Name.ToLower();

            ApiRootNodeModel response = new ApiRootNodeModel();
            List<ApiDataModel> responseDeletionValues = new List<ApiDataModel>();
            ObjectResult parent = await Get(id);
            if (parent != null)
            {
                if (parent.Value != null)
                {
                    Type t = parent.Value.GetType();

                    switch (parent.StatusCode)
                    {
                        case 200:
                            ApiRootNodeModel model = (ApiRootNodeModel)parent.Value;
                            if (model.Data != null)
                            {
                                bool isDataList = Utils.IsList<ApiDataModel>(model.Data);
                                List<ApiDataModel> targetModels = new List<ApiDataModel>();
                                if (isDataList)
                                {
                                    targetModels = (List<ApiDataModel>)model.Data;
                                }
                                else
                                {
                                    targetModels.Add((ApiDataModel)model.Data);
                                }
                                response.Errors = new List<ApiErrorModel>();
                                foreach (ApiDataModel item in targetModels)
                                {
                                    AbstractModel abstractModel = (AbstractModel)item.Attributes;
                                    if (!abstractModel.Deleted)
                                    {
                                        var queryResponse = await DeleteObject(item);
                                        if (queryResponse == HttpStatusCode.InternalServerError)
                                        {

                                            ApiErrorModel errMod = new ApiErrorModel();
                                            errMod.Title = "error";
                                            errMod.Detail = queryResponse.ToString();
                                            errMod.Id = item.Id;
                                            errMod.Code = ApiErrorModel.ERROR_CODES.INTERNAL;
                                            errMod.HttpStatus = ((int)HttpStatusCode.UnprocessableEntity).ToString();
                                            response.Errors.Add(errMod);
                                        }
                                        else
                                        {
                                            responseDeletionValues.Add(item);

                                        }
                                    }
                                    else
                                    {
                                        ApiErrorModel errMod = new ApiErrorModel();
                                        errMod.Title = "error";
                                        errMod.Detail = "already deleted";
                                        errMod.Id = item.Id;
                                        errMod.Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_CONFLICT;
                                        errMod.HttpStatus = ((int)HttpStatusCode.Conflict).ToString();
                                        response.Errors.Add(errMod);
                                    }
                                }
                                response.Data = responseDeletionValues;
                                if (response.HasErrors)
                                {
                                    return JsonApiErrorResult(response.Errors, HttpStatusCode.BadRequest, "an error occurred", "response.HasErrors == true", methodInfo);
                                }

                                var tmp = Activator.CreateInstance<T>();
                                await _cacheHandler.RemoveAsync(tmp.DatabaseTable);
                                tmp.Dispose();

                                string message = "entity-deleted";
                                _rabbitMqHandler.PublishObject(GetExchangeDeclarationName(), response, RabbitMqEntityDeleteRoutingKey, message, null, null);
                                return Ok(response);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return parent;
        }
        #endregion

        #region InteralMethods

        [NonAction]
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [NonAction]
        private async Task<HttpStatusCode> DeleteObject(ApiDataModel model)
        {
            if (model != null)
            {
                var tmp = (AbstractModel)model.Attributes;
                tmp.Deleted = true;
                model.Attributes = tmp;
                return await UpdateAbstractModel(model.Id, model);
            }
            return HttpStatusCode.InternalServerError;
        }

        [NonAction]
        protected async Task<QueryResponseData<object>> SqlOp(AbstractModel abstractModelFieldValues, AbstractModel abstractModelWhereClause, Type type, MySqlDefinitionProperties.SQL_STATEMENT_ART statementType)
        {

            string query = abstractModelWhereClause.GenerateQuery(statementType, statementType == MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT ? null : abstractModelWhereClause, abstractModelFieldValues).ToString();
            return await _databaseHandler.ExecuteQueryWithMap(query, abstractModelFieldValues, type);
        }

        #endregion
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class CustomApiV1ControllerBase<T> : CustomApiControllerBase<T>
        where T : AbstractModel
    {
        public CustomApiV1ControllerBase(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<T> abstractBackendModule, IServiceProvider serviceProvider) :
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
            var t = GetRoutes();
        }
        public CustomApiV1ControllerBase(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<T> abstractBackendModule, IServiceProvider serviceProvider) :
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {

        }
        public CustomApiV1ControllerBase() : base() 
        {
            
        }
    }

}
