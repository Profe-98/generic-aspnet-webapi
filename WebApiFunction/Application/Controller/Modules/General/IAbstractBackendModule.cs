﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Dapper.Context;
using WebApiFunction.Database;

namespace WebApiFunction.Application.Controller.Modules
{
    public interface IAbstractBackendModule<T>
        where T : AbstractModel
    {
        ISingletonDatabaseHandler Db { get; }
        IMysqlDapperContext MysqlDapperContext { get; }

        void Commit(DbTransaction transaction);
        Task<DbTransaction> CreateTransaction();
        Task<QueryResponseData> Delete(T model, DbTransaction transaction = null);
        void Dispose();
        Task<QueryResponseData> Insert(T model, DbTransaction transaction = null);
        void Rollback(DbTransaction transaction);
        Task<QueryResponseData<T>> Select(T model, T whereClauseModel = null);
        Task<QueryResponseData> Update(T modelToChange, T customWhereClauseObjectInstance, DbTransaction transaction = null);
        Guid CreateUuid();

    }
}
