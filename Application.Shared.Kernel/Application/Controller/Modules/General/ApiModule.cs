using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService;
using Application.Shared.Kernel.Web.AspNet.Filter;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;
using Microsoft.AspNetCore.Authorization;

namespace Application.Shared.Kernel.Application.Controller.Modules
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
            public List<AuthorizeAttribute> AuthorizeAttributes;

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
        public async void RegisterHub(IHubService hubService) 
        {
            string query = null;
            SignalrHubModel hubModel = new SignalrHubModel();
            hubModel.Route = hubService.RouteAttribute.Route;
            hubModel.Active = true;


            query = hubModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, hubModel).ToString();
            QueryResponseData<SignalrHubModel> queryResponseDataHub = await Db.ExecuteQueryWithMap<SignalrHubModel>(query, hubModel);

            hubModel.Name = hubService.GetType().Name.ToLower();
            hubModel.NodeUuid = _nodeManagerHandler.NodeModel.Uuid;
            hubModel.IsRegistered = true;
            if (!queryResponseDataHub.HasData)
            {
                query = hubModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                queryResponseDataHub = await Db.ExecuteQueryWithMap<SignalrHubModel>(query, hubModel);
            }
            else
            {
                hubModel.IsRegistered = true;
                query = hubModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE,
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
                query = methodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, methodModel).ToString();
                QueryResponseData<SignalrHubMethodModel> queryResponseDataHubMethod = await Db.ExecuteQueryWithMap<SignalrHubMethodModel>(query, methodModel);

                if (!queryResponseDataHubMethod.HasData)
                {

                    query = methodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    queryResponseDataHubMethod = await Db.ExecuteQueryWithMap<SignalrHubMethodModel>(query, methodModel);
                }
                else
                {
                    query = methodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, methodModel).ToString();
                    await Db.ExecuteQueryWithMap<SignalrHubMethodModel>(query, methodModel);
                }
                methodModel = queryResponseDataHubMethod.FirstRow;



                RoleRelationToSignalrHubMethodModel roleRelationToSignalrHubMethodModel = new RoleRelationToSignalrHubMethodModel();
                roleRelationToSignalrHubMethodModel.RoleUuid = rootModel.Uuid;
                roleRelationToSignalrHubMethodModel.SignalrHubMethodUuid = methodModel.Uuid;
                query = roleRelationToSignalrHubMethodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, roleRelationToSignalrHubMethodModel).ToString();
                QueryResponseData<RoleRelationToSignalrHubMethodModel> queryResponseDataRoleRelationToHubMethod = await Db.ExecuteQueryWithMap<RoleRelationToSignalrHubMethodModel>(query, roleRelationToSignalrHubMethodModel);

                if (!queryResponseDataRoleRelationToHubMethod.HasData)
                {

                    query = roleRelationToSignalrHubMethodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                    queryResponseDataRoleRelationToHubMethod = await Db.ExecuteQueryWithMap<RoleRelationToSignalrHubMethodModel>(query, roleRelationToSignalrHubMethodModel);
                }
                else
                {
                    query = roleRelationToSignalrHubMethodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, roleRelationToSignalrHubMethodModel).ToString();
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

                        query = signalrHubMethodArgumentsModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, signalrHubMethodArgumentsModel).ToString();
                        QueryResponseData<SignalrHubMethodArgumentsModel> queryResponseDataHubMethodArgs = await Db.ExecuteQueryWithMap<SignalrHubMethodArgumentsModel>(query, signalrHubMethodArgumentsModel);

                        signalrHubMethodArgumentsModel.NetType = param.ParameterType.ToString();
                        if (!queryResponseDataHubMethodArgs.HasData)//no method args in database 
                        {

                            query = signalrHubMethodArgumentsModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                            queryResponseDataHubMethodArgs = await Db.ExecuteQueryWithMap<SignalrHubMethodArgumentsModel>(query, signalrHubMethodArgumentsModel);
                        }
                        else
                        {
                            if (queryResponseDataHubMethodArgs.FirstRow.NetType.ToString() != signalrHubMethodArgumentsModel.NetType)//net type changed in method meta
                            {

                                query = signalrHubMethodArgumentsModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, methodModel).ToString();
                                await Db.ExecuteQueryWithMap<SignalrHubMethodArgumentsModel>(query, signalrHubMethodArgumentsModel);
                            }

                        }
                        signalrHubMethodArgumentsModel = queryResponseDataHubMethodArgs.FirstRow;
                    }
                }
            }


        }
        public async void RegisterApi(IAuthorizationPolicyProvider authorizationPolicyProvider, CustomControllerBase controller, List<ControllerActionDescriptor> controllerDesc, List<HttpMethodModel> httpMethodModels)

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
            string query = apiModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, apiModel).ToString();
            QueryResponseData<ApiModel> queryResponseDataApi = await Db.ExecuteQueryWithMap<ApiModel>(query, apiModel);
            if (!queryResponseDataApi.HasData)
            {
                apiModel.RouterPattern = "{area}/{controller}/{action}/{id?}";
                apiModel.AreaPattern = BackendAPIDefinitionsProperties.AreaWildcard;
                query = apiModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                queryResponseDataApi = await Db.ExecuteQueryWithMap<ApiModel>(query, apiModel);
            }

            ControllerModel controllerModel = new ControllerModel();
            controllerModel.Name = controllerName;
            controllerModel.ApiUuid = queryResponseDataApi.FirstRow.Uuid;//api_uuid
            controllerModel.Active = true;
            controllerModel.NodeUuid = _nodeManagerHandler.NodeModel.Uuid;


            query = controllerModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerModel).ToString();
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
                query = controllerModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
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
                query = controllerModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, new ControllerModel { ApiUuid = queryResponseDataApi.FirstRow.Uuid, Name = controllerName, Active = true }).ToString();
                await Db.ExecuteQueryWithMap<ControllerModel>(query, controllerModel);
            }




            foreach (ControllerActionDescriptor c in controllerDesc)
            {

                string actionName = c.ActionName.ToLower();
                List<string> httpMethod = new List<string>();
                if (c.AttributeRouteInfo != null && c.AttributeRouteInfo.Template != null)
                {
                    var methods = controller.GetType().GetMethods();
                    var authAttFromController = controller.GetType().GetCustomAttributes<AuthorizeAttribute>();
                    List<AuthorizeAttribute> authAttr = new List<AuthorizeAttribute>();
                    foreach (MethodInfo method in methods)
                    {
                        var methodHttpAttr = method.GetCustomAttributes<HttpMethodAttribute>();
                        var methodConsumesAttr = method.GetCustomAttributes<CustomConsumesFilter>();
                        var methodProducesAttr = method.GetCustomAttributes<CustomProducesFilter>();
                        authAttr = authAttFromController.ToList();

                        var authAttFromMethod = method.GetCustomAttributes<AuthorizeAttribute>();
                        if (authAttFromMethod.Count() > 0)
                        {
                            authAttFromMethod.ToList().ForEach(x => authAttr.Add(x));
                        }


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
                            controllerActions[currentIterationLevelControllerName][actionName].Add(new ControllerActionDescriptorExt { HttpMethod = x, Route = routeData.ToLower() + (routeData.ToLower().EndsWith('}') ? string.Empty : "/"), AuthorizeAttributes = authAttr });
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
                    query = controllerActionModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerActionModel).ToString();
                    QueryResponseData<ControllerActionModel> queryResponseDataControllerAction = await Db.ExecuteQueryWithMap<ControllerActionModel>(query, controllerActionModel);
                    if (!queryResponseDataControllerAction.HasData)
                    {
                        query = controllerActionModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                        queryResponseDataControllerAction = await Db.ExecuteQueryWithMap<ControllerActionModel>(query, controllerActionModel);
                    }

                    //1.
                    /*
                     
                            foreach (var item in data.AuthorizeAttributes)
                            {
                                if (!String.IsNullOrEmpty(item.Policy))
                                {

                                    var policy = await authorizationPolicyProvider.GetPolicyAsync(item.Policy);

                                }
                            }
                     */
                    //2.
                    //3.

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
                        query = relationToHttpMethodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, relationToHttpMethodModel).ToString();
                        QueryResponseData<ControllerActionRelationToHttpMethodModel> queryResponseDataControllerActionRelToHttp = await Db.ExecuteQueryWithMap<ControllerActionRelationToHttpMethodModel>(query, relationToHttpMethodModel);
                        if (!queryResponseDataControllerActionRelToHttp.HasData)
                        {
                            query = relationToHttpMethodModel.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                            queryResponseDataControllerActionRelToHttp = await Db.ExecuteQueryWithMap<ControllerActionRelationToHttpMethodModel>(query, relationToHttpMethodModel);



                            RoleRelationToControllerActionRelationToHttpMethodModel controllerActionRelationToHttpMethod = new RoleRelationToControllerActionRelationToHttpMethodModel();
                            controllerActionRelationToHttpMethod.RoleUuid = queryResponseDataController.FirstRow.IsAuthcontroller || queryResponseDataController.FirstRow.IsErrorController ? anonymous.Uuid : rootRole.Uuid;//role
                            controllerActionRelationToHttpMethod.ControllerActionRelationToHttpMethodUuid = queryResponseDataControllerActionRelToHttp.FirstRow.Uuid;//http rel w/ controller action
                            controllerActionRelationToHttpMethod.Active = true;
                            query = controllerActionRelationToHttpMethod.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, controllerActionRelationToHttpMethod).ToString();
                            QueryResponseData<RoleRelationToControllerActionRelationToHttpMethodModel> queryResponseDataRoleToContActHttp = await Db.ExecuteQueryWithMap<RoleRelationToControllerActionRelationToHttpMethodModel>(query, controllerActionRelationToHttpMethod);
                            if (!queryResponseDataRoleToContActHttp.HasData)
                            {
                                query = controllerActionRelationToHttpMethod.GenerateQuery(MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT).ToString();
                                queryResponseDataRoleToContActHttp = await Db.ExecuteQueryWithMap<RoleRelationToControllerActionRelationToHttpMethodModel>(query, controllerActionRelationToHttpMethod);
                            }
                        }
                    }
                }
            }
            else
                throw new Exception();






        }

        #endregion
    }
}
