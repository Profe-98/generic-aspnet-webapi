using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Infrastructure.Database
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

        public Guid GetUUID();
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
