using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySQL.Helix;

namespace WebApiFunction.Application.Controller.Modules.Helix
{
    public class MessageSendingTypeModule : AbstractBackendModule<MessageSendingTypeModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public MessageSendingTypeModule(ISingletonDatabaseHandler databaseHandler, ICachingHandler cache, IMysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods

        #endregion
    }
}
