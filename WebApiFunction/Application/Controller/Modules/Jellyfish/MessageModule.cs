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
using WebApiFunction.Application.Model.DataTransferObject.Jellyfish;

namespace WebApiFunction.Application.Controller.Modules.Jellyfish
{
    public class MessageModule : AbstractBackendModule<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public MessageModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods
        public async Task<IEnumerable<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>> GetAllMessages()
        {
            return await MysqlDapperContext.GetConnection().QueryAsync<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>("SELECT * FROM message;");
        }
        public async Task<List<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>> GetAllChatMessages(Guid chatUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>("SELECT * FROM message WHERE chat_uuid = @chatUuid;", chatUuid);
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<List<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>> GetAllChatNotReceivedMessages(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.MessageModel>("SELECT m.* FROM message as m inner join chat_relation_to_user as crtu on(crtu.chat_uuid = m.chat_uuid) left join message_acknowledge as ma on(ma.message_uuid = m.uuid) where crtu.user_uuid = @userUuid and ma.uuid is null;", new { userUuid = userUuid });
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<List<MessageAcknowledgeModel>> GetAcknowledgedMessages(Guid userUuid, Guid[] messageUuids)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<MessageAcknowledgeModel>("SELECT * FROM message_acknowledge WHERE message_uuid IN @messageUuid and user_uuid = @userUuid;", new { messageUuid = messageUuids, userUuid = userUuid });
            return res.ToList();
        }
        public async Task<List<MessageAcknowledgeModel>> AcknowledgeMessages(Guid userUuid,List<MessageAcknowledgeDTO> acknowledgeMessages)
        {
            acknowledgeMessages.ForEach(async(x) => {
                var rowsAffected = await MysqlDapperContext.GetConnection().ExecuteAsync("INSERT INTO message_acknowledge (`message_uuid`,`user_uuid`,`uuid`) VALUES (@messageUuid,@userUuid,@uuid);", new { messageUuid = x.MessageUuid, userUuid = userUuid,uuid=CreateUuid() });
                
            });

            var msgIdsArr = acknowledgeMessages.GroupBy((x) => x.MessageUuid).ToList().Select(x => x.Key).ToArray();

            var insertedAcks = await GetAcknowledgedMessages(userUuid,msgIdsArr);
            return insertedAcks != null? insertedAcks.ToList():null;  
        }
        #endregion
    }
}
