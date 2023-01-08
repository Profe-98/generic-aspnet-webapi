﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySql.Table;

namespace WebApiFunction.Application.Controller.Modules
{
    public class MessageModule : CustomBackendModule<MessageModel>
    {
        #region Private
        #endregion
        #region Public

        #endregion
        #region Ctor & Dtor
        public MessageModule(IScopedDatabaseHandler databaseHandler, ICachingHandler cache, WebApiFunction.Application.Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext) : base(databaseHandler, cache, mysqlDapperContext)
        {

        }
        #endregion
        #region Methods

        #endregion
    }
}
