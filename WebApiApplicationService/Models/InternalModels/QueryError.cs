using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WebApiApplicationService.InternalModels
{
    public class QueryError : IDisposable
    {
        #region Private
        private string _query = null;
        #endregion
        #region Public
        public string Query
        {
            get
            {
                return _query;
            }
            set
            {
                _query = value;
            }
        }
        public string Message { get; set; }
        public string Level { get; set; }
        public int Code { get; set; }
        public bool HasErrorData
        {
            get
            {
                return this.Message == null ? false : true;
            }
        }
        #endregion
        #region Ctor & Dtor
        public QueryError()
        {

        }
        ~QueryError()
        {

        }
        #endregion
        #region Methods
        public void FillDataFromOdbcError(string query, MySqlError error)
        {
            this.Query = query;
            if (error != null)
            {
                this.Message = error.Message;
                this.Code = error.Code;
                this.Level = error.Level;
            }
        }
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
