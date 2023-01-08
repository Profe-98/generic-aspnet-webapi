using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using WebApiFunction.Application.Model.Database.MySql.Table;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Database;

namespace WebApiFunction.Application.Controller.Modules
{
    public class MessageRelationToAccountModule : CustomBackendModule<MessageRelationToAccountModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public MessageRelationToAccountModule(IScopedDatabaseHandler databaseHandler, ICachingHandler cache, WebApiFunction.Application.Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods

        #endregion
    }
}
