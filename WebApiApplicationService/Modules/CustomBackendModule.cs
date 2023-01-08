using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Common;
using WebApiApplicationService.Models.Database;

namespace WebApiApplicationService.Modules
{
    public class CustomBackendModule<T> : IDisposable
        where T : AbstractModel
    {
        #region Private
        private readonly ICachingHandler _cachingHandler;
        private readonly IScopedDatabaseHandler _db = null;
        #endregion
        public IScopedDatabaseHandler Db
        {
            get
            {
                return _db;
            }
        }
        #region Ctor
        public CustomBackendModule(IScopedDatabaseHandler databaseHandler,ICachingHandler cachingHandler)
        {
            _cachingHandler = cachingHandler;
            _db = databaseHandler;
            string t = typeof(T).Name;
        }
        ~CustomBackendModule()
        {
            this.Dispose();
        }
        #endregion
        #region Methods
        private async Task<QueryResponseData> SqlOp(T model,SQLDefinitionProperties.SQL_STATEMENT_ART stmnt,DbTransaction transaction = null)
        {
            string query = model.GenerateQuery(stmnt,null,model).ToString();
            QueryResponseData dt = await Db.ExecuteQuery<T>(query, model,transaction: transaction);
            return dt;
        }

        private async Task<QueryResponseData<T>> SqlOp(T model, SQLDefinitionProperties.SQL_STATEMENT_ART stmnt, T whereClauseValueObject, DbTransaction transaction = null)
        {
            string query = model.GenerateQuery(stmnt,whereClauseValueObject,model).ToString();
            QueryResponseData<T> dt = await Db.ExecuteQueryWithMap<T>(query, model, transaction: transaction);
            return dt;
        }

        public async Task<DbTransaction> CreateTransaction()
        {
            return await Db.CreateTransaction();
        }
        public void Commit(DbTransaction transaction)
        {
            Db.Commit(transaction);
        }
        public void Rollback(DbTransaction transaction)
        {
            Db.Rollback(transaction);
        }

        public async Task<QueryResponseData> Insert(T model,DbTransaction transaction = null)
        {
            QueryResponseData response = null;
            response = await SqlOp(model, SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT, transaction: transaction);

            return response;
        }

        public async Task<QueryResponseData> Update(T modelToChange,T customWhereClauseObjectInstance, DbTransaction transaction = null)
        {
            QueryResponseData response = null;

            response = await SqlOp(modelToChange, SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, customWhereClauseObjectInstance, transaction: transaction);
            return response;
        }

        public async Task<QueryResponseData> Delete(T model, DbTransaction transaction = null)
        {
            QueryResponseData queryResponseData = await SqlOp(model, SQLDefinitionProperties.SQL_STATEMENT_ART.DELETE,transaction: transaction);
            return queryResponseData;
        }

        public async Task<QueryResponseData<T>> Select(T model,T whereClauseModel =null)
        {
            bool whereClauseNotPreSetted = whereClauseModel == null;
            whereClauseModel = whereClauseNotPreSetted ? 
                (Activator.CreateInstance<T>()):whereClauseModel;

            whereClauseModel.Uuid = model.Uuid;
            whereClauseModel.Deleted = false;

            QueryResponseData<T> response = null;

            response = await SqlOp(model, SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, whereClauseModel);

            return response;
        }

        
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            
        }

        #endregion
    }
}
