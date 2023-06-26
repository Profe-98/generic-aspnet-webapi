using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySQL;
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
using WebApiFunction.Web.AspNet.Controller;
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
using WebApiFunction.Application.Model.Database.MySQL.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using Dapper;

namespace WebApiFunction.Application.Controller.Modules.Jellyfish
{
    public class UserModule : AbstractBackendModule<UserModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public UserModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods
        public async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            return await MysqlDapperContext.GetConnection().QueryAsync<UserModel>("SELECT * FROM user;");
        }
        public async Task<string> GetUser(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<string>("SELECT signalr_connection_id FROM user WHERE uuid = @uuid;", new {uuid= userUuid });
            if (res == null)
                return null;
            return res.First();
        }
        public async Task<List<UserModel>> GetUserOpenFriendshipRequests(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<UserModel>("SELECT u.* FROM user_friendship_request as ufr inner join user as u on(u.uuid = ufr.user_uuid) WHERE ufr.target_user_uuid = @uuid;", new { uuid = userUuid });
            if (res == null)
                return null;
            return res.ToList();
        }
        #endregion
    }
}
