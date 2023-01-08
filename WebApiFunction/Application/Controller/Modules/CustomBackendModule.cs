using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Common;
using MySql.Data.MySqlClient;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Application.Controller.DapperModules;
using WebApiFunction.Application.Model.Database.MySql.Dapper.Context;

namespace WebApiFunction.Application.Controller.Modules
{
    public class CustomBackendModule<T> : IDisposable
        where T : AbstractModel
    {
        #region Private
        private readonly ICachingHandler _cachingHandler;
        private readonly IScopedDatabaseHandler _db = null;
        private readonly IAbstractDapperRepository<T> _abstractDapperRepository;
        private readonly MysqlDapperContext _mysqlDapperContext;
        #endregion
        public IScopedDatabaseHandler Db
        {
            get
            {
                return _db;
            }
        }
        #region Ctor
        public CustomBackendModule(IScopedDatabaseHandler databaseHandler, ICachingHandler cachingHandler,WebApiFunction.Application.Model.Database.MySql.Dapper.Context.MysqlDapperContext mysqlDapperContext)
        {
            _cachingHandler = cachingHandler;
            _db = databaseHandler;
            _mysqlDapperContext = mysqlDapperContext;
            _abstractDapperRepository = InitDapper();
            string t = typeof(T).Name;
        }
        private IAbstractDapperRepository<T> InitDapper()
        {

            Type dapperType = typeof(AbstractDapperRepository<>);
            var newT = dapperType.MakeGenericType(typeof(T));
            var dapperInstance = (IAbstractDapperRepository<T>)Activator.CreateInstance(newT,args: new object[] { _mysqlDapperContext });
            return dapperInstance;
        }
        ~CustomBackendModule()
        {
            Dispose();
        }
        #endregion
        #region Methods
        private async Task<QueryResponseData> SqlOp(T model, SQLDefinitionProperties.SQL_STATEMENT_ART stmnt, DbTransaction transaction = null)
        {
            string query = model.GenerateQuery(stmnt, null, model).ToString();
            QueryResponseData dt = await Db.ExecuteQuery<T>(query, model, transaction: transaction);
            return dt;
        }

        private async Task<QueryResponseData<T>> SqlOp(T model, SQLDefinitionProperties.SQL_STATEMENT_ART stmnt, T whereClauseValueObject, DbTransaction transaction = null)
        {
            string query = model.GenerateQuery(stmnt, whereClauseValueObject, model).ToString();
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
            response = await SqlOp(model, SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT, transaction: transaction);

            return response;
        }

        public async Task<QueryResponseData> Update(T modelToChange, T customWhereClauseObjectInstance, DbTransaction transaction = null)
        {
            QueryResponseData response = null;

            response = await SqlOp(modelToChange, SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE, customWhereClauseObjectInstance, transaction: transaction);
            return response;
        }

        public async Task<QueryResponseData> Delete(T model, DbTransaction transaction = null)
        {
            QueryResponseData queryResponseData = await SqlOp(model, SQLDefinitionProperties.SQL_STATEMENT_ART.DELETE, transaction: transaction);
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
