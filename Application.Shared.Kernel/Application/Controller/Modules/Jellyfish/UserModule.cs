using System;
using Dapper;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;

namespace Application.Shared.Kernel.Application.Controller.Modules.Jellyfish
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
        public async Task<bool> UpdateProfilePicture(Guid userUuid, byte[] image)
        {
            var res = await MysqlDapperContext.GetConnection().ExecuteAsync("UPDATE user SET picture = @blobdata WHERE uuid = @uuid;", new { blobdata = image, uuid = userUuid });
            return res == 1;
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
        public async Task<Guid> AcceptFriendshipRequest(Guid requestUuid,Guid fromUserUuid, Guid toUserUuid)
        {
            var guid = CreateUuid();
            var resDelete = await MysqlDapperContext.GetConnection().ExecuteAsync("DELETE FROM user_friendship_request WHERE uuid = @requestuuid limit 1;", new { requestuuid = requestUuid });
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
