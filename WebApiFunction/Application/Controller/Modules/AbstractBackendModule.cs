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
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySQL.Table;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Web.AspNet.Filter;
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
using Microsoft.Extensions.DependencyInjection;
using WebApiFunction.Application.Model.Database.MySQL.Dapper.Context;
using WebApiFunction.Application.Model.Database.MySQL;
using Dapper;

namespace WebApiFunction.Application.Controller.Modules
{
    public class AbstractBackendModule<T> : IAbstractBackendModule<T>, IDisposable where T : AbstractModel
    {
        #region Private
        private readonly ICachingHandler _cachingHandler;
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
        private async Task<QueryResponseData> SqlOp(T model, SQLDefinitionProperties.SQL_STATEMENT_ART stmnt, DbTransaction transaction = null)
        {

            string query = model.GenerateQuery(stmnt, null, model).ToString();
            QueryResponseData dt = await Db.ExecuteQuery<T>(query, model, transaction: transaction);
            return dt;
        }

        private async Task<QueryResponseData<T>> SqlOp(T model, SQLDefinitionProperties.SQL_STATEMENT_ART stmnt, T whereClauseValueObject, DbTransaction transaction = null)
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

        #endregion
    }
}
