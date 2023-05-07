using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Microsoft.Win32;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using System.Diagnostics;
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
    public class QueryResponseData<T> : QueryResponseData
    {
        private List<T> _dataStorage = default;
        public List<T> DataStorage
        {
            get
            {
                return _dataStorage;
            }
            set
            {
                _dataStorage = value;
            }
        }
        public T FirstRow
        {
            get
            {

                return GetNestedObjByIndex(0);
            }
        }
        public T LastRow
        {
            get
            {

                return GetNestedObjByIndex(_dataStorage != null ? _dataStorage.Count - 1 : GeneralDefs.NotFoundResponseValue);
            }
        }
        public bool HasStorageData
        {
            get
            {
                return _dataStorage != null ?
                    _dataStorage.Count != 0 && HasData && !HasErrors ? true : false : false;
            }
        }
        #region Ctor
        public QueryResponseData(string query) : base(query)
        {

        }
        #endregion
        #region Methods
        private T GetNestedObjByIndex(int index)
        {
            if (_dataStorage != null)
            {
                if (_dataStorage.Count != 0)
                {
                    if (index >= _dataStorage.Count || index < 0)
                        return default;

                    return _dataStorage[index];
                }
            }
            return default;
        }
        #endregion
    }
    public class QueryResponseData : IDisposable
    {
        #region Private
        private string _bulkfile = null;
        private bool _bulkerLoad = false;
        private string _statement = null;
        private Stopwatch _fetchData = new Stopwatch();
        private string _message = null;
        private int _number = GeneralDefs.NotFoundResponseValue;
        private int _errorCode = GeneralDefs.NotFoundResponseValue;
        private DataTable _data = null;
        private MySqlParameterCollection _queryParametes = null;
        private DataTable _metaData = null;
        private MySqlException _exceptionData = null;
        private DbTransaction _transaction = null;
        #endregion
        #region Public
        public class SchemaNode
        {
            #region Private
            private string _schemaName;
            private string _nodeName;
            private SQLDefinitionProperties.SQL_FROM_CLAUSES _fromClause;
            #endregion
            #region Public
            public string NodeFullyName
            {
                get
                {
                    string responseValue = null;
                    if (_schemaName == null)
                    {
                        responseValue = _nodeName;
                    }
                    else
                    {
                        responseValue = _schemaName + "." + _nodeName;
                    }
                    return responseValue;
                }
            }
            public string SchemaName
            {
                get
                {
                    return _schemaName;
                }
                set
                {
                    _schemaName = value;
                }
            }
            public string NodeName
            {
                get
                {
                    return _nodeName;
                }
                set
                {
                    _nodeName = value;
                }
            }
            public SQLDefinitionProperties.SQL_FROM_CLAUSES FromClause
            {
                get
                {
                    return _fromClause;
                }
                set
                {
                    _fromClause = value;
                }
            }
            #endregion
            #region Ctor & Dtor
            public SchemaNode()
            {

            }
            #endregion
            #region Methods
            #endregion
        }
        public DbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
        }
        public bool IsTransactionalOperation { get => _transaction != null; }
        public string StatementWithoutSpecialChars
        {
            get
            {
                if (_statement == null)
                    return null;
                return
                    _statement = _statement.Replace("`", "");
            }
        }
        /// <summary>
        /// Respond the included given Table/View/Functionnames/Schema/Procedurenames of this.Statement Property 
        /// </summary>
        public List<SchemaNode> StatementIncludedDbNodes
        {
            get
            {
                List<SchemaNode> responseValue = new List<SchemaNode>();
                SQLDefinitionProperties.SQL_STATEMENT_ART type = StatementArt;
                string statementTmp = StatementWithoutSpecialChars;
                if (statementTmp != null && type != SQLDefinitionProperties.SQL_STATEMENT_ART.NONE)
                {
                    List<string> splitList = null;
                    splitList = statementTmp.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (splitList == null)
                        return responseValue;

                    if (splitList.Count != 0)
                    {
                        if (type == SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT)
                        {
                            splitList.RemoveRange(0, 2);//INSERT INTO aus Idx 0&1 entfernen
                        }
                        else if (type == SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE)
                        {
                            splitList.RemoveAt(0);//UPDATE aus Idx 0 entfernen
                        }

                        if (type == SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT || type == SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE)
                        {

                            SchemaNode node = new SchemaNode();
                            node.FromClause = SQLDefinitionProperties.SQL_FROM_CLAUSES.INVALID;
                            SplitNodeFromSchema(splitList[0], ref node);
                            responseValue.Add(node);
                        }
                        for (int i = 0; i < splitList.Count; i++)
                        {
                            string value = splitList[i];
                            if (!string.IsNullOrEmpty(value))
                            {

                                SQLDefinitionProperties.SQL_FROM_CLAUSES fromClause = SQLDefinitionProperties.GetSqlFromClause(value);
                                if (fromClause != SQLDefinitionProperties.SQL_FROM_CLAUSES.INVALID)//next value must be an table/view/functionname/procedurename/schemaname etc. 
                                {
                                    int nextIndex = i + 1;
                                    if (nextIndex < splitList.Count)
                                    {
                                        SchemaNode node = new SchemaNode();
                                        node.FromClause = fromClause;
                                        string dbNodeName = splitList[nextIndex].Replace(";", "").Replace(" ", string.Empty);

                                        //extract the schema from node name e.g. in many query or for multiple schema joins: 
                                        // ---> SELECT c.id,c.odbc_driver_version_id,c.sqlQuery FROM rest_api.odbc_driver_version_schema_data_command as c inner join rest_api.schema_data_command as s on(s.id = c.id) WHERE c.odbc_driver_version_id = 1;
                                        // its extract the schema from tablename in this example: SCHEMA.TABLE --> rest_api.odbc_driver_version_schema_data_command, split by dot (.)
                                        SplitNodeFromSchema(dbNodeName, ref node);

                                        responseValue.Add(node);
                                    }
                                }
                            }
                        }

                    }
                }
                return responseValue;
            }
        }
        private void SplitNodeFromSchema(string dbNodeName, ref SchemaNode node)
        {
            string[] dbNodeExtractSchemaName = dbNodeName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (dbNodeExtractSchemaName.Length == 2)
            {
                node.SchemaName = dbNodeExtractSchemaName[0];
                node.NodeName = dbNodeExtractSchemaName[1];
            }
            else
            {
                node.NodeName = dbNodeName;
                node.SchemaName = null;
            }
        }
        public string Statement
        {
            get
            {
                return _statement;
            }
            private set
            {
                _statement = value;
            }
        }
        public bool IsBulkLoad
        {
            get
            {
                return _bulkerLoad;
            }
            set
            {
                _bulkerLoad = value;
            }
        }
        public string BulkFile
        {
            get
            {
                return _bulkfile;
            }
            private set
            {
                _bulkfile = value;
            }
        }
        public SQLDefinitionProperties.SQL_STATEMENT_ART StatementArt
        {
            get
            {
                return GetStatementArt(Statement);
            }
        }
        public DataTable Data
        {
            get
            {
                return _data;
            }
            set
            {
                InitStopFetching();
                _data = value;

            }
        }
        public DataTable MetaData
        {
            get
            {
                return _metaData;
            }
            set
            {
                InitStopFetching();
                _metaData = value;
            }
        }

        public bool HasSuccess
        {
            get
            {
                if (_errorCode != GeneralDefs.NotFoundResponseValue)
                    return false;

                return true;
            }
        }
        public bool HasErrors
        {
            get
            {
                if (_errorCode == GeneralDefs.NotFoundResponseValue)
                    return false;

                return true;
            }
        }
        public bool HasData
        {
            get
            {
                if (_data == null)
                    return false;

                if (_data.Rows.Count == 0)
                    return false;

                return true;
            }
        }
        public bool HasQueryParameters
        {
            get
            {
                if (_queryParametes == null)
                    return false;

                if (_queryParametes.Count == 0)
                    return false;

                return true;
            }
        }
        public List<MySqlParameter> ActiveQueryParameters
        {
            get
            {
                List<MySqlParameter> responseValues = new List<MySqlParameter>();
                if (QueryParametes != null)
                {
                    if (QueryParametes.Count != 0)
                    {
                        foreach (MySqlParameter item in QueryParametes)
                        {
                            string valueDescriptor = item.ParameterName;
                            bool findInStatement = Statement.IndexOf(valueDescriptor) == GeneralDefs.NotFoundResponseValue ?
                                false : true;
                            if (findInStatement)
                            {
                                responseValues.Add(item);
                            }
                        }
                    }
                }

                return responseValues;
            }
        }
        public bool HasActiveQueryParameters
        {
            get
            {
                return ActiveQueryParameters.Count == 0 ?
                    false : true;
            }
        }
        public bool HasNoDataOrErrors
        {
            get
            {
                return !HasData || HasErrors;
            }
        }
        public bool HasMetaData
        {
            get
            {
                if (_metaData == null)
                    return false;

                return true;
            }
        }
        public object LastInsertedId { get; set; } = Guid.Empty;
        public int Number
        {
            get
            {
                return _number;
            }
            set
            {
                InitStopFetching();
                _number = value;
            }
        }
        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                InitStopFetching();
                _errorCode = value;
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {

                InitStopFetching();
                _message = value;
            }
        }
        public override string ToString()
        {
            string tmp = Statement;
            if (QueryParametes != null)
            {
                ActiveQueryParameters.ForEach(x => tmp = tmp.Replace(x.ParameterName, x.Value.ToString()));
            }
            string query = "STATEMENT: " + tmp + "";
            return query;
        }
        public MySqlParameterCollection QueryParametes
        {
            get
            {
                return _queryParametes;
            }
            set
            {
                _queryParametes = value;
            }
        }
        public long FetchTime
        {
            get
            {
                return _fetchData.ElapsedMilliseconds;
            }
        }
        public MySqlException Exception
        {
            get
            {
                return _exceptionData;
            }
            set
            {
                _exceptionData = value;
            }
        }
        public bool HasStatementJoins
        {
            get
            {
                List<SchemaNode> tmp = StatementIncludedDbNodes.FindAll(x =>
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.JOIN ||
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.LEFT_INNER_JOIN ||
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.LEFT_JOIN ||
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.LEFT_OUTER_JOIN ||
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.RIGHT_INNER_JOIN ||
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.RIGHT_JOIN ||
                x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.RIGHT_OUTER_JOIN
                );
                return tmp.Count == 0 ? false : true;
            }
        }
        public SchemaNode PrimaryNode
        {
            get
            {
                SchemaNode responseValues = null;
                List<SchemaNode> tmp = StatementIncludedDbNodes;
                var stmnArt = StatementArt;
                if (tmp == null)
                    return responseValues;

                if (stmnArt == SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT || stmnArt == SQLDefinitionProperties.SQL_STATEMENT_ART.DELETE)
                {
                    SchemaNode lastWhere = tmp.FindLast(x => x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.WHERE);

                    int indexWhere = -1;
                    if (lastWhere != null)
                    {
                        indexWhere = tmp.IndexOf(lastWhere);
                    }
                    if (indexWhere != -1)//erstes From vor Where Clause
                    {
                        for (int i = indexWhere; i > 0; i--)
                        {
                            if (tmp[i].FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.FROM)
                            {
                                responseValues = tmp[i];
                                break;
                            }
                        }
                    }
                    else//oder letztes From in der List wenn keine Where Clause vorhanden
                    {
                        responseValues = tmp.FindLast(x => x.FromClause == SQLDefinitionProperties.SQL_FROM_CLAUSES.FROM);
                    }
                }
                else if (stmnArt == SQLDefinitionProperties.SQL_STATEMENT_ART.INSERT || stmnArt == SQLDefinitionProperties.SQL_STATEMENT_ART.UPDATE)
                {
                    if (tmp.Count != 0)
                        responseValues = tmp[0];
                }
                return responseValues;
            }
        }
        #endregion
        #region Ctor & Dtor
        public QueryResponseData(string query)
        {
            Statement = query;
            _fetchData.Start();
        }
        public QueryResponseData(bool bulkLoad, string filePath)
        {
            IsBulkLoad = bulkLoad;
            _bulkfile = filePath;
            _fetchData.Start();
        }
        ~QueryResponseData()
        {
            Dispose();
        }
        #endregion
        #region Methods
        public void SetTransaction(DbTransaction transaction)
        {
            _transaction = transaction;
        }
        public static SQLDefinitionProperties.SQL_STATEMENT_ART GetStatementArt(string statement)
        {
            if (string.IsNullOrEmpty(statement))
            {
                return SQLDefinitionProperties.SQL_STATEMENT_ART.NONE;
            }
            Array statementArtValues = Enum.GetValues(typeof(SQLDefinitionProperties.SQL_STATEMENT_ART));
            foreach (int val in statementArtValues)
            {
                SQLDefinitionProperties.SQL_STATEMENT_ART tmp = (SQLDefinitionProperties.SQL_STATEMENT_ART)val;
                string strTmp = tmp.ToString()?.ToLower();
                if (statement.ToLower().StartsWith(strTmp))
                {
                    return tmp;
                }
            }
            return SQLDefinitionProperties.SQL_STATEMENT_ART.NONE;
        }
        private void InitStopFetching()
        {
            if (_fetchData.IsRunning)
                _fetchData.Stop();
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
