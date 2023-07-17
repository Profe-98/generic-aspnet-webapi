﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Data;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net.Mime;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Cors;
using System.Runtime.CompilerServices;
using WebApiFunction.Application.Model.Internal;

using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Web.AspNet.Filter;
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
using WebApiFunction.Web.AspNet.CustomActionResult;
using WebApiFunction.Application;
using InfluxDB.Client.Api.Domain;
using Microsoft.AspNetCore.Identity;
using WebApiFunction.Web.Websocket.SignalR.HubService;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Database.MySQL.Table;
using WebApiFunction.Application.Model.Database.MySQL.View;
using WebApiFunction.Application.Model.Database.MySQL;

namespace WebApiFunction.Web.AspNet.Controller
{
    [EnableCors("api-gateway")]
    public class CustomControllerBase : ControllerBase, IDisposable
    {
        #region Private
        private Guid _internalId;

        private readonly IActionSelector _actionSelector;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICachingHandler _cachingHandler;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqHandler _rabbitMqHandler;
        #endregion
        #region Public
        public string ControllerName
        {
            get
            {
                string tmpName = GetType().Name.ToLower();
                if (tmpName == "ControllerActionRelationToHttpMethodController")
                { }

                int countController = tmpName.LastIndexOf("controller");
                tmpName = tmpName.Substring(0, countController);


                return tmpName;
            }
        }
        #endregion
        #region Ctor
        public CustomControllerBase()
        {
        }
        public CustomControllerBase(IWebHostEnvironment env, IConfiguration configuration, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IActionSelector actionSelector) : this()
        {
            _webHostEnvironment = env;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _actionSelector = actionSelector;
            _configuration = configuration;

        }
        public CustomControllerBase(IWebHostEnvironment env, ICachingHandler cachingHandler, IConfiguration configuration, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IActionSelector actionSelector, IRabbitMqHandler rabbitMqHandler) : this()
        {
            _webHostEnvironment = env;
            _cachingHandler = cachingHandler;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _actionSelector = actionSelector;
            _configuration = configuration;
            _rabbitMqHandler = rabbitMqHandler;

        }
        public CustomControllerBase(IWebHostEnvironment env, ICachingHandler cachingHandler, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler) : this()
        {
            _webHostEnvironment = env;
            _cachingHandler = cachingHandler;
            _configuration = configuration;
            _rabbitMqHandler = rabbitMqHandler;
        }

        #endregion
        #region HttpMethods


        #endregion
        #region InternalMethods

        [NonAction]
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public ActionDescriptor GetMatchingAction(string path, string httpMethod)
        {
            var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors.Items;
            // match by route template
            var matchingDescriptors = new List<ActionDescriptor>();
            foreach (var actionDescriptor in actionDescriptors)
            {
                if (actionDescriptor.AttributeRouteInfo == null)
                    continue;
                var matchesRouteTemplate = MatchesTemplate(actionDescriptor.AttributeRouteInfo.Template, path);
                if (matchesRouteTemplate)
                {
                    matchingDescriptors.Add(actionDescriptor);
                }
            }

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = path;
            httpContext.Request.Method = httpMethod;
            var routeContext = new RouteContext(httpContext);
            ActionDescriptor action = null;
            try
            {
                action = _actionSelector.SelectBestCandidate(routeContext, matchingDescriptors.AsReadOnly());
            }
            catch (AmbiguousActionException ex)
            {

            }
            return action;
        }


        public bool MatchesTemplate(string routeTemplate, string requestPath)
        {
            var template = TemplateParser.Parse(routeTemplate);

            var matcher = new TemplateMatcher(template, GetDefaults(template));
            var values = new RouteValueDictionary();
            return matcher.TryMatch(requestPath, values);
        }

        private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }
        public bool CheckGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return false;

            try
            {
                if (Guid.TryParse(guid.ToCharArray(), out Guid result))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public AreaAttribute GetArea()
        {
            Type classType = GetType();
            if (classType != null)
            {
                Attribute attribute = Attribute.GetCustomAttribute(classType, typeof(AreaAttribute));
                if (attribute != null)
                {
                    return (AreaAttribute)attribute;
                }
            }
            return null;
        }
        public List<RouteAttribute> GetRoutes()
        {
            List<RouteAttribute> routeAttributes = new List<RouteAttribute>();
            Type classType = GetType();
            if (classType != null)
            {
                var attributes = Attribute.GetCustomAttributes(classType, typeof(RouteAttribute));
                if (attributes != null)
                {
                    attributes.ToList().ForEach(x => routeAttributes.Add((RouteAttribute)x));
                }
            }
            List<RouteAttribute> tmp = GetMethodAttributesGeneric<RouteAttribute>(GetType());
            foreach (RouteAttribute r in tmp)
            {
                routeAttributes.Add(r);
            }
            return routeAttributes;
        }
        public List<HttpMethodAttribute> GetHttpMethods()
        {
            return GetMethodAttributesGeneric<HttpMethodAttribute>(GetType());
        }
        public List<HttpMethodAttribute> GetHttpMethods(Type type)
        {
            return GetMethodAttributesGeneric<HttpMethodAttribute>(type);
        }
        private List<T> GetAttributesGeneric<T>(Type type) where T : Attribute
        {

            List<T> responseValues = new List<T>();
            List<PropertyInfo> parentModelProperties = type.GetProperties().ToList();
            for (int j = 0; j < parentModelProperties.Count; j++)
            {
                PropertyInfo item = parentModelProperties[j];
                List<Attribute> attributes = item.GetCustomAttributes(typeof(T)).ToList();
                if (attributes.Count != 0)
                {
                    Attribute attribute = attributes.First();
                    if (attribute != null)
                    {
                        T routeAttr = (T)attribute;
                        responseValues.Add(routeAttr);
                    }
                }
            }
            return responseValues;
        }
        private List<T> GetMethodAttributesGeneric<T>(Type type) where T : Attribute
        {

            List<T> responseValues = new List<T>();
            List<MethodInfo> parentModelProperties = type.GetMethods().ToList();
            for (int j = 0; j < parentModelProperties.Count; j++)
            {
                MethodInfo item = parentModelProperties[j];
                List<Attribute> attributes = item.GetCustomAttributes(typeof(T)).ToList();
                if (attributes.Count != 0)
                {
                    Attribute attribute = attributes.First();
                    if (attribute != null)
                    {
                        T routeAttr = (T)attribute;
                        responseValues.Add(routeAttr);
                    }
                }
            }
            return responseValues;
        }
        #endregion

        #region ActionsResuls

        public JsonApiObjectResult JsonApiResult(List<ApiDataModel> value, HttpStatusCode httpStatusCode, string message = null, string debugMessage = null, object debugObj = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            bool devMode = _webHostEnvironment.IsDevelopment();
            JsonApiObjectResult result = new JsonApiObjectResult(value, message);
            result.StatusCode = (int)httpStatusCode;
            result.Value = new ApiRootNodeModel()
            {
                Data = value,
                Meta = new ApiMetaModel
                {
                    Count = value.Count,
                    OptionalMessage = message,
                    DebugMessage = devMode ? debugMessage + string.Format(" at {0} in {1}, Line: {2}", file, member, line) : null,
                    DebugObject = devMode ? debugObj : null
                },
                Jsonapi = ApiRootNodeModel.GetApiInformation()
            };

            AppendHeaderWhenStatusTooManyRequ(httpStatusCode);
            return result;
        }
        public JsonApiErrorResult JsonApiErrorResult(List<ApiErrorModel> value, HttpStatusCode httpStatusCode, string message = null, string debugMessage = null, object debugObj = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            bool devMode = _webHostEnvironment.IsDevelopment();
            JsonApiErrorResult result = new JsonApiErrorResult(value, message);
            result.StatusCode = (int)httpStatusCode;
            result.Value = new ApiRootNodeModel()
            {
                Data = null,
                Errors = value,
                Meta = new ApiMetaModel
                {
                    Count = value.Count,
                    OptionalMessage = message,
                    DebugMessage = devMode ? debugMessage + string.Format(" at {0} in {1}, Line: {2}", file, member, line) : null,
                    DebugObject = devMode ? debugObj : null
                },
                Jsonapi = ApiRootNodeModel.GetApiInformation()
            };

            AppendHeaderWhenStatusTooManyRequ(httpStatusCode);
            return result;
        }
        public static JsonApiErrorResult JsonApiErrorResultS(List<ApiErrorModel> value, HttpStatusCode httpStatusCode, string message = null, string debugMessage = null, object debugObj = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            JsonApiErrorResult result = new JsonApiErrorResult(value, message);
            result.StatusCode = (int)httpStatusCode;
            value.ForEach(x => x.HttpStatus = ((int)httpStatusCode).ToString());
            result.Value = new ApiRootNodeModel()
            {
                Data = null,
                Errors = value,
                Meta = new ApiMetaModel
                {
                    Count = value.Count,
                    OptionalMessage = message,
                },
                Jsonapi = ApiRootNodeModel.GetApiInformation()
            };

            return result;
        }
        public JsonApiErrorResult JsonApiErrorResult(ApiRootNodeModel value, HttpStatusCode httpStatusCode, string message = null, string debugMessage = null, object debugObj = null)
        {
            bool devMode = _webHostEnvironment.IsDevelopment();
            JsonApiErrorResult result = new JsonApiErrorResult(value.Errors, message);
            result.StatusCode = (int)httpStatusCode;
            value.Jsonapi = ApiRootNodeModel.GetApiInformation();
            result.Value = value;
            AppendHeaderWhenStatusTooManyRequ(httpStatusCode);
            return result;
        }
        public static JsonApiErrorResult JsonApiErrorResultS(ApiRootNodeModel value, HttpStatusCode httpStatusCode, string message = null, string debugMessage = null, object debugObj = null)
        {
            JsonApiErrorResult result = new JsonApiErrorResult(value.Errors, message);
            result.StatusCode = (int)httpStatusCode;
            value.Jsonapi = ApiRootNodeModel.GetApiInformation();
            result.Value = value;
            return result;
        }
        private void AppendHeaderWhenStatusTooManyRequ(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.TooManyRequests)
            {
                bool adding = Response.Headers.TryAdd("Retry-After", new Microsoft.Extensions.Primitives.StringValues("" + BackendAPIDefinitionsProperties.SiteProtectorBanTime.TotalSeconds));
            }
        }
        #endregion

    }

    public static class CustomControllerBaseExtensions
    {
        public static Task<ObjectResult> ToJsonApiObjectResultTaskResult(this JsonApiErrorResult jsonApiObjectResult)
        {
            return Task.FromResult((ObjectResult)jsonApiObjectResult);
        }
        public static bool IsErrorControllerRequest(this PathString uri)
        {
            if (uri.Value == null)
                return false;

            bool response = uri.Value == BackendAPIDefinitionsProperties.ProductiveErrorController || uri.Value == BackendAPIDefinitionsProperties.DebugErrorController;
            return response;
        }
        public static bool IsHealthControllerRequest(this PathString uri)
        {
            if (uri.Value == null)
                return false;

            bool response = uri.Value == BackendAPIDefinitionsProperties.HealthController;
            return response;
        }
        public static bool IsBackendRequest(this PathString uri)
        {
            return uri.Value == null ?
                false : uri.Value.StartsWith("/" + GeneralDefs.ApiAreaV1);
        }
        public static List<string> GetUriParts(this PathString uri)
        {
            return uri.Value?.ToString().ToLower().Split('/')?.ToList();
        }

        public static Dictionary<string, Uri> GetAuthentificationEndpoint(this HttpContext context)
        {
            Dictionary<string, Uri> responseValue = new Dictionary<string, Uri>();
            List<ApiModel> apis = AppManager.Api;
            foreach (ApiModel api in apis)
            {
                responseValue.Add(api.Name, api.GetRouteToController(api.AuthController));
            }
            return responseValue;
        }
        public static ActionResult Cookie(this ActionResult result, HttpContext context, string key, string value, CookieOptions cookieOptions = null)
        {
            bool exist = context.Request.Cookies.ContainsKey(key);
            if (exist)
            {
                context.Response.Cookies.Delete(key);
            }
            context.Response.Cookies.Append(
                    key,
                    value,
                    cookieOptions == null ?
                        new CookieOptions()
                        {
                            Secure = true,
                            HttpOnly = true,
                            Path = "/"
                        } : cookieOptions
                );

            return result;
        }
        public static string HeaderValueGet(this HttpContext context, string key, bool headerFromResponse = false)
        {
            if (context == null)
                return null;

            bool exist = headerFromResponse ?
                context.Response.Headers.ContainsKey(key) : context.Request.Headers.ContainsKey(key);
            if (exist)
            {
                return headerFromResponse ?
                    context.Response.Headers[key] : context.Request.Headers[key];
            }
            return null;
        }
        public static void HeaderValueSet(this HttpContext context, string key, string value, bool headerFromResponse = false)
        {
            if (context == null)
                return;

            if (!string.IsNullOrEmpty(key))
                key = key.ToLower();

            bool exist = headerFromResponse ?
                context.Response.Headers.ContainsKey(key) : context.Request.Headers.ContainsKey(key);
            if (exist)
            {
                if (headerFromResponse)
                {
                    context.Response.Headers[key] = value;
                }
                else
                {
                    context.Request.Headers[key] = value;
                }
            }
            else
            {
                if (headerFromResponse)
                {
                    context.Response.Headers.Add(key, value);
                }
                else
                {
                    context.Request.Headers.Add(key, value);
                }
            }
        }
        public static string GetRequestJWTFromHeader(this HttpContext context)
        {
            string headerValue = context.HeaderValueGet("Authorization");
            if (headerValue != null)
            {
                string token = null;
                if (!string.IsNullOrEmpty(headerValue))
                {
                    string[] stringSplitter = headerValue.Split(' ');
                    if (stringSplitter.Length == 2)
                    {
                        token = stringSplitter[1];
                        if (string.IsNullOrEmpty(token))
                            return null;

                        return token;
                    }
                }
            }
            return null;
        }

        public static ILogger TraceHttpTraffic(this ILogger logger, MethodBase executedMethod, HttpContext httpContext, string caller)
        {
            string traceId = httpContext.TraceIdentifier;
            try
            {

                string remoteIp = null;
                int remotePort = 0;
                string httpMethod = null;
                string httpPath = null;
                string userAgent = httpContext.HeaderValueGet("user-agent");
                if (httpContext.Connection != null)
                {
                    if (httpContext.Connection.RemoteIpAddress != null)
                    {
                        remoteIp = httpContext.Connection.RemoteIpAddress.ToString();
                        remotePort = httpContext.Connection.RemotePort;
                    }
                }
                if (httpContext.Request != null)
                {
                    if (httpContext.Request.Path != null)
                    {
                        httpPath = httpContext.Request.Path.Value ?? "/";
                    }
                    if (httpContext.Request.Method != null)
                    {
                        httpMethod = httpContext.Request.Method;
                    }
                }


                if (httpContext.Response.HasStarted)
                {
                    string tmp = string.Format("Flow(Trade-Id: {1})= <-- HTTP-Response: {2}={3}, {4} [{5}:{6}], Uri={7}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFF"), traceId, httpMethod, httpContext.Response.StatusCode, userAgent,
                    remoteIp, remotePort,
                    httpPath);
                    logger?.Logging(LogLevel.Information, tmp, executedMethod, caller, CustomLogEvents.HttpResponse);
                }
                else
                {
                    string tmp = string.Format("Flow(Trade-Id: {1})= --> HTTP-Reqest-Pre-Execution: {2}, {3} [{4}:{5}], Uri={6}, Caller: {7}",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFF"), traceId, httpMethod, userAgent,
                        remoteIp,
                        remotePort,
                        httpPath, caller ?? "N/A");
                    logger?.Logging(LogLevel.Information, tmp, executedMethod, caller, CustomLogEvents.HttpRequest);
                }
            }
            catch (Exception ex)
            {

            }
            return logger;
        }
        public static ILogger Logging(this ILogger logger, LogLevel logLevel, string msg, MethodBase executedMethod, string caller, int eventId = CustomLogEvents.GeneralInfo)
        {

            logger?.Log(logLevel, eventId, "{1}",
                msg);

            //logger?.Log(logLevel, eventId, null, (s, e) => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFF") + ":\t "+s.ToString());
            return logger;
        }

        public static async void RegisterNetClasses(ISingletonNodeDatabaseHandler databaseHandler, string[] databaseEntiyNamespaces)
        {

            string query = null;

            string namespaceValConcat = null;
            for (int i = 0; i < databaseEntiyNamespaces.Length; i++)
            {
                namespaceValConcat += "'" + databaseEntiyNamespaces[i] + "'";
                if (i != databaseEntiyNamespaces.Length - 1)
                {
                    namespaceValConcat += ",";
                }
            }
            QueryResponseData<ClassModel> c = null;

            List<Type> classesFromNameSpace = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && databaseEntiyNamespaces.ToList().IndexOf(t.Namespace) != -1).ToList();
            if (classesFromNameSpace.Count == 0)
            {
                throw new NotSupportedException("no classes found in namespace '" + namespaceValConcat + "'");
            }

            Func<Type, List<ClassModel>, Task<bool>> fAddClassToDb = new Func<Type, List<ClassModel>, Task<bool>>(async (item, c) =>
            {
                string assemblyName = item.Assembly.FullName ?? "null";
                bool found = c == null ?
                false :
                c.Find(x => x.Assembly == assemblyName && x.Namespace == item.Namespace && x.NetName == item.Name) != null;

                if (!found && item.Name.EndsWith("Model"))
                {

                    ClassModel classModel = new ClassModel { Assembly = assemblyName, Namespace = item.Namespace, NetName = item.Name };
                    classModel.TableName = classModel.fGenerateTableNameFromNetClassName(item.Name);

                    query = "INSERT INTO `class` " +
                                "(`uuid`," +
                                "`assembly`," +
                                "`namespace`," +
                                "`net_name`," +
                                "`table_name`) " +
                                " VALUES " +
                                "(UUID()," +
                                "@assembly," +
                                "@namespace," +
                                "@net_name," +
                                "@table_name);";
                    QueryResponseData<ClassModel> insertClassModelResponse = await databaseHandler.ExecuteQueryWithMap<ClassModel>(query, classModel);


                }
                return true;
            });
            query = "SELECT * FROM class WHERE namespace in (" + namespaceValConcat + ");";
            c = await databaseHandler.ExecuteQuery<ClassModel>(query);
            if (classesFromNameSpace.Count != 0)
            {
                if (c.HasData)
                {
                    classesFromNameSpace.ForEach(item => fAddClassToDb(item, c.DataStorage));
                }
                else
                {
                    classesFromNameSpace.ForEach(item => fAddClassToDb(item, null));
                }
            }
            if (c.HasData)//prüfen ob datenbankeinträge auch wirkliche c# class im entsprechenden namespace sind, wenn nicht ist die klasse nicht mehr vorhanden und wird aus der db gelöscht
            {
                foreach (var item in c.DataStorage)
                {
                    bool found = classesFromNameSpace.Find(x => item.Assembly == (x.Assembly.FullName ?? "null") && x.Namespace == item.Namespace && x.Name == item.NetName) != null;
                    if (!found)
                    {

                        ClassModel classModel = new ClassModel { Uuid = item.Uuid };

                        query = "DELETE FROM class WHERE uuid = '@uuid'";
                        QueryResponseData<ClassModel> deleteClassModelResponse = await databaseHandler.ExecuteQueryWithMap<ClassModel>(query, classModel);
                    }
                }
            }

            query = "SELECT * FROM class WHERE namespace in (" + namespaceValConcat + ");";
            c = await databaseHandler.ExecuteQuery<ClassModel>(query);
            if (c.HasData)
            {
                foreach (ClassModel classModel in c.DataStorage)
                {
                    var constraints = await databaseHandler.ExecuteQuery<ClassRelationModel>("select INFORMATION_SCHEMA.COLUMNS.COLUMN_KEY as `COLUMN_KEY`, INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME, INFORMATION_SCHEMA.COLUMNS.TABLE_NAME,CONSTRAINT_NAME,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS join INFORMATION_SCHEMA.KEY_COLUMN_USAGE on INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME=INFORMATION_SCHEMA.KEY_COLUMN_USAGE.COLUMN_NAME and INFORMATION_SCHEMA.COLUMNS.TABLE_NAME=INFORMATION_SCHEMA.KEY_COLUMN_USAGE.TABLE_NAME where INFORMATION_SCHEMA.KEY_COLUMN_USAGE.TABLE_NAME='" + classModel.TableName + "' and referenced_table_name is not null;");
                    if (constraints.HasData)
                    {
                        foreach (DataRow row in constraints.Data.Rows)
                        {
                            bool oneToOne = row["COLUMN_KEY"].ToString() == "PRI";
                            bool oneToMany = row["COLUMN_KEY"].ToString() == "UNI";

                            Guid relationType = oneToOne ? new Guid("04b690c3-5050-11ec-9b15-d8bbc10f2ae0") : new Guid("093cf365-5050-11ec-9b15-d8bbc10f2ae0");

                            string refTable = row["REFERENCED_TABLE_NAME"].ToString();
                            ClassModel two = c.DataStorage.Find(x => x.TableName == refTable);

                            if (two == null)
                            {

                                System.Diagnostics.Debug.WriteLine("warning: classmodel '" + refTable + "' cant found in namespace: " + namespaceValConcat + "");
                                continue;
                            }
                            List<ClassRelationModel> executionList = new List<ClassRelationModel>();
                            //high to low level model
                            executionList.Add(new ClassRelationModel { EntityOne = classModel.TableName, Direction = "-->", EntityTwo = two.TableName, EntityOneClassUuid = classModel.Uuid, EntityTwoClassUuid = two.Uuid, EntityOneKeyCol = row["COLUMN_NAME"].ToString(), EntityTwoKeyCol = row["REFERENCED_COLUMN_NAME"].ToString(), EntityOneRelationshipTypeUuid = relationType, EntityTwoRelationshipTypeUuid = relationType });
                            //higher level model
                            //executionList.Add(new ClassRelationModel { EntityOne = two.TableName, Direction = "<--", EntityTwo = classModel.TableName, EntityOneClassUuid = two.Uuid, EntityTwoClassUuid = classModel.Uuid, EntityOneKeyCol = row["REFERENCED_COLUMN_NAME"].ToString(), EntityTwoKeyCol = row["COLUMN_NAME"].ToString(), EntityOneRelationshipTypeUuid = relationType, EntityTwoRelationshipTypeUuid = relationType });

                            foreach (var classRelationModel in executionList)
                            {
                                query = "SELECT * FROM class_relation WHERE entity_one = '" + classRelationModel.EntityOne + "' and entity_one_key_col = '" + classRelationModel.EntityOneKeyCol + "' and entity_two = '" + classRelationModel.EntityTwo + "' and entity_two_key_col = '" + classRelationModel.EntityTwoKeyCol + "';";
                                var relationExists = await databaseHandler.ExecuteQuery<ClassRelationModel>(query);
                                if (!relationExists.HasData)
                                {
                                    query = "INSERT INTO `class_relation` " +
                                            "(`uuid`," +
                                            "`direction`," +
                                            "`entity_one_class_uuid`," +
                                            "`entity_one`," +
                                            "`entity_one_key_col`," +
                                            "`entity_one_relationship_type_uuid`," +
                                            "`entity_two_relationship_type_uuid`," +
                                            "`entity_two`," +
                                            "`entity_two_class_uuid`," +
                                            "`entity_two_key_col`)" +
                                            " VALUES " +
                                            "(UUID()," +
                                            "@direction," +
                                            "@entity_one_class_uuid," +
                                            "@entity_one," +
                                            "@entity_one_key_col," +
                                            "@entity_one_relationship_type_uuid," +
                                            "@entity_two_relationship_type_uuid," +
                                            "@entity_two," +
                                            "@entity_two_class_uuid," +
                                    "@entity_two_key_col);";

                                    QueryResponseData<ClassRelationModel> insertRelationResponse = await databaseHandler.ExecuteQueryWithMap<ClassRelationModel>(query, classRelationModel);
                                }
                            }
                        }
                    }

                    string forecastControllerName = classModel.NetName.Replace("Model", "");
                    query = "SELECT * FROM controller WHERE name = '" + forecastControllerName.ToLower() + "';";
                    QueryResponseData<ControllerModel> queryResponseDataController = await databaseHandler.ExecuteQuery<ControllerModel>(query);
                    if (queryResponseDataController.HasData)
                    {
                        query = "SELECT * FROM controller_relation_to_class WHERE controller_uuid = '" + queryResponseDataController.FirstRow.Uuid + "' AND class_uuid = '" + classModel.Uuid + "';";
                        QueryResponseData<ControllerModel> queryResponseDataControllerRelToClass = await databaseHandler.ExecuteQuery<ControllerModel>(query);
                        if (!queryResponseDataControllerRelToClass.HasData)
                        {
                            ControllerRelationToClass controllerRelationToClass = new ControllerRelationToClass();
                            controllerRelationToClass.ControllerUuid = queryResponseDataController.FirstRow.Uuid;
                            controllerRelationToClass.ClassUuid = classModel.Uuid;
                            query = "INSERT INTO controller_relation_to_class (uuid,controller_uuid,class_uuid) VALUES (UUID(),@controller_uuid,@class_uuid);";
                            QueryResponseData<ControllerRelationToClass> insertRelationResponse = await databaseHandler.ExecuteQueryWithMap<ControllerRelationToClass>(query, controllerRelationToClass);
                        }
                    }
                    else
                    {

                    }
                }
            }
            var data = await databaseHandler.ExecuteQuery<ClassRelationToClassViewModel>("SELECT * FROM class_relation_to_class_view WHERE namespace in (" + namespaceValConcat + ");");
            if (data.HasData)
            {
                foreach (var item in data.DataStorage)
                {
                    var d = c.DataStorage.Find(x => x.TableName == item.TableName);

                    Type nettype = classesFromNameSpace.Find(x => x.Name == d.NetName);
                    if (!SQLDefinitionProperties.BackendTablesEx.ContainsKey(item.TableName))
                    {

                        /*if (classesFromNameSpace.Find(x => x.Name==netModelName) == null)
                        {
                            throw new Exception("relation '"+item.EntityOne+"<->"+item.EntityTwo+"' is not a member in namespace '"+ BackendAPIDefinitionsProperties.DatabaseModelNamespace + "', class not found");
                        }*/
                        if (nettype != null)
                            SQLDefinitionProperties.BackendTablesEx.Add(item.TableName, new Application.Model.Database.MySQL.Data.ClassModelWrapper(d, nettype));
                    }

                    if (item.EntityOne != null)
                    {
                        SQLDefinitionProperties.BackendTablesEx[item.TableName].Relations.Add(item);
                    }
                    else
                    {

                    }
                }
            }
            var allRel = SQLDefinitionProperties.BackendTablesEx.ToList().Select(x => x.Value).ToList().SelectMany(x => x.Relations).ToList();
            foreach (string key in SQLDefinitionProperties.BackendTablesEx.Keys)
            {
                var tmp = allRel.FindAll(x => x.EntityOne == key || x.EntityTwo == key);
                tmp.ForEach(x =>
                {
                    if (SQLDefinitionProperties.BackendTablesEx[key].Relations.Find(y => y.ToString() == x.ToString()) == null)
                    {
                        SQLDefinitionProperties.BackendTablesEx[key].Relations.Add(x);
                    }
                });
            }
        }

        public static async void RegisterBackend(this IEndpointRouteBuilder endpoints, INodeManagerHandler nodeManagerHandler, IServiceProvider services, IWebHostEnvironment env, ISingletonNodeDatabaseHandler databaseHandler, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IConfiguration configuration, string[] databaseEntiyNamespace, Dictionary<string, IHubService> hubServiceRoutes = null)
        {
            ApiModule apiModule = new ApiModule(databaseHandler, nodeManagerHandler);
            apiModule.ResetRegister();

            string query = null;
            List<ControllerActionDescriptor> controllers = actionDescriptorCollectionProvider.
            ActionDescriptors.
            Items.
            OfType<ControllerActionDescriptor>().
            ToList();
            List<Type> workItems = new List<Type>();
            RoleModel roleModelRoot = new RoleModel();
            RoleModel roleModelAnonymous = new RoleModel();



            var appSettingsVarInstallBehaviourEnvVar = configuration.GetValue<bool>("InstallWithDefaultConfiguration");
            var appSettingsVarRegisterEndpointBehaviourEnvVar = configuration.GetValue<bool>("RegisterEndpointMode");
            if (appSettingsVarInstallBehaviourEnvVar)
            {

                //Für initiales füllen

                AbstractModel abstractModel = new AbstractModel();

                Func<QueryResponseData<CommunicationMediumModel>, Task<bool>> funcSelectOrInsertCm = new Func<QueryResponseData<CommunicationMediumModel>, Task<bool>>(async (cMExists) =>
                {
                    foreach (string key in BackendAPIDefinitionsProperties.CommunicationMedium.Keys)
                    {
                        CommunicationMediumModel cMModel = new CommunicationMediumModel();

                        Guid currentStorageCmId = BackendAPIDefinitionsProperties.CommunicationMedium[key];
                        CommunicationMediumModel tmp = cMExists.DataStorage.Find(x => x.Name == key);
                        bool found = tmp != null;
                        if (!found)
                        {
                            cMModel.Name = key;
                            cMModel.Active = true;
                            cMModel.Deleted = false;
                            string query = cMModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                            cMExists = await databaseHandler.ExecuteQueryWithMap<CommunicationMediumModel>(query, cMModel);
                            if (cMExists.HasErrors)
                                throw new Exception();

                            if (cMExists.HasData)
                                BackendAPIDefinitionsProperties.CommunicationMedium[key] = (Guid)cMExists.LastInsertedId;
                        }
                        else if (currentStorageCmId == Guid.Empty)
                        {
                            BackendAPIDefinitionsProperties.CommunicationMedium[key] = tmp.Uuid;
                        }
                    }
                    return true;
                });

                query = "SELECT * FROM communication_medium;";
                var cMExists = await databaseHandler.ExecuteQuery<CommunicationMediumModel>(query);
                bool response = await funcSelectOrInsertCm(cMExists);

                roleModelRoot.Name = BackendAPIDefinitionsProperties.RootRoleName;
                roleModelRoot.Active = true;
                query = roleModelRoot.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleModelRoot).ToString();
                var rootRoleExists = await databaseHandler.ExecuteQueryWithMap<RoleModel>(query, roleModelRoot);
                if (!rootRoleExists.HasData)
                {
                    roleModelRoot.Description = "Root Role";
                    query = roleModelRoot.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    rootRoleExists = await databaseHandler.ExecuteQueryWithMap<RoleModel>(query, roleModelRoot);
                }
                roleModelRoot = rootRoleExists.FirstRow;

                roleModelAnonymous.Name = BackendAPIDefinitionsProperties.AnonymousRoleName;
                roleModelAnonymous.Active = true;
                query = roleModelAnonymous.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleModelAnonymous).ToString();
                var rootAnonExists = await databaseHandler.ExecuteQueryWithMap<RoleModel>(query, roleModelAnonymous);
                if (!rootAnonExists.HasData)
                {
                    roleModelAnonymous.Description = "Root Role";
                    query = roleModelAnonymous.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    rootAnonExists = await databaseHandler.ExecuteQueryWithMap<RoleModel>(query, roleModelAnonymous);
                }
                roleModelAnonymous = rootAnonExists.FirstRow;

                UserTypeModel rootUserType = new UserTypeModel();
                rootUserType.Name = BackendAPIDefinitionsProperties.RootRoleName;
                rootUserType.Active = true;
                query = rootUserType.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, rootUserType).ToString();
                var rootUserTypeExists = await databaseHandler.ExecuteQueryWithMap<UserTypeModel>(query, rootUserType);
                if (!rootUserTypeExists.HasData)
                {
                    query = rootUserType.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    rootUserTypeExists = await databaseHandler.ExecuteQueryWithMap<UserTypeModel>(query, rootUserType);
                }
                rootUserType = rootUserTypeExists.FirstRow;


                AccountModel rootAccount = new AccountModel();
                rootAccount.User = BackendAPIDefinitionsProperties.RootRoleName;
                rootAccount.Active = true;
                rootAccount.CommunicationMediumUuid = BackendAPIDefinitionsProperties.CommunicationMedium[BackendAPIDefinitionsProperties.CmKeyLocalhost];//localhost communication_medium
                query = rootAccount.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, rootAccount).ToString();
                var rootAcceExists = await databaseHandler.ExecuteQueryWithMap<AccountModel>(query, rootAccount);
                if (!rootAcceExists.HasData)
                {
                    query = rootAccount.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    rootAcceExists = await databaseHandler.ExecuteQueryWithMap<AccountModel>(query, rootAccount);
                }
                rootAccount = rootAcceExists.FirstRow;

                UserModel rootUser = new UserModel();
                rootUser.User = BackendAPIDefinitionsProperties.RootName;
                rootUser.Active = true;
                query = rootUser.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, rootUser).ToString();
                var rootExists = await databaseHandler.ExecuteQueryWithMap<UserModel>(query, rootUser);
                if (!rootExists.HasData)
                {
                    rootUser.Password = BackendAPIDefinitionsProperties.RootUserPassword;
                    rootUser.AccountUuid = rootAccount.Uuid;//acount uuid
                    rootUser.UserTypeUuid = rootUserType.Uuid;//user type uuid
                    query = rootUser.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    rootExists = await databaseHandler.ExecuteQueryWithMap<UserModel>(query, rootUser);
                }
                rootUser = rootExists.FirstRow;

                UserRelationToRoleModel userRelationRoleModel = new UserRelationToRoleModel();
                userRelationRoleModel.RoleUuid = roleModelRoot.Uuid;//role uuid
                userRelationRoleModel.UserUuid = rootUser.Uuid;//user uuid
                userRelationRoleModel.Active = true;
                query = userRelationRoleModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, userRelationRoleModel).ToString();
                var userRelationRoleModelExists = await databaseHandler.ExecuteQueryWithMap<UserRelationToRoleModel>(query, userRelationRoleModel);
                if (!userRelationRoleModelExists.HasData)
                {
                    query = userRelationRoleModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    userRelationRoleModelExists = await databaseHandler.ExecuteQueryWithMap<UserRelationToRoleModel>(query, userRelationRoleModel);
                }
                userRelationRoleModel = userRelationRoleModelExists.FirstRow;

            }
            else
            {
                roleModelRoot.Name = BackendAPIDefinitionsProperties.RootRoleName;
                roleModelRoot.Active = true;
                query = roleModelRoot.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleModelRoot).ToString();
                var rootRoleExists = await databaseHandler.ExecuteQueryWithMap<RoleModel>(query, roleModelRoot);
                roleModelRoot = rootRoleExists.FirstRow;

                roleModelAnonymous.Name = BackendAPIDefinitionsProperties.AnonymousRoleName;
                roleModelAnonymous.Active = true;
                query = roleModelAnonymous.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleModelAnonymous).ToString();
                var rootAnonExists = await databaseHandler.ExecuteQueryWithMap<RoleModel>(query, roleModelAnonymous);
                roleModelAnonymous = rootAnonExists.FirstRow;
            }

            //1.controller actions in datenbank
            //2.actions zu rollen
            //3.bei request schauen ob user in role und action von controller einer role zugewiesen ist
            //4. default role ist Admin, welche beim start wenn sie nicht existiert angelegt wird (datenbank) und jeder controller+actions crud


            var httpMethodQueryResponseData = await databaseHandler.ExecuteQuery<HttpMethodModel>("SELECT * FROM http_method;");
            if (!httpMethodQueryResponseData.HasData)
                throw new Exception();

            foreach (ControllerActionDescriptor controlller in controllers)
            {
                string ns = controlller.ControllerTypeInfo.Namespace;
                Type baseType = controlller.ControllerTypeInfo.BaseType;
                Type controllerType = controlller.ControllerTypeInfo.UnderlyingSystemType;
                Type[] type = baseType.GenericTypeArguments;

                if (!ns.ToLower().Contains("ocelot") && controlller.ControllerTypeInfo.Name.EndsWith("Controller") && !workItems.Contains(controllerType))
                {
                    ConstructorInfo[] ctorInfo = controllerType.GetConstructors();
                    if (ctorInfo.Length == 1)
                    {
                        ParameterInfo[] ctorParams = ctorInfo[0].GetParameters();
                        object[] ctorP = new object[ctorParams.Length];
                        for (int i = 0; i < ctorParams.Length; i++)
                        {
                            var ctorParamTypeFromDI = services.GetService(ctorParams[i].ParameterType);
                            if (ctorParamTypeFromDI != null)
                            {

                                ctorP[i] = ctorParamTypeFromDI;
                            }

                        }
                        var repository = Activator.CreateInstance(controllerType, ctorP);
                        CustomControllerBase customControllerBase = (CustomControllerBase)repository;
                        if (controllerType.BaseType != null)
                        {
                            if (controllerType.BaseType.GenericTypeArguments.Length != 0)
                            {
                                foreach (var item in controllerType.BaseType.GenericTypeArguments)
                                {
                                    if (databaseEntiyNamespace.ToList().IndexOf(item.Namespace) != -1)
                                    {
                                        ConstructorInfo[] ctorInfo2 = item.GetConstructors();
                                        ParameterInfo[] ctorParams2 = ctorInfo2[0].GetParameters();
                                        AbstractModel abstractModel1 = (AbstractModel)Activator.CreateInstance(item, ctorParams2);
                                        string tablename = abstractModel1.fGenerateTableNameFromNetClassName(item.Name);
                                        if (SQLDefinitionProperties.BackendTablesEx.ContainsKey(tablename))
                                        {
                                            SQLDefinitionProperties.BackendTablesEx[tablename].SetController(controlller);
                                        }

                                    }
                                }
                            }
                        }
                        var controllerEndpoints = controllers.FindAll(x => x.ControllerName.ToLower() == controlller.ControllerName.ToLower());

                        if (appSettingsVarRegisterEndpointBehaviourEnvVar)
                            apiModule.RegisterApi(customControllerBase, controllerEndpoints, httpMethodQueryResponseData.DataStorage, roleModelRoot, roleModelAnonymous);

                        workItems.Add(controllerType);
                    }
                }




            }
            if (hubServiceRoutes != null)
            {
                foreach (var hubRoute in hubServiceRoutes)
                {
                    apiModule.RegisterHub(hubRoute.Value, roleModelRoot, roleModelAnonymous);
                }
            }

            if (appSettingsVarRegisterEndpointBehaviourEnvVar)
            {
                Console.WriteLine("!!! Restart the application to fill the memory relations of classes and controllers !!!");
            }

            List<ApiModel> registeredApis = await apiModule.GetApis();
            endpoints.MapControllers().RequireCors("api-gateway");
            AppManager.InitRegisterApi(registeredApis);
            IRabbitMqHandler rabbitMqHandler = services.GetService<IRabbitMqHandler>();
            rabbitMqHandler.PublishObject("route-management", nodeManagerHandler.NodeModel, "register-notification", "node registered: " + nodeManagerHandler.NodeModel.Name + "", null, null);

        }
    }
}
