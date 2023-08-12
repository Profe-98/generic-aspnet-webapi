using System.Data.Common;
using Application.Shared.Kernel.Application.Model.Dapper.Mysql.Context;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Application.Model.Database.MySQL;
using Application.Shared.Kernel.Application.Model.DataTransferObject;
using Application.Shared.Kernel.Infrastructure.Database.Mysql;

namespace Application.Shared.Kernel.Application.Controller.Modules
{
    public class AbstractBackendModule<T> : IAbstractBackendModule<T>, IDisposable where T : AbstractModel
    {
        #region Private
        protected readonly ICachingHandler _cachingHandler;
        private readonly ISingletonDatabaseHandler _db = null;
        private IMysqlDapperContext _mysqlDapperContext;
        #endregion
        public ISingletonDatabaseHandler Db
        {
            get
            {
                return _db;
            }
        }

        public IMysqlDapperContext MysqlDapperContext => _mysqlDapperContext;

        #region Ctor
        public AbstractBackendModule(
            ISingletonDatabaseHandler databaseHandler, 
            ICachingHandler cachingHandler,
            IMysqlDapperContext mysqlDapperContext)
        {
            _cachingHandler = cachingHandler;
            _db = databaseHandler;
            _mysqlDapperContext = mysqlDapperContext;

        }
        ~AbstractBackendModule()
        {
            Dispose();
        }
        #endregion
        #region Methods
        private async Task<QueryResponseData> SqlOp(T model, MySqlDefinitionProperties.SQL_STATEMENT_ART stmnt, DbTransaction transaction = null)
        {

            string query = model.GenerateQuery(stmnt, null, model).ToString();
            QueryResponseData dt = await Db.ExecuteQuery<T>(query, model, transaction: transaction);
            return dt;
        }

        private async Task<QueryResponseData<T>> SqlOp(T model, MySqlDefinitionProperties.SQL_STATEMENT_ART stmnt, T whereClauseValueObject, DbTransaction transaction = null)
        {
            string query = model.GenerateQuery(stmnt, whereClauseValueObject, model).ToString();
            model.Uuid = whereClauseValueObject.Uuid;
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

        public async Task<QueryResponseData> Insert(T model, DbTransaction transaction = null)
        {
            QueryResponseData response = null;
            response = await SqlOp(model, MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT, transaction: transaction);

            return response;
        }

        public async Task<QueryResponseData> Update(T modelToChange, T customWhereClauseObjectInstance, DbTransaction transaction = null)
        {
            QueryResponseData response = null;

            response = await SqlOp(modelToChange, MySqlDefinitionProperties.SQL_STATEMENT_ART.UPDATE, customWhereClauseObjectInstance, transaction: transaction);
            return response;
        }

        public async Task<QueryResponseData> Delete(T model, DbTransaction transaction = null)
        {
            QueryResponseData queryResponseData = await SqlOp(model, MySqlDefinitionProperties.SQL_STATEMENT_ART.DELETE, transaction: transaction);
            return queryResponseData;
        }

        public async Task<QueryResponseData<T>> Select(T model, T whereClauseModel = null)
        {
            bool whereClauseNotPreSetted = whereClauseModel == null;
            whereClauseModel = whereClauseNotPreSetted ?
                Activator.CreateInstance<T>() : whereClauseModel;

            whereClauseModel.Uuid = model.Uuid;
            whereClauseModel.Deleted = false;

            QueryResponseData<T> response = null;

            response = await SqlOp(model, MySqlDefinitionProperties.SQL_STATEMENT_ART.SELECT, whereClauseModel);

            return response;
        }

        public AbstractBackendModule<T> BackendModule
        {
            get
            {
                return this;
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);

        }

        public Guid CreateUuid()
        {
            return Guid.NewGuid();
        }

        #endregion
    }
}
