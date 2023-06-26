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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
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
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiFunction.Web.AspNet.Filter
{
    public class AuthorizationFilterC : CustomControllerBase, IAuthorizationFilter
    {
        private readonly ILogger<AuthorizationFilterC> _logger;
        private readonly IAuthHandler _authHandler;
        public readonly bool JsonPropertyCheck = false;

        public AuthorizationFilterC(IWebHostEnvironment env, IAuthHandler authHandler, IConfiguration configuration, ILogger<AuthorizationFilterC> logger)
        {
            _authHandler = authHandler;
            _logger = logger;
        }
        //Bevor Action ausgeführt wird, pre-request-execution um Content-Type zu prüfen
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationFilter : TypeFilterAttribute
    {
        public readonly List<CrudRoleDescriptor> CrudRoleDescriptors;

        public bool HasStaticRole
        {
            get
            {
                return CrudRoleDescriptors != null && CrudRoleDescriptors.Count != 0;
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
                _cruds = new Dictionary<Guid, CRUD>() { { Create, CRUD.Create }, { Read, CRUD.Read }, { Update, CRUD.Update }, { Delete, CRUD.Delete } };

                _crudEnumValues = Enum.GetValues(typeof(CRUD));
            }

            public CRUD GetCrud(string crud)
            {
                CRUD response = CRUD.Undefined;
                if (!string.IsNullOrEmpty(crud))
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
                return _cruds.ContainsKey(uuid) ? _cruds[uuid] : CRUD.Undefined;
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

        private AuthorizationFilter(RoleDesc[] crudRoleDescriptors, bool jsonPropertyCheck, Type type = null) : base(type == null ? typeof(AuthorizationFilterC) : type)
        {
            CrudRoleDescriptors = new List<CrudRoleDescriptor>();
            crudRoleDescriptors.ToList().ForEach(x => CrudRoleDescriptors.Add(new CrudRoleDescriptor(x.Role, x.Permissions)));
            if (crudRoleDescriptors.Length > 0)
            {

            }
            Arguments = new object[2] { CrudRoleDescriptors, jsonPropertyCheck };
            IsReusable = false;
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
        public AuthorizationFilter(string role, bool jsonPropertyCheck) : this(new RoleDesc[0], jsonPropertyCheck, null)
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
        public AuthorizationFilter(string[] roles, CRUD[] perms) : this(new RoleDesc[0], false, null)
        {
            bool equal = roles.Length == perms.Length;
            if (!equal && roles.Length != 0)
                throw new NotSupportedException();

            for (int i = 0; i < roles.Length; i++)
            {
                CrudRoleDescriptors.Add(new CrudRoleDescriptor(roles[i], perms[i]));
            }
            Arguments = new object[1] { CrudRoleDescriptors };
            IsReusable = false;
        }
    }

}
