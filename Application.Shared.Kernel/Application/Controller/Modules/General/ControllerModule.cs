using Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Application.Controller.Modules
{
    public class ControllerModule : AbstractBackendModule<ControllerModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public ControllerModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods
        #endregion
    }
}
