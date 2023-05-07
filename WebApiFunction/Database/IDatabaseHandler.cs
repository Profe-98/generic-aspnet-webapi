using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data.Common;
using System.Runtime.CompilerServices;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
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

namespace WebApiFunction.Database
{
    public interface IDatabaseHandler : IDisposable
    {

        event EventHandler<QueryException> ExceptionOccuredEvent;

        public event EventHandler<QueryException> OnExceptionOccured
        {
            add => ExceptionOccuredEvent += value;
            remove => ExceptionOccuredEvent -= value;
        }
        public Dictionary<string, MySqlDatabaseHandler.MySqlEnvVarModel<object>> GetAllResets();
        public Task<bool> SetEnvironmentVar<T>(string var, T val, T resetVal, bool resetAutoByNextQuery = true, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<bool> ResetEnvironmentVar(string var, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData> ExecuteQuery(string query, MySqlCommand mySqlCmd = null, QueryResponseData queryResponseData = null, int uuidIndex = GeneralDefs.NotFoundResponseValue, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData> ExecuteScript(string query, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData> ExecuteQuery<T>(string query, object objectInstance, QueryResponseData queryResponseData = null, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData<T>> ExecuteQuery<T>(string query, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public QueryResponseData ExecuteQuerySync(string query, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData<T>> ExecuteQueryWithMap<T>(string query, object objectInstance, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData<object>> ExecuteQueryWithMap(string query, object objectInstance, Type type, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<DbTransaction> CreateTransaction([CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public void Rollback(DbTransaction transaction, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public void Commit(DbTransaction transaction, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        public Task<QueryResponseData> ExecuteBulkLoad(string filePath, BulkLoaderOptions bulkLoaderOptions, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="fieldSet">WHERE Clause for Select or Update/Insert Fields</param>
        /// <returns></returns>
        public Task<QueryResponseData<T>> ExecuteQueryWithDapper<T>(string query, object param = default, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0) where T : class;

        public Task<QueryResponseData> GetUUID();
        public string ConvertArrayToWhereInFieldArray<T>(T[] arrayOrList) where T : struct;
        public string ConvertListToWhereInFieldArray<T>(List<T> arrayOrList) where T : struct;
        public bool IsUUIDFormat(string value);
        Guid InstanceGuid { get; }
    }

    public interface ISingletonDatabaseHandler : IDatabaseHandler
    {

    }
    public interface IScopedDatabaseHandler : IDatabaseHandler
    {

    }
    public interface ITransientDatabaseHandler : IDatabaseHandler
    {

    }
    public interface ISingletonNodeDatabaseHandler : IDatabaseHandler
    {

    }
}
