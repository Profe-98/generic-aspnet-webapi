using System;
using Dapper;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;

namespace Application.Shared.Kernel.Application.Controller.Modules.Jellyfish
{
    public class ChatModule : AbstractBackendModule<ChatModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public ChatModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods
        public async Task<IEnumerable<ChatModel>> GetAllChats()
        {
            return await MysqlDapperContext.GetConnection().QueryAsync<ChatModel>("SELECT * FROM chat;");
        }
        public async Task<ChatModel> GetChat(Guid chatUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<ChatModel>("SELECT * FROM chat WHERE uuid = @chatUuid;", chatUuid);
            if (res == null)
                return null;
            return res.First();
        }
        public async Task<List<UserModel>> GetChatMembers(Guid chatUuid)
        {
            var res = await MysqlDapperContext.GetConnection().QueryAsync<UserModel>("SELECT u.* FROM chat_relation_to_user as crtu inner join user as u on(crtu.user_uuid = u.uuid) inner join chat as c on(c.uuid = crtu.chat_uuid) WHERE crtu.chat_uuid = @chatUuid;", new { chatUuid  = chatUuid });
            if (res == null)
                return null;
            return res.ToList();
        }
        public async Task<bool> UpdateChatPicture(Guid chatUuid, byte[] image)
        {
            var res = await MysqlDapperContext.GetConnection().ExecuteAsync("UPDATE chat SET picture = @blobdata WHERE uuid = @uuid;", new { blobdata = image, uuid = chatUuid });
            return res == 1;
        }
        #endregion
    }
}
