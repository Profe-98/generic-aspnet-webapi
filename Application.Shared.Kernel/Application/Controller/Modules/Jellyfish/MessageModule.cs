using Dapper;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;
using Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;

namespace Application.Shared.Kernel.Application.Controller.Modules.Jellyfish
{
    public class MessageModule : AbstractBackendModule<MessageModel>
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
        public async Task<IEnumerable<MessageModel>> GetAllMessages()
        {
            return await MysqlDapperContext.GetConnection().QueryAsync<MessageModel>("SELECT * FROM message;");
        }
        public async Task<List<MessageModel>> GetAllChatMessages(Guid chatUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<MessageModel>("SELECT * FROM message WHERE chat_uuid = @chatUuid;", chatUuid);
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<List<MessageModel>> GetAllChatNotReceivedMessages(Guid userUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<MessageModel>("SELECT m.* FROM message as m inner join chat_relation_to_user as crtu on(crtu.chat_uuid = m.chat_uuid) left join message_acknowledge as ma on(ma.message_uuid = m.uuid) where crtu.user_uuid = @userUuid and ma.uuid is null;", new { userUuid = userUuid });
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
