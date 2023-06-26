using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
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
using WebApiFunction.Web.Websocket.SignalR.HubService;
using InfluxDB.Client.Api.Domain;
using Google.Protobuf.Reflection;
using WebApiFunction.Application.Model.Database.MySQL.Table;
using WebApiFunction.Application.Model.Database.MySQL.View;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Web.AspNet.Filter;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiFunction.Application.Controller.Modules
{
    public class ApiModule
    {
        #region Private
        private readonly ISingletonNodeDatabaseHandler Db = null;
        private readonly INodeManagerHandler _nodeManagerHandler = null;
        #endregion
        #region Public 
        #endregion
        #region Ctor
        public ApiModule(ISingletonNodeDatabaseHandler databaseHandler, INodeManagerHandler nodeManagerHandler)
        {
            Db = databaseHandler;
            _nodeManagerHandler = nodeManagerHandler;
        }
        #endregion
        #region Methods
        public async void ResetRegister()
        {
            QueryResponseData response = await Db.ExecuteQuery<ControllerModel>("UPDATE controller SET is_registered = @is_registered;", new ControllerModel { IsRegistered = false });
            if (response.HasErrors)
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "Cant reset controller register");
            }
        }

        public class ControllerActionDescriptorExt
        {
            public string Route;
            public string HttpMethod;

            public string UniqueIdentifiers
            {
                get
                {
                    return HttpMethod + ":" + Route;
                }
            }

            public static string GenerateUniqueIdentifier(string method, string route)
            {
                return method + ":" + route;
            }

            public override string ToString()
            {
                return UniqueIdentifiers;
            }
        }
        public async void RegisterHub(IHubService hubService, RoleModel rootModel, RoleModel anonymousRole) 
        {
            string query = null;
            SignalrHubModel hubModel = new SignalrHubModel();
            hubModel.Route = hubService.RouteAttribute.Route;
            hubModel.Active = true;


            query = hubModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, hubModel).ToString();
            QueryResponseData<SignalrHubModel> queryResponseDataHub = await Db.ExecuteQueryWithMap<SignalrHubModel>(query, hubModel);

            hubModel.Name = hubService.GetType().Name.ToLower();
            hubModel.NodeUuid = _nodeManagerHandler.NodeModel.Uuid;
            hubModel.IsRegistered = true;
            if (!queryResponseDataHub.HasData)
            {
                query = hubModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                queryResponseDataHub = await Db.ExecuteQueryWithMap<SignalrHubModel>(query, hubModel);
            }
            else
            {
                hubModel.IsRegistered = true;
                query = hubModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE,
                    new SignalrHubModel
                    {
                        Route = hubService.RouteAttribute.Route,
                        Active = true
                    }
                    ).ToString();
                await Db.ExecuteQueryWithMap<SignalrHubModel>(query, hubModel);
            }
            hubModel = queryResponseDataHub.FirstRow;
            foreach (var method in hubService.HubMethods)
            {
                //1. Register von Hubs+Methoden+Methoden Args fertigstellen
                //2.1. Authorization via Claim für SignalR Hubs+Methods
                //2.2. MySQL View mit Hub Routes+Methods+Authorization
                //2.3. MySQL View fuer Hubs mit Controller Route View Result Union
                //3. Routes ApiGateway providen / kann Ocelot Websocket durchschleifen?

                SignalrHubMethodModel methodModel = new SignalrHubMethodModel();
                methodModel.Name = method.Name.ToLower();
                methodModel.ClassLocation = method.DeclaringType.FullName;
                string accessModifiyer = null;
                if (method.IsPublic)
                {
                    accessModifiyer = "public";
                }
                else if (method.IsPrivate)
                {
                    accessModifiyer = "private";
                }
                methodModel.AccessModifiyer = accessModifiyer;

                methodModel.SignalrHubUuid = hubModel.Uuid;
                query = methodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, methodModel).ToString();
                QueryResponseData<SignalrHubMethodModel> queryResponseDataHubMethod = await Db.ExecuteQueryWithMap<SignalrHubMethodModel>(query, methodModel);

                if (!queryResponseDataHubMethod.HasData)
                {

                    query = methodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    queryResponseDataHubMethod = await Db.ExecuteQueryWithMap<SignalrHubMethodModel>(query, methodModel);
                }
                else
                {
                    query = methodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, methodModel).ToString();
                    await Db.ExecuteQueryWithMap<SignalrHubMethodModel>(query, methodModel);
                }
                methodModel = queryResponseDataHubMethod.FirstRow;

                RoleRelationToSignalrHubMethodModel roleRelationToSignalrHubMethodModel = new RoleRelationToSignalrHubMethodModel();
                roleRelationToSignalrHubMethodModel.RoleUuid = rootModel.Uuid;
                roleRelationToSignalrHubMethodModel.SignalrHubMethodUuid = methodModel.Uuid;
                query = roleRelationToSignalrHubMethodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleRelationToSignalrHubMethodModel).ToString();
                QueryResponseData<RoleRelationToSignalrHubMethodModel> queryResponseDataRoleRelationToHubMethod = await Db.ExecuteQueryWithMap<RoleRelationToSignalrHubMethodModel>(query, roleRelationToSignalrHubMethodModel);

                if (!queryResponseDataRoleRelationToHubMethod.HasData)
                {

                    query = roleRelationToSignalrHubMethodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    queryResponseDataRoleRelationToHubMethod = await Db.ExecuteQueryWithMap<RoleRelationToSignalrHubMethodModel>(query, roleRelationToSignalrHubMethodModel);
                }
                else
                {
                    query = roleRelationToSignalrHubMethodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, roleRelationToSignalrHubMethodModel).ToString();
                    await Db.ExecuteQueryWithMap<RoleRelationToSignalrHubMethodModel>(query, roleRelationToSignalrHubMethodModel);
                }

                var methodParams = method.GetParameters();
                if (methodParams.Length > 0)
                {
                    foreach (var param in methodParams)
                    {
                        SignalrHubMethodArgumentsModel signalrHubMethodArgumentsModel = new SignalrHubMethodArgumentsModel();
                        signalrHubMethodArgumentsModel.SignalrHubMethodUuid = methodModel.Uuid;
                        signalrHubMethodArgumentsModel.Name = param.Name;

                        query = signalrHubMethodArgumentsModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, signalrHubMethodArgumentsModel).ToString();
                        QueryResponseData<SignalrHubMethodArgumentsModel> queryResponseDataHubMethodArgs = await Db.ExecuteQueryWithMap<SignalrHubMethodArgumentsModel>(query, signalrHubMethodArgumentsModel);

                        signalrHubMethodArgumentsModel.NetType = param.ParameterType.ToString();
                        if (!queryResponseDataHubMethodArgs.HasData)//no method args in database 
                        {

                            query = signalrHubMethodArgumentsModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                            queryResponseDataHubMethodArgs = await Db.ExecuteQueryWithMap<SignalrHubMethodArgumentsModel>(query, signalrHubMethodArgumentsModel);
                        }
                        else
                        {
                            if (queryResponseDataHubMethodArgs.FirstRow.NetType.ToString() != signalrHubMethodArgumentsModel.NetType)//net type changed in method meta
                            {

                                query = signalrHubMethodArgumentsModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, methodModel).ToString();
                                await Db.ExecuteQueryWithMap<SignalrHubMethodArgumentsModel>(query, signalrHubMethodArgumentsModel);
                            }

                        }
                        signalrHubMethodArgumentsModel = queryResponseDataHubMethodArgs.FirstRow;
                    }
                }
            }


        }
        public async void RegisterApi(CustomControllerBase controller, List<ControllerActionDescriptor> controllerDesc, List<HttpMethodModel> httpMethodModels, RoleModel rootRole, RoleModel anonymous)

        {
            List<ControllerModel> responseValues = new List<ControllerModel>();
            string controllerName = controller.ControllerName.ToLower();
            AreaAttribute area = controller.GetArea();
            string areaName = area.RouteValue.ToLower();
            Dictionary<string, Dictionary<string, List<ControllerActionDescriptorExt>>> controllerActions = new Dictionary<string, Dictionary<string, List<ControllerActionDescriptorExt>>>();


            List<RouteAttribute> routeAttributes = controller.GetRoutes();
            ApiModel apiModel = new ApiModel();
            apiModel.Name = areaName.ToLower();
            apiModel.Active = true;
            string query = apiModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, apiModel).ToString();
            QueryResponseData<ApiModel> queryResponseDataApi = await Db.ExecuteQueryWithMap<ApiModel>(query, apiModel);
            if (!queryResponseDataApi.HasData)
            {
                apiModel.RouterPattern = "{area}/{controller}/{action}/{id?}";
                apiModel.AreaPattern = BackendAPIDefinitionsProperties.AreaWildcard;
                query = apiModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                queryResponseDataApi = await Db.ExecuteQueryWithMap<ApiModel>(query, apiModel);
            }

            ControllerModel controllerModel = new ControllerModel();
            controllerModel.Name = controllerName;
            controllerModel.ApiUuid = queryResponseDataApi.FirstRow.Uuid;//api_uuid
            controllerModel.Active = true;
            controllerModel.NodeUuid = _nodeManagerHandler.NodeModel.Uuid;


            query = controllerModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerModel).ToString();
            QueryResponseData<ControllerModel> queryResponseDataController = await Db.ExecuteQueryWithMap<ControllerModel>(query, controllerModel);
            if (!queryResponseDataController.HasData)
            {
                if (controllerModel.Name.ToLower().Equals(BackendAPIDefinitionsProperties.AuthentificationControllerName))
                {
                    controllerModel.IsAuthcontroller = true;
                }
                else if (controllerModel.Name.ToLower().Equals(BackendAPIDefinitionsProperties.ErrorControllerName))
                {
                    controllerModel.IsErrorController = true;
                }
                controllerModel.IsRegistered = true;
                query = controllerModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                queryResponseDataController = await Db.ExecuteQueryWithMap<ControllerModel>(query, controllerModel);
            }
            else
            {
                if (controllerModel.Name.ToLower().Equals(BackendAPIDefinitionsProperties.AuthentificationControllerName))
                {
                    controllerModel.IsAuthcontroller = true;
                }
                else if (controllerModel.Name.ToLower().Equals(BackendAPIDefinitionsProperties.ErrorControllerName))
                {
                    controllerModel.IsErrorController = true;
                }
                controllerModel.IsRegistered = true;
                query = controllerModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, new ControllerModel { ApiUuid = queryResponseDataApi.FirstRow.Uuid, Name = controllerName, Active = true }).ToString();
                await Db.ExecuteQueryWithMap<ControllerModel>(query, controllerModel);
            }




            foreach (ControllerActionDescriptor c in controllerDesc)
            {

                string actionName = c.ActionName.ToLower();
                List<string> httpMethod = new List<string>();
                if (c.AttributeRouteInfo != null && c.AttributeRouteInfo.Template != null)
                {
                    var methods = controller.GetType().GetMethods();
                    foreach (MethodInfo method in methods)
                    {
                        var methodHttpAttr = method.GetCustomAttributes<HttpMethodAttribute>();
                        var methodConsumesAttr = method.GetCustomAttributes<CustomConsumesFilter>();
                        var methodProducesAttr = method.GetCustomAttributes<CustomProducesFilter>();
                        if (methodHttpAttr.Count() != 0 && actionName.ToLower().Equals(method.Name.ToLower()))
                        {
                            methodHttpAttr.ToList().ForEach(x => x.HttpMethods.ToList().ForEach(y =>
                            {
                                if (!httpMethod.Contains(y))
                                    httpMethod.Add(y);
                            }));
                        }
                    }
                    string currentIterationLevelControllerName = c.ControllerName.ToLower();
                    string routeData = c.AttributeRouteInfo.Template;
                    if (!controllerActions.ContainsKey(currentIterationLevelControllerName))
                    {
                        controllerActions.Add(currentIterationLevelControllerName, new Dictionary<string, List<ControllerActionDescriptorExt>>());
                    }
                    if (!controllerActions[currentIterationLevelControllerName].ContainsKey(actionName))
                    {
                        controllerActions[currentIterationLevelControllerName].Add(actionName, new List<ControllerActionDescriptorExt>());
                    }
                    httpMethod.ForEach(x =>
                    {
                        if (controllerActions[currentIterationLevelControllerName][actionName].Find(y => y.HttpMethod == x && y.Route == routeData) == null)
                            controllerActions[currentIterationLevelControllerName][actionName].Add(new ControllerActionDescriptorExt { HttpMethod = x, Route = routeData.ToLower() + (routeData.ToLower().EndsWith('}') ? string.Empty : "/") });
                    }

                    );
                }
            }
            if (controllerActions.ContainsKey(controllerName))
            {

                foreach (string action in controllerActions[controllerName].Keys)
                {
                    ControllerActionModel controllerActionModel = new ControllerActionModel();
                    controllerActionModel.ControllerUuid = queryResponseDataController.FirstRow.Uuid; //controller uuid
                    controllerActionModel.Name = action;
                    controllerActionModel.Active = true;
                    query = controllerActionModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerActionModel).ToString();
                    QueryResponseData<ControllerActionModel> queryResponseDataControllerAction = await Db.ExecuteQueryWithMap<ControllerActionModel>(query, controllerActionModel);
                    if (!queryResponseDataControllerAction.HasData)
                    {
                        query = controllerActionModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                        queryResponseDataControllerAction = await Db.ExecuteQueryWithMap<ControllerActionModel>(query, controllerActionModel);
                    }
                    foreach (ControllerActionDescriptorExt data in controllerActions[controllerName][action])
                    {
                        HttpMethodModel httpMethodModel = httpMethodModels.Find(x => x.Name.ToLower().Equals(data.HttpMethod.ToLower()));
                        if (httpMethodModel == null)
                            throw new Exception();

                        ControllerActionRelationToHttpMethodModel relationToHttpMethodModel = new ControllerActionRelationToHttpMethodModel();
                        relationToHttpMethodModel.ActionRoute = data.Route;
                        relationToHttpMethodModel.ControllerActionUuid = queryResponseDataControllerAction.FirstRow.Uuid;//controlleraction
                        relationToHttpMethodModel.HttpMethodUuid = httpMethodModel.Uuid;//http method
                        relationToHttpMethodModel.Active = true;
                        query = relationToHttpMethodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, relationToHttpMethodModel).ToString();
                        QueryResponseData<ControllerActionRelationToHttpMethodModel> queryResponseDataControllerActionRelToHttp = await Db.ExecuteQueryWithMap<ControllerActionRelationToHttpMethodModel>(query, relationToHttpMethodModel);
                        if (!queryResponseDataControllerActionRelToHttp.HasData)
                        {
                            query = relationToHttpMethodModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                            queryResponseDataControllerActionRelToHttp = await Db.ExecuteQueryWithMap<ControllerActionRelationToHttpMethodModel>(query, relationToHttpMethodModel);

                            RoleRelationToControllerActionRelationToHttpMethodModel controllerActionRelationToHttpMethod = new RoleRelationToControllerActionRelationToHttpMethodModel();
                            controllerActionRelationToHttpMethod.RoleUuid = queryResponseDataController.FirstRow.IsAuthcontroller || queryResponseDataController.FirstRow.IsErrorController ? anonymous.Uuid : rootRole.Uuid;//role
                            controllerActionRelationToHttpMethod.ControllerActionRelationToHttpMethodUuid = queryResponseDataControllerActionRelToHttp.FirstRow.Uuid;//http rel w/ controller action
                            controllerActionRelationToHttpMethod.Active = true;
                            query = controllerActionRelationToHttpMethod.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerActionRelationToHttpMethod).ToString();
                            QueryResponseData<RoleRelationToControllerActionRelationToHttpMethodModel> queryResponseDataRoleToContActHttp = await Db.ExecuteQueryWithMap<RoleRelationToControllerActionRelationToHttpMethodModel>(query, controllerActionRelationToHttpMethod);
                            if (!queryResponseDataRoleToContActHttp.HasData)
                            {
                                query = controllerActionRelationToHttpMethod.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                                queryResponseDataRoleToContActHttp = await Db.ExecuteQueryWithMap<RoleRelationToControllerActionRelationToHttpMethodModel>(query, controllerActionRelationToHttpMethod);
                            }
                        }
                    }
                }
            }
            else
                throw new Exception();






        }

        public async Task<List<ApiModel>> GetApis()
        {
            List<ApiModel> responseValue = new List<ApiModel>();
            QueryResponseData<ApiModel> data = await Db.ExecuteQuery<ApiModel>("SELECT * FROM api;");
            if (data.HasStorageData)
            {
                responseValue = data.DataStorage;
                for (int i = 0; i < responseValue.Count; i++)
                {
                    ApiModel apiModel = responseValue[i];
                    apiModel.AvaibleControllers = new List<ControllerModel>();
                    ControllerModel controllerModel = new ControllerModel();
                    controllerModel.ApiUuid = apiModel.Uuid;
                    controllerModel.Active = true;
                    string query = controllerModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerModel).ToString();
                    QueryResponseData<ControllerModel> queryResponseDataController = await Db.ExecuteQueryWithMap<ControllerModel>(query, controllerModel);
                    if (queryResponseDataController.HasData)
                    {
                        apiModel.AvaibleControllers = queryResponseDataController.DataStorage;
                        for (int f = 0; f < apiModel.AvaibleControllers.Count; f++)
                        {
                            ControllerModel controller = apiModel.AvaibleControllers[f];
                            RoleToControllerViewModel roleToControllerViewModel = new RoleToControllerViewModel();
                            roleToControllerViewModel.ControllerUuid = controller.Uuid;
                            query = roleToControllerViewModel.GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleToControllerViewModel).ToString();
                            QueryResponseData<RoleToControllerViewModel> queryResponseDataControllerRoles = await Db.ExecuteQueryWithMap<RoleToControllerViewModel>(query, roleToControllerViewModel);
                            if (queryResponseDataControllerRoles.HasData)
                            {
                                controller.Roles = queryResponseDataControllerRoles.DataStorage;
                            }
                            apiModel.AvaibleControllers[f] = controller;
                        }
                    }
                    responseValue[i] = apiModel;
                }
            }
            return responseValue;
        }
        #endregion
    }
}
