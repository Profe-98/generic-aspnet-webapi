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
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish.DataTransferObject;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public async Task<UserModel> GetUser(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<UserModel>("SELECT * FROM user WHERE uuid = @uuid;", new {uuid= userUuid });
            if (res == null)
                return null;
            return res.First();
        }
        public async Task<List<UserFriendshipUserModelDTO>> GetUserOpenFriendshipRequests(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<UserFriendshipUserModelDTO>("SELECT u.signalr_connection_id,u.user,u.first_name,u.last_name,ufr.* FROM user_friendship_request as ufr inner join user as u on(u.uuid = ufr.user_uuid) WHERE ufr.target_user_uuid = @uuid;", new { uuid = userUuid });
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<List<UserModel>> GetUserFriends(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<UserModel>("SELECT u.* FROM user_friends as uf inner join user as u on(u.uuid = uf.friend_user_uuid) WHERE uf.user_uuid = @uuid or uf.friend_user_uuid = @uuid;", new { uuid = userUuid });
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<List<UserModel>> SearchUser(string searchStr)
        {

            var res = await MysqlDapperContext.GetConnection().QueryAsync<UserModel>("SELECT * FROM user WHERE LOWER(user) LIKE @searchStr;", new { searchStr = "%"+ searchStr+"%".ToLower() });
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<Guid> CreateFriendshipRequest(Guid fromUserUuid, Guid toUserUuid, string message)
        {
            var guid = CreateUuid();
            var rowsAffected = await MysqlDapperContext.GetConnection().ExecuteAsync("INSERT INTO user_friendship_request (`uuid`,`user_uuid`,`target_user_uuid`,`target_user_request_message`) VALUES (@guid,@fromUserUuid,@toUserUuid,@message);", new { guid = guid, fromUserUuid = fromUserUuid, toUserUuid = toUserUuid, message = message });
            return rowsAffected == 1 ? guid : Guid.Empty;
        }
        public async Task<Guid> AcceptFriendshipRequest(Guid fromUserUuid, Guid toUserUuid)
        {
            var guid = CreateUuid();
            var rowsAffected = await MysqlDapperContext.GetConnection().ExecuteAsync("INSERT INTO user_friends (`uuid`,`user_uuid`,`friend_user_uuid`) VALUES (@guid,@fromUserUuid,@toUserUuid);", new { guid = guid, fromUserUuid = fromUserUuid, toUserUuid = toUserUuid });
            return rowsAffected == 1 ? guid : Guid.Empty;
        }
        public async Task<bool> SetSignalR(Guid userUuid, string signalRConnectionId)
        {

            var rowsAffected = (!String.IsNullOrEmpty(signalRConnectionId) ?
                (await MysqlDapperContext.GetConnection().ExecuteAsync("UPDATE user SET signalr_connection_id = @signalRConnectionId WHERE uuid = @userUuid limit 1;", new { signalRConnectionId = signalRConnectionId, userUuid = userUuid })) : (await MysqlDapperContext.GetConnection().ExecuteAsync("UPDATE user SET signalr_connection_id = null WHERE uuid = @userUuid limit 1;", new { userUuid = userUuid })));
            return rowsAffected == 1;
        }
        #endregion
    }
}
