using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using WebApiApplicationService.Attribute;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace WebApiApplicationService
{

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DatabaseColumnPropertyAttribute : System.Attribute
    {

        #region Private
        private DatabaseAttributeManager _databaseAttributeManager = new DatabaseAttributeManager();
        private string _columnName = null;
        private object _notSetValue = null;
        private MySqlDbType _dbType = MySqlDbType.String;
        #endregion Private
        #region Public
        #endregion Public
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
        }
        public MySqlDbType DataType
        {
            get
            {
                return _dbType;
            }
        }
        public object NotSetValue
        {
            get
            {
                return _notSetValue;
            }
        }
        public bool MandantoryFieldInQueryGenerating { get; private set; }

        #region Ctor & Dtor
        public DatabaseColumnPropertyAttribute(string columnName, MySqlDbType type, object notSetValue = null)
        {
            Init(columnName, type, false, notSetValue);
        }
        #endregion Ctor & Dtor
        #region Methods

        public void Init(string columnName, MySqlDbType type,bool mandantoryInQueryGenerating, object notSetValue = null)
        {
            _columnName = columnName;
            _dbType = type;
            _notSetValue = notSetValue;
            this.MandantoryFieldInQueryGenerating = mandantoryInQueryGenerating;
        }
        public override string ToString()
        {
            string responseValue = string.Format("Class: {0},Class-Property-Values=({1})",this.GetType().ToString(),_columnName);

            return responseValue;

        }
        #endregion Methods
    }
}
