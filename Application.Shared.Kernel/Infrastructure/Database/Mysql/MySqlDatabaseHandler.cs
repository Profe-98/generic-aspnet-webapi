using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using Dapper;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Service;

namespace Application.Shared.Kernel.Infrastructure.Database.Mysql
{

    public class MySqlDatabaseHandler : IScopedDatabaseHandler, ISingletonDatabaseHandler, ITransientDatabaseHandler, ISingletonNodeDatabaseHandler
    {
        public struct MYSQL_ENV_VARS
        {
            public const string SAFE_UPDATES = "SQL_SAFE_UPDATES";
            public const string AUTO_COMMIT = "AUTOCOMMIT";
        }

        public Guid InstanceGuid { get => Guid.NewGuid(); }
        public string MonitorLockObject = "thread_lock_databasehandler_executequery=";
        private bool _threadSafetyDataReader = false;
        private Dictionary<string, MySqlEnvVarModel<object>> _instanceStorage = new Dictionary<string, MySqlEnvVarModel<object>>();
        private DatabaseAttributeManager _databaseAttributeManager = new DatabaseAttributeManager();
        private DateTime _connectionCloseDateTime = DateTime.MinValue;
        private DateTime _connectionCreateDateTime = DateTime.MinValue;
        private MySqlConnection _odbcConnection = null;
        private string _odbcConnectionString = null;
        private int _odbcConnectionTimeout = 15;
        public bool IsConnectionAvaible { get; set; }
        private bool _autoCommit = false;
        private DbDataReader reader = null;
        public DbDataReader CurrentReader
        {
            get { return reader; }
        }
        private MySqlCommand command = null;

        public event EventHandler<QueryException> ExceptionOccuredEvent;

        public MySqlCommand CurrentCommand
        {
            get { return command; }
        }
        public bool AutoCommit
        {
            get
            {
                return _autoCommit;
            }
            set
            {
                _autoCommit = value;
                if (value == false)
                {
                    SetEnvironmentVar(MYSQL_ENV_VARS.AUTO_COMMIT, 1, 0, false);

                }
                else
                {
                    ResetEnvironmentVar(MYSQL_ENV_VARS.AUTO_COMMIT);
                }
            }
        }

        public DatabaseAttributeManager DatabaseAttributeManager
        {
            get
            {
                return _databaseAttributeManager;
            }
        }

        internal MySqlDatabaseHandler()
        {
            ExceptionOccuredEvent += MySqlDatabaseHandler_ExceptionOccuredEvent;
        }

        private void MySqlDatabaseHandler_ExceptionOccuredEvent(object? sender, QueryException e)
        {

        }

        public MySqlDatabaseHandler(IAppconfig appconfig) : this()
        {
            InitODBCHandler(appconfig.AppServiceConfiguration.DatabaseConfigurationModel.MysqlConnectionString);
            AutoCommit = appconfig.AppServiceConfiguration.DatabaseConfigurationModel.AutoCommit;

        }
        public MySqlDatabaseHandler(MySqlConnectionStringBuilder connectionString, bool autoCommit) : this()
        {
            InitODBCHandler(connectionString);
            AutoCommit = autoCommit;

        }
        private void InitODBCHandler(MySqlConnectionStringBuilder connectionString, bool threadSafetyDataReader = true)
        {
            MonitorLockObject += Guid.NewGuid().ToString();
            if (connectionString == null)
                return;

            _threadSafetyDataReader = threadSafetyDataReader;
            if (_odbcConnection == null)
            {
                _odbcConnectionString = connectionString.ToString();
                _odbcConnection = new MySqlConnection(_odbcConnectionString);
            }

        }
        public async Task<bool> ResetEnvironmentVar(string var, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData queryResponseData = null;
            if (_instanceStorage.ContainsKey(var))
            {
                string tmpQuery = "SET " + _instanceStorage[var].Value + " = " + _instanceStorage[var].ResetValue + "; ";
                queryResponseData = await ExecuteQuery(tmpQuery);
            }
            return queryResponseData != null && queryResponseData.HasSuccess;
        }
        public async Task<bool> SetEnvironmentVar<T>(string var, T val, T resetVal, bool resetAutoByNextQuery = true, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            if (_instanceStorage.ContainsKey(var))
            {
                _instanceStorage[var].Value = val;
                _instanceStorage[var].ResetValue = resetVal;
                _instanceStorage[var].ResetAfterNextQueryExec = resetAutoByNextQuery;
            }
            else
            {
                _instanceStorage.Add(var, new MySqlEnvVarModel<object>
                {
                    Value = val,
                    ResetValue = resetVal,
                    ResetAfterNextQueryExec = resetAutoByNextQuery
                });
            }
            string tmpQuery = "SET " + var + " = " + val + "; ";
            QueryResponseData queryResponseData = await ExecuteQuery(tmpQuery);
            return queryResponseData.HasSuccess;
        }
        public string ConvertArrayToWhereInFieldArray<T>(T[] arrayOrList) where T : struct
        {
            if (arrayOrList == null)
                return null;

            return ConvertListToWhereInFieldArray(arrayOrList.ToList());
        }
        /// <summary>
        /// Convert an array with primitive types (int,string,double etc.) to an IN-WHERE-CLAUSE Field-Array e.g. 
        /// ConvertListToWhereInFieldArray<int>(new List<int>{0,1,2,10}) => string: IN (0,1,2,10)
        /// or for string List
        /// ConvertListToWhereInFieldArray<string>(new List<string>{"0","1","2","10"}) => string: IN ('0','1','2','10')
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayOrList"></param>
        /// <returns></returns>
        public string ConvertListToWhereInFieldArray<T>(List<T> arrayOrList) where T : struct
        {
            if (arrayOrList == null)
                return null;

            string response = arrayOrList.Count == 0 ? null : "IN (";
            for (int i = 0; i < arrayOrList.Count; i++)
            {
                string tmp = null;
                if (typeof(T) == typeof(string) || typeof(T) == typeof(DateTime) || typeof(T) == typeof(Guid) || typeof(T) == typeof(float) || typeof(T) == typeof(double))
                {
                    tmp = "'" + arrayOrList[i] + "'";
                }
                else
                {
                    tmp = "" + arrayOrList[i] + "";
                }
                response += tmp;
                if (i < arrayOrList.Count - 1)
                    response += ",";
            }
            if (response != null)
                response += ")";
            return response;
        }
        public virtual void Dispose()
        {
            if (ConnectionState == ConnectionState.Open)
                Close();

            GC.SuppressFinalize(this);
        }

        public bool IsUUIDFormat(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        public async Task<bool> OpenAsync()
        {
            if (_odbcConnection.State != ConnectionState.Open)
            {
                try
                {
                    await _odbcConnection.OpenAsync();
                    _connectionCreateDateTime = DateTime.Now;
                    IsConnectionAvaible = true;
                    return true;
                }
                catch (Exception ex)
                {
                    IsConnectionAvaible = false;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                IsConnectionAvaible = false;
                return false;
            }
            IsConnectionAvaible = true;
            return true;
        }
        public bool Open()
        {
            if (_odbcConnection.State != ConnectionState.Open)
            {
                try
                {
                    _odbcConnection.Open();
                    _connectionCreateDateTime = DateTime.Now;
                    IsConnectionAvaible = true;
                    return true;
                }
                catch (Exception ex)
                {
                    IsConnectionAvaible = false;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                IsConnectionAvaible = false;
                return false;
            }
            return true;
        }
        public void Close()
        {
            if (_odbcConnection.State == ConnectionState.Open)
            {
                try
                {
                    _odbcConnection.Close();
                    _connectionCloseDateTime = DateTime.Now;
                    IsConnectionAvaible = false;
                }
                catch (Exception ex)
                {
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
            }
        }

        public void ChangeDatabase(string database)
        {
            if (string.IsNullOrEmpty(database))
                return;

            _odbcConnection.ChangeDatabase(database);
        }

        /// <summary>
        /// Gets the next auto_increment id for table
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="autoIncrementColumnId"></param>
        /// <returns></returns>
        public int GetAutoIncrementId(DataTable dataTable, int autoIncrementColumnId = GeneralDefs.NotFoundResponseValue)
        {
            int responseValue = GeneralDefs.NotFoundResponseValue;
            int autoIncrementStep = 1;
            if (autoIncrementColumnId == GeneralDefs.NotFoundResponseValue)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    DataColumn column = dataTable.Columns[i];
                    if (column.AutoIncrement)
                    {
                        autoIncrementColumnId = i;
                        autoIncrementStep = Convert.ToInt32(column.AutoIncrementStep);
                        break;
                    }
                }
            }
            if (dataTable.Columns.Count > autoIncrementColumnId)
            {
                if (autoIncrementColumnId != GeneralDefs.NotFoundResponseValue)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int currentRowId = (int)row[autoIncrementColumnId];
                        if (currentRowId > responseValue)
                        {
                            responseValue = currentRowId;
                        }
                    }

                    if (responseValue != -1)
                    {
                        responseValue += autoIncrementStep;//max value that we find in datatable add 1 
                    }
                }
            }
            return responseValue;
        }
        public async Task<DbTransaction> CreateTransaction([CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            bool open = Open();
            return await _odbcConnection.BeginTransactionAsync();
        }
        public async void Rollback(DbTransaction transaction, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            try
            {
                bool open = Open();
                await transaction.RollbackAsync();

            }
            catch (Exception ex)
            {

            }
        }
        public async void Commit(DbTransaction transaction, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            try
            {
                bool open = Open();
                await transaction.CommitAsync();


            }
            catch (Exception ex)
            {

            }
        }
        public MySqlCommand GetCommandInstance(string query)
        {
            return new MySqlCommand(query, _odbcConnection);
        }
        public Guid GetUUID()
        {
            return Guid.NewGuid();
        }
        /// <summary>
        /// Last inserted id queried by user (only available for tables w/ auto_increment id column)
        /// </summary>
        /// <returns></returns>
        public async Task<ulong> LastInsertId()
        {
            QueryResponseData response = await ExecuteQuery("SELECT LAST_INSERT_ID();");
            return response.HasData ?
                (ulong)response.Data.Rows[0].ItemArray[0] : ulong.MinValue;
        }
        public async Task<QueryResponseData<T>> ExecuteQuery<T>(string query, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData<T> responseData = new QueryResponseData<T>(query);
            QueryResponseData tmpData = await ExecuteQuery(query, null, responseData, transaction: transaction, file: file, member: member, line: line);
            responseData = (QueryResponseData<T>)tmpData;
            responseData = await FillGenericDataSet<T>(tmpData);

            return responseData;
        }
        private async Task<QueryResponseData<T>> FillGenericDataSet<T>(QueryResponseData queryResponseData)
        {
            QueryResponseData<T> queryResponse = (QueryResponseData<T>)queryResponseData;
            if (queryResponseData.StatementArt == MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT)
            {
                string insertResultGetQuery = "SELECT * FROM " + queryResponseData.PrimaryNode.NodeFullyName + " WHERE " + MySqlDefinitionProperties.GeneralUuidFieldName + " = '" + queryResponseData.LastInsertedId + "';";
                QueryResponseData<T> insertedData = await ExecuteQuery<T>(insertResultGetQuery);
                if (insertedData.HasData)
                {
                    queryResponseData.Data = insertedData.Data;
                }
            }
            if (queryResponseData.HasData && !queryResponseData.HasErrors)
            {
                List<T> obj = _databaseAttributeManager.MapValueFromDataTableToGenericList<T>(queryResponseData.Data);

                queryResponse.DataStorage = obj;
            }
            return queryResponse;
        }
        private async Task<QueryResponseData<object>> FillGenericDataSet(QueryResponseData queryResponseData, Type type)
        {
            QueryResponseData<object> queryResponse = (QueryResponseData<object>)queryResponseData;
            if (queryResponseData.StatementArt == MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT)
            {
                string insertResultGetQuery = "SELECT * FROM " + queryResponseData.PrimaryNode.NodeFullyName + " WHERE " + MySqlDefinitionProperties.GeneralUuidFieldName + " = '" + queryResponseData.LastInsertedId + "';";
                QueryResponseData<object> insertedData = await ExecuteQueryWithMap(insertResultGetQuery, null, type);
                if (insertedData.HasData)
                {
                    queryResponseData.Data = insertedData.Data;
                }
            }
            if (queryResponseData.HasData && !queryResponseData.HasErrors)
            {
                List<object> obj = _databaseAttributeManager.MapValueFromDataTableToGenericList(queryResponseData.Data, type);
                queryResponse.DataStorage = obj;
            }
            return queryResponse;
        }
        public async Task<QueryResponseData<T>> ExecuteQueryWithMap<T>(string query, object objectInstance, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData<T> responseData = new QueryResponseData<T>(query);
            QueryResponseData tmpData = await ExecuteQuery<T>(query, objectInstance, responseData, transaction: transaction, file: file, member: member, line: line);
            responseData = (QueryResponseData<T>)tmpData;

            responseData = await FillGenericDataSet<T>(tmpData);

            return responseData;
        }
        public async Task<QueryResponseData<object>> ExecuteQueryWithMap(string query, object objectInstance, Type type, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData<object> responseData = new QueryResponseData<object>(query);
            QueryResponseData tmpData = await ExecuteQuery<object>(query, objectInstance, responseData, transaction: transaction, file: file, member: member, line: line);
            responseData = (QueryResponseData<object>)tmpData;

            responseData = await FillGenericDataSet(tmpData, type);

            return responseData;
        }
        public async Task<QueryResponseData> ExecuteQuery<T>(string query, object objectInstance, QueryResponseData queryResponseData, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            queryResponseData = queryResponseData == null ? new QueryResponseData(query) : queryResponseData;
            MySqlCommand mySqlCommand = new MySqlCommand(query, _odbcConnection);
            _databaseAttributeManager.FetchObjectToCommandParameters(objectInstance, ref mySqlCommand, MySqlDefinitionProperties.SQL_STATEMENT_ART.NONE);
            int indexOfUuidField = GeneralDefs.NotFoundResponseValue;
            if (mySqlCommand.Parameters != null)
            {
                if (queryResponseData.StatementArt == MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT)
                {
                    indexOfUuidField = mySqlCommand.Parameters.IndexOf("@" + MySqlDefinitionProperties.GeneralUuidFieldName);

                    if (indexOfUuidField != GeneralDefs.NotFoundResponseValue)
                    {

                        if ((mySqlCommand.Parameters[indexOfUuidField].DbType == DbType.StringFixedLength || mySqlCommand.Parameters[indexOfUuidField].DbType == DbType.String) && (mySqlCommand.Parameters[indexOfUuidField].Value == null || (Guid)mySqlCommand.Parameters[indexOfUuidField].Value == Guid.Empty))
                        {
                            Guid guidResp = GetUUID();
                            mySqlCommand.Parameters[indexOfUuidField].Value = guidResp.ToString();
                        }
                    }
                }

            }
            return await ExecuteQuery(query, mySqlCommand, queryResponseData, indexOfUuidField, transaction: transaction, file: file, member: member, line: line);
        }

        public QueryResponseData ExecuteQuerySync(string query, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData responseValue = new QueryResponseData(query);
            DataTable responseData = new DataTable();
            DataTable responseMetaData = new DataTable();


            if (string.IsNullOrEmpty(query))
                return null;

            if (_threadSafetyDataReader)
                Monitor.Enter(MonitorLockObject);//wenn über BackgroundService Actions mit Query Execution ausgeführt werden sollen, muss DbDataReader durch lock von Multitask/Multithread Zugriff geschützt werden, da der DbDataReader nur mit einer Connection Instance readen kann

            bool openedConnection = Open();
            if (openedConnection)
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(query, _odbcConnection);

                    using (DbDataReader reader = command.ExecuteReader())
                    {

                        if (reader.FieldCount != 0)
                        {
                            responseData = new DataTable();

                            int relativeIncrement = 0;
                            bool run = true;
                            DateTime startGetData = DateTime.Now.AddSeconds(_odbcConnectionTimeout);
                            DataTable schemaData = reader.GetSchemaTable();
                            responseMetaData = schemaData;

                            while (reader.Read() && run)
                            {
                                if (startGetData <= DateTime.Now)
                                {
                                    run = false;
                                }
                                DataRow row = responseData.NewRow();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    object val = reader.GetFieldValue<object>(i);
                                    string columnName = reader.GetName(i);
                                    if (columnName == null) continue;
                                    if (!ColumnExistInDataTable(responseData, columnName))
                                    {
                                        responseData.Columns.Add(columnName, reader.GetFieldType(i));
                                    }
                                    row[columnName] = val;
                                }

                                if (row != null)
                                {
                                    responseData.Rows.Add(row);
                                }

                                relativeIncrement++;
                            }

                        }
                    }
                    responseValue.Message = GetDataTableRowCount(responseData) + " returned";
                }
                catch (MySqlException ex)
                {
                    responseValue.Number = ex.Number;
                    responseValue.Exception = ex;
                    responseValue.ErrorCode = ex.ErrorCode;
                    responseValue.Message = ex.Message;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                Close();
            }
            if (_threadSafetyDataReader && Monitor.IsEntered(MonitorLockObject))
                Monitor.Exit(MonitorLockObject);

            responseValue.Data = responseData;
            responseValue.MetaData = responseMetaData;
            return responseValue;
        }
        public async Task<QueryResponseData> ExecuteScript(string query, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData responseValue = new QueryResponseData(query);
            DataTable responseData = new DataTable();
            DataTable responseMetaData = new DataTable();


            if (string.IsNullOrEmpty(query))
                return null;

            if (_threadSafetyDataReader)
                Monitor.Enter(MonitorLockObject);//wenn über BackgroundService Actions mit Query Execution ausgeführt werden sollen, muss DbDataReader durch lock von Multitask/Multithread Zugriff geschützt werden, da der DbDataReader nur mit einer Connection Instance readen kann

            bool openedConnection = Open();
            if (openedConnection)
            {
                try
                {
                    MySqlScript script = new MySqlScript(_odbcConnection, query);
                    int resp = await script.ExecuteAsync();

                    responseValue.Message = GetDataTableRowCount(responseData) + " returned";
                }
                catch (MySqlException ex)
                {
                    responseValue.Number = ex.Number;
                    responseValue.Exception = ex;
                    responseValue.ErrorCode = ex.ErrorCode;
                    responseValue.Message = ex.Message;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                Close();
            }
            if (_threadSafetyDataReader && Monitor.IsEntered(MonitorLockObject))
                Monitor.Exit(MonitorLockObject);

            responseValue.Data = responseData;
            responseValue.MetaData = responseMetaData;
            return responseValue;
        }
        public async Task<QueryResponseData> ExecuteBulkLoad(string filePath, BulkLoaderOptions bulkLoaderOptions, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData responseValue = new QueryResponseData(true, filePath);
            DataTable responseData = new DataTable();
            DataTable responseMetaData = new DataTable();


            if (string.IsNullOrEmpty(filePath))
                return null;

            if (!File.Exists(filePath))
                return null;

            if (_threadSafetyDataReader)
                Monitor.Enter(MonitorLockObject);//wenn über BackgroundService Actions mit Query Execution ausgeführt werden sollen, muss DbDataReader durch lock von Multitask/Multithread Zugriff geschützt werden, da der DbDataReader nur mit einer Connection Instance readen kann

            bool openedConnection = Open();
            if (openedConnection)
            {
                try
                {
                    MySqlBulkLoader bulkloader = new MySqlBulkLoader(_odbcConnection);

                    bulkloader.Local = bulkLoaderOptions.Local;
                    bulkloader.TableName = bulkLoaderOptions.TableName;
                    bulkloader.FieldTerminator = bulkLoaderOptions.FieldTerminator;
                    bulkloader.FileName = bulkLoaderOptions.FileName;
                    if (bulkLoaderOptions.NumberOfLinesToSkip != GeneralDefs.NotFoundResponseValue)
                        bulkloader.NumberOfLinesToSkip = bulkLoaderOptions.NumberOfLinesToSkip;
                    if (bulkLoaderOptions.Priority != MySqlBulkLoaderPriority.None)
                        bulkloader.Priority = bulkLoaderOptions.Priority;
                    bulkloader.LineTerminator = bulkLoaderOptions.LineTerminator;
                    if (bulkLoaderOptions.FieldQuotationCharacter != char.MinValue)
                        bulkloader.FieldQuotationCharacter = bulkLoaderOptions.FieldQuotationCharacter;
                    bulkloader.FieldQuotationOptional = bulkLoaderOptions.FieldQuotationOptional;
                    if (bulkLoaderOptions.Timeout != GeneralDefs.NotFoundResponseValue)
                        bulkloader.Timeout = bulkLoaderOptions.Timeout;
                    if (bulkLoaderOptions.CharacterSet != null)
                        bulkloader.CharacterSet = bulkLoaderOptions.CharacterSet;
                    if (bulkLoaderOptions.Columns != null)
                        bulkloader.Columns.AddRange(bulkLoaderOptions.Columns);
                    if (bulkLoaderOptions.ConflictOption != MySqlBulkLoaderConflictOption.None)
                        bulkloader.ConflictOption = bulkLoaderOptions.ConflictOption;
                    if (bulkLoaderOptions.EscapeCharacter != char.MinValue)
                        bulkloader.EscapeCharacter = bulkLoaderOptions.EscapeCharacter;
                    if (bulkLoaderOptions.Expressions != null)
                        bulkloader.Expressions.AddRange(bulkLoaderOptions.Expressions);
                    int resp = await bulkloader.LoadAsync();

                    responseValue.Message = GetDataTableRowCount(responseData) + " returned";
                }
                catch (MySqlException ex)
                {
                    responseValue.Number = ex.Number;
                    responseValue.Exception = ex;
                    responseValue.ErrorCode = ex.ErrorCode;
                    responseValue.Message = ex.Message;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                Close();
            }
            if (_threadSafetyDataReader && Monitor.IsEntered(MonitorLockObject))
                Monitor.Exit(MonitorLockObject);

            responseValue.Data = responseData;
            responseValue.MetaData = responseMetaData;
            return responseValue;
        }

        /// <summary>
        /// Executes SQL Statements on the Connected SQL Server
        /// </summary>
        /// <param name="query">The SQL Statement that should be execute (DCL&DML&DDL)</param>
        /// <param name="rowDefaultBackupColumn">Stores the Default-Backup Data of each Row as Object in the cell, for e.g. Undo operations</param>
        /// <returns>The QueryResponseData Object that contains all Data</returns>
        public async Task<QueryResponseData> ExecuteQuery(string query, MySqlCommand mySqlCmd = null, QueryResponseData queryResponseData = null, int uuidIndex = GeneralDefs.NotFoundResponseValue, DbTransaction transaction = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0)
        {
            QueryResponseData responseValue = queryResponseData == null ?
                new QueryResponseData(query) : queryResponseData;

            DataTable responseData = new DataTable();
            DataTable responseMetaData = new DataTable();


            responseValue.SetTransaction(transaction);
            responseValue.QueryParametes = mySqlCmd == null ?
                null : mySqlCmd.Parameters;

            if (string.IsNullOrEmpty(query))
                return null;

            if (_threadSafetyDataReader)
                Monitor.Enter(MonitorLockObject);//wenn über BackgroundService Actions mit Query Execution ausgeführt werden sollen, muss DbDataReader durch lock von Multitask/Multithread Zugriff geschützt werden, da der DbDataReader nur mit einer Connection Instance readen kann


            bool openedConnection = Open();
            if (openedConnection)
            {

                try
                {
                    using (command = mySqlCmd == null ?
                        !responseValue.IsTransactionalOperation ? new MySqlCommand(query, _odbcConnection) : new MySqlCommand(query, _odbcConnection, (MySqlTransaction)transaction) : mySqlCmd)
                    {
                        using (reader = await command.ExecuteReaderAsync())
                        {

                            if (reader.FieldCount != 0)
                            {
                                responseData = new DataTable();

                                int relativeIncrement = 0;
                                bool run = true;
                                DateTime startGetData = DateTime.Now.AddSeconds(_odbcConnectionTimeout);
                                DataTable schemaData = reader.GetSchemaTable();
                                responseMetaData = schemaData;

                                while (await reader.ReadAsync() && run)
                                {
                                    if (startGetData <= DateTime.Now)
                                    {
                                        run = false;
                                    }
                                    DataRow row = responseData.NewRow();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        object val = await reader.GetFieldValueAsync<object>(i);
                                        string columnName = reader.GetName(i);
                                        if (columnName == null) continue;
                                        if (!ColumnExistInDataTable(responseData, columnName))
                                        {
                                            responseData.Columns.Add(columnName, reader.GetFieldType(i));
                                        }
                                        row[columnName] = val;
                                    }

                                    if (row != null)
                                    {
                                        responseData.Rows.Add(row);
                                    }

                                    relativeIncrement++;
                                }

                            }
                            reader.Close();
                        }
                        if (mySqlCmd != null && uuidIndex != GeneralDefs.NotFoundResponseValue)
                        {

                            if (responseValue.StatementArt == MySqlDefinitionProperties.SQL_STATEMENT_ART.INSERT)
                            {
                                responseValue.LastInsertedId = mySqlCmd.Parameters[uuidIndex].Value;
                            }
                        }
                    }

                    responseValue.Message = GetDataTableRowCount(responseData) + " returned";
                }
                catch (MySqlException ex)
                {
                    if (transaction != null)
                    {
                        Rollback(transaction);
                    }
                    responseValue.Number = ex.Number;
                    responseValue.Exception = ex;
                    responseValue.ErrorCode = ex.ErrorCode;
                    responseValue.Message = ex.Message;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                Close();
            }
            if (_threadSafetyDataReader && Monitor.IsEntered(MonitorLockObject))
                Monitor.Exit(MonitorLockObject);

            responseValue.Data = responseData;
            responseValue.MetaData = responseMetaData;

            var allResets = GetAllResets();
            foreach (var r in allResets.Keys)
            {
                var rV = allResets[r];
                bool resp = await SetEnvironmentVar(r, rV.ResetValue, rV.ResetValue, false);
                if (resp)
                {
                    _instanceStorage.Remove(r);
                }
                else
                {
                    throw new Exception("environment var cant reset after quere exec (" + r + ")");
                }
            }

            return responseValue;
        }

        public class MySqlGuidTypeHandler : SqlMapper.TypeHandler<Guid>
        {
            public override void SetValue(IDbDataParameter parameter, Guid guid)
            {
                parameter.Value = guid.ToString();
            }

            public override Guid Parse(object value)
            {
                return new Guid((string)value);
            }
        }

        public async Task<QueryResponseData<T>> ExecuteQueryWithDapper<T>(string query, object param = null, [CallerFilePath] string file = "", [CallerMemberName] string member = "", [CallerLineNumber] int line = 0) where T : class
        {
            QueryResponseData<T> responseValue = new QueryResponseData<T>(query);

            DataTable responseData = new DataTable();
            DataTable responseMetaData = new DataTable();

            if (string.IsNullOrEmpty(query))
                return null;

            if (_threadSafetyDataReader)
                Monitor.Enter(MonitorLockObject);//wenn über BackgroundService Actions mit Query Execution ausgeführt werden sollen, muss DbDataReader durch lock von Multitask/Multithread Zugriff geschützt werden, da der DbDataReader nur mit einer Connection Instance readen kann


            bool openedConnection = Open();
            if (openedConnection)
            {

                try
                {
                    /*var data = await _odbcConnection.QueryAsync<T,Models.UserRelationRoleModel,T>(query, (u,r) => {
                        u.UserRelationRoleModels = new List<Models.UserRelationRoleModel>();
                        u.UserRelationRoleModels.Add(r);
                        return u;
                    },splitOn: "uuid");
                    responseValue.DataStorage = data == null?
                        new List<T>():data.ToList();*/

                    ///_odbcConnection.QueryAsync<T>(query);

                    responseValue.Message = GetDataTableRowCount(responseData) + " returned";
                }
                catch (MySqlException ex)
                {
                    responseValue.Number = ex.Number;
                    responseValue.Exception = ex;
                    responseValue.ErrorCode = ex.ErrorCode;
                    responseValue.Message = ex.Message;
                    ExceptionOccuredEvent.Invoke(this, new QueryException { Exception = ex, Sender = this });
                }
                Close();
            }
            if (_threadSafetyDataReader && Monitor.IsEntered(MonitorLockObject))
                Monitor.Exit(MonitorLockObject);

            responseValue.Data = responseData;
            responseValue.MetaData = responseMetaData;

            var allResets = GetAllResets();
            foreach (var r in allResets.Keys)
            {
                var rV = allResets[r];
                bool resp = await SetEnvironmentVar(r, rV.ResetValue, rV.ResetValue, false);
                if (resp)
                {
                    _instanceStorage.Remove(r);
                }
                else
                {
                    throw new Exception("environment var cant reset after quere exec (" + r + ")");
                }
            }

            return responseValue;
        }
        public Dictionary<string, MySqlEnvVarModel<object>> GetAllResets()
        {
            Dictionary<string, MySqlEnvVarModel<object>> response = new Dictionary<string, MySqlEnvVarModel<object>>();
            _instanceStorage.Keys.ToList().ForEach((x) =>
            {
                MySqlEnvVarModel<object> tmp = _instanceStorage[x];
                if (tmp.ResetAfterNextQueryExec)
                {
                    response.Add(x, _instanceStorage[x]);
                }
            });
            return response;
        }
        public bool ColumnExistInDataTable(DataTable dataTable, string columnName)
        {
            if (dataTable == null)
                return false;

            if (columnName == null)
                return false;

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string tmpColumnName = dataTable.Columns[i].ColumnName.ToLower();
                if (tmpColumnName.Equals(columnName.ToLower()))
                    return true;
            }
            return false;
        }
        private int GetDataTableRowCount(DataTable dataTable)
        {
            int rowCount = 0;
            if (dataTable != null)
            {
                rowCount = dataTable.Rows.Count;
            }
            return rowCount;
        }

        public string ServerVersion
        {
            get
            {
                return _odbcConnection.ServerVersion;
            }
        }
        public string ConnectionString
        {
            get
            {
                return _odbcConnection.ConnectionString;
            }
        }
        public string Database
        {
            get
            {
                return _odbcConnection.Database;
            }
        }
        public DateTime ConnectionCreateDateTime
        {
            get
            {
                return _connectionCreateDateTime;
            }
        }
        public ConnectionState ConnectionState
        {
            get
            {
                return _odbcConnection.State;
            }
        }

        public T ConvertValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }


        public class MySqlEnvVarModel<T>
        {
            private T _value = default;
            private T _resetValue = default;

            public T Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    LastSetTime = DateTime.Now;
                    _value = value;
                }
            }
            public T ResetValue
            {
                get
                {
                    return _resetValue;
                }
                set
                {
                    _resetValue = value;
                }
            }
            public DateTime LastSetTime { get; private set; }
            public bool ResetAfterNextQueryExec { get; set; }
        }
    }

    public class BulkLoaderOptions
    {
        public string TableName { get; private set; }
        public bool Local { get; set; } = true;
        public string FieldTerminator { get; set; } = ";";
        public string FileName { get; private set; }
        public int NumberOfLinesToSkip { get; set; } = GeneralDefs.NotFoundResponseValue;
        public string LineTerminator { get; set; } = "\n";
        public MySqlBulkLoaderPriority Priority { get; set; } = MySqlBulkLoaderPriority.None;
        public char FieldQuotationCharacter { get; set; } = char.MinValue;
        public bool FieldQuotationOptional { get; set; } = false;
        public int Timeout { get; set; } = GeneralDefs.NotFoundResponseValue;
        public string CharacterSet { get; set; } = null;
        public List<string> Columns { get; set; } = null;
        public MySqlBulkLoaderConflictOption ConflictOption { get; set; } = MySqlBulkLoaderConflictOption.None;
        public char EscapeCharacter { get; set; } = char.MinValue;
        public List<string> Expressions { get; set; } = null;

        public BulkLoaderOptions(string table, string filePath)
        {
            FileName = filePath;
            TableName = table;
        }
    }

}
