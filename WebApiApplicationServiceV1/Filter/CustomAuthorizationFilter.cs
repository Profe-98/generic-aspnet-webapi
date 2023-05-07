using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Net;
using WebApiApplicationService.Models;
using WebApiApplicationService;
using Microsoft.AspNetCore.Hosting;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Modules;
using Microsoft.Extensions.Configuration;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.InternalModels
{
    /*[AttributeUsage(AttributeTargets.Class| AttributeTargets.Method)]
    public class AuthorizationFilter : TypeFilterAttribute
    {
        public readonly List<CrudRoleDescriptor> CrudRoleDescriptors;

        public bool HasStaticRole
        {
            get
            {
                return CrudRoleDescriptors != null&&CrudRoleDescriptors.Count != 0; 
            }
        }

        [Flags]
        public enum CRUD : short
        {
            Undefined = 0,
            Create = 1,
            Read = 2,
            Update = 4,
            Delete = 8,
        }

        public struct CrudUuid
        {
            private static readonly Array _crudEnumValues;
            //Guid Werte aus DB
            public static readonly Guid Create = new Guid("d22a1e89-1937-11ec-a4ca-d8bbc10f2ae0");
            public static readonly Guid Read = new Guid("d722ee4b-1937-11ec-a4ca-d8bbc10f2ae0");
            public static readonly Guid Update = new Guid("db440aef-1937-11ec-a4ca-d8bbc10f2ae0");
            public static readonly Guid Delete = new Guid("dee44a8b-1937-11ec-a4ca-d8bbc10f2ae0");

            private static readonly Dictionary<Guid, CRUD> _cruds;

            static CrudUuid()
            {
                _cruds = new Dictionary<Guid, CRUD>() { { Create, CRUD.Create },{ Read,CRUD.Read },{ Update, CRUD.Update },{ Delete, CRUD.Delete } };

                _crudEnumValues = Enum.GetValues(typeof(CRUD));
            }

            public CRUD GetCrud(string crud)
            {
                CRUD response = CRUD.Undefined;
                if(!String.IsNullOrEmpty(crud))
                {
                    for (int i = 0; i < _crudEnumValues.Length; i++)
                    {
                        CRUD tmp = (CRUD)_crudEnumValues.GetValue(i);
                        if (tmp.ToString().ToLower().Equals(crud.ToLower()))
                            return tmp;
                    }
                }
                return response;
            }
            public CRUD GetCrud(Guid uuid)
            {
                return _cruds.ContainsKey(uuid)?_cruds[uuid]:CRUD.Undefined;
            }
        }

        public class CrudRoleDescriptor
        {
            public readonly string Role;
            public readonly CRUD Permissions;
            public CrudRoleDescriptor(string role, CRUD crudValues = CRUD.Undefined)//bsp. Admin, crudValues = CRUD.Create | CRUD.Update
            {
                Role = role;
                Permissions = crudValues;
            }
        }

        public struct RoleDesc
        {
            public string Role;
            public CRUD Permissions;

        }

        private AuthorizationFilter(RoleDesc[] crudRoleDescriptors,bool jsonPropertyCheck, Type type = null) : base(type == null?typeof(CustomAuthorizationFilter): type)
        {
            CrudRoleDescriptors = new List<CrudRoleDescriptor>();
            crudRoleDescriptors.ToList().ForEach(x => CrudRoleDescriptors.Add(new CrudRoleDescriptor(x.Role, x.Permissions)));
            if(crudRoleDescriptors.Length > 0)
            {

            }
            this.Arguments = new object[2] { CrudRoleDescriptors, jsonPropertyCheck };
            this.IsReusable = false;
        }

        /// <summary>
        /// Wenn User für Route berechtigt (hat Role für entsprechende ControllerAction in DB)
        /// </summary>
        /// <param name="role"></param>
        public AuthorizationFilter() : this(new RoleDesc[0], false, null)
        {

        }

        /// <summary>
        /// Descript the access rights for Property in Modelclass, e.g. when a User try to set a json node like "admin":true, normaly he could not set it because he is a standard user, to protect this property 
        /// you can restrict the access of it with this Attribute
        /// </summary>
        /// <param name="role"></param>
        /// <param name="jsonPropertyCheck"></param>
        public AuthorizationFilter(string role,bool jsonPropertyCheck) : this(new RoleDesc[0], jsonPropertyCheck, null)
        {

        }
        /// <summary>
        /// Wenn User in Rolle + Route + Controller + HTTP-Methode, dann okay
        /// </summary>
        /// <param name="role"></param>
        public AuthorizationFilter(string role) : this(new RoleDesc[0], false, null)
        {

        }

        /// <summary>
        /// Wenn User in Rolle + Route + Controller + HTTP-Methode & die CRUD der Rolle enthalten sind, okay
        /// </summary>
        /// <param name="role"></param>
        /// <param name="perms"></param>
        public AuthorizationFilter(string role, CRUD perms) : this(new RoleDesc[1] { new RoleDesc { Role = role, Permissions = perms } }, false, null)
        {

        }
        /// <summary>
        /// Wenn User in Rollen + Routen + Controller + HTTP-Methoden & die CRUDs der Rollen enthalten sind, okay
        /// </summary>
        /// <param name="role"></param>
        /// <param name="perms"></param>
        public AuthorizationFilter(string[] roles,CRUD[] perms) : this(new RoleDesc[0], false, null)
        {
            bool equal = roles.Length == perms.Length;
            if (!equal && roles.Length != 0)
                throw new NotSupportedException();

            for (int i = 0; i < roles.Length; i++)
            {
                CrudRoleDescriptors.Add(new CrudRoleDescriptor(roles[i], perms[i]));
            }
            this.Arguments = new object[1] { CrudRoleDescriptors };
            this.IsReusable = false;
        }
    }
    public class CustomAuthorizationFilter : CustomControllerBase, IAuthorizationFilter
    {
        private readonly ILogger<CustomAuthorizationFilter> _logger;
        private readonly ICachingHandler _cache;
        private readonly IAuthHandler _authHandler;
        public readonly List<AuthorizationFilter.CrudRoleDescriptor> Roles = null;
        public readonly bool JsonPropertyCheck = false;
        
        public CustomAuthorizationFilter(IWebHostEnvironment env,IAuthHandler authHandler, ICachingHandler cache,IConfiguration configuration, ILogger<CustomAuthorizationFilter> logger, List<AuthorizationFilter.CrudRoleDescriptor> roles,bool jsonPropertyCheck) : base (env,cache,configuration)
        {
            _authHandler = authHandler;
            _cache = cache;
            _logger = logger;
            Roles = roles;
            JsonPropertyCheck = jsonPropertyCheck;
        }
        //Bevor Action ausgeführt wird, pre-request-execution um Content-Type zu prüfen
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            bool auth = false;
            var filter = context.Filters.OfType<CustomAuthorizationFilter>().Where(x => x.Roles != null && x.Roles.Count > 0).ToList();
            ControllerActionDescriptor controllerActionDescriptor = ((ControllerActionDescriptor)context.ActionDescriptor);
            string actionName = controllerActionDescriptor.ActionName;
            string routeTemplate = context.ActionDescriptor.AttributeRouteInfo?.Template;
            string controllerName = controllerActionDescriptor.ControllerName.ToLower();
            string httpMethod = context.HttpContext.Request.Method;
            string requestUri = context.HttpContext.Request.Path.Value;

            string[] routeTemplateSplit = routeTemplate.Split(new string[] { "/"},StringSplitOptions.None);
            string apiPartFromRouteTemplate = routeTemplateSplit[0];

            string jwt = context.HttpContext.GetRequestJWTFromHeader();
            if (!JsonPropertyCheck)
            {

                ApiModel api = AppManager.Api.Find(x => x.Name == apiPartFromRouteTemplate);
                ControllerModel controller = api.AvaibleControllers.Find(x => x.Name == controllerName);
                List<RoleToControllerViewModel> roles = controller.Roles.FindAll(x => x.RouteSegments.Length == routeTemplateSplit.Length);
                var findFilter = filter.Find(x => x.Roles.Find(x => x.Role == BackendAPIDefinitionsProperties.AnonymousRoleName) != null);
                if (findFilter == null)
                {
                    if (!String.IsNullOrEmpty(jwt))
                    {
                        AuthHandler.CheckLoginResponse check = await _authHandler.CheckLogin(jwt, true);
                        if (check.IsCorrectToken)
                        {
                            if (!check.TokenExpired)
                            {
                                if (check.IsCorrectUserAgent)
                                {
                                    if (check.IsCorrectIp)
                                    {
                                        if (Roles.Count != 0)
                                        {
                                            bool found = false;
                                            foreach (AuthorizationFilter.CrudRoleDescriptor crudRoleDescriptor in Roles)
                                            {
                                                string roleC = crudRoleDescriptor.Role.ToLower();
                                                var fM = roles.Find(x => x.Role.ToLower().Equals(roleC) && crudRoleDescriptor.Permissions.HasFlag((AuthorizationFilter.CRUD)(x.Flag)) && x.HttpMethod == httpMethod && x.Controller == controllerName);
                                                found = fM != null;
                                                if (found)
                                                {
                                                    auth = true;
                                                    break;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            auth = check.IsAuthorizedForPath;
                                        }

                                    }
                                }
                            }
                            if (!auth)
                            {
                                bool logoutResponse = await _authHandler.Logout(jwt);
                            }
                        }
                    }


                }
                else
                {
                    auth = true;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(jwt))
                {
                    var check = await _authHandler.GetSession(jwt);
                    
                    foreach (AuthorizationFilter.CrudRoleDescriptor crudRoleDescriptor in Roles)
                    {
                        string roleC = crudRoleDescriptor.Role.ToLower();
                        var fM = check.UserModel.AvaibleRoles.Find(x => x.Name.ToLower() == roleC);
                        bool found = fM != null;
                        if (found)
                        {
                            auth = true;
                            break;
                        }
                    }
                }
            }
            if(!auth)
            {
                var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN,
                 Detail = BackendAPIDefinitionsProperties.HttpRequestNotAuthorized
                }
                }, System.Net.HttpStatusCode.Forbidden, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestNotAuthorized, null);
                context.Result = response;
            }

        }
    }*/
}
