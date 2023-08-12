using Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Database;

namespace Application.Shared.Kernel.Application.Controller.Modules
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

        #endregion
    }
}
