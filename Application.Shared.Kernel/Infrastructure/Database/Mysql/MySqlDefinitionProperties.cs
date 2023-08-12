using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Data;

namespace Application.Shared.Kernel.Infrastructure.Database.Mysql
{
    public static class MySqlDefinitionProperties
    {

        public static Dictionary<MySqlDbType, Type> NetTypeMySqlDbTypeMapping = new Dictionary<MySqlDbType, Type>()
        {
            { MySqlDbType.Decimal,typeof(decimal) },
            { MySqlDbType.Byte,typeof(byte) },
            { MySqlDbType.Int16,typeof(int) },
            { MySqlDbType.Int24,typeof(int) },
            { MySqlDbType.Int32 ,typeof(int)},
            { MySqlDbType.Int64,typeof(int) },
            { MySqlDbType.Float,typeof(float) },
            { MySqlDbType.Double,typeof(double) },
            { MySqlDbType.Timestamp,typeof(TimeSpan) },
            { MySqlDbType.Date,typeof(DateOnly) },
            { MySqlDbType.Time ,typeof(TimeSpan)},
            { MySqlDbType.DateTime,typeof(DateTime) },
            { MySqlDbType.Year,typeof(int) },
            { MySqlDbType.Newdate,typeof(DateTime) },
            { MySqlDbType.VarString,typeof(string) },
            { MySqlDbType.Bit,typeof(bool) },
            { MySqlDbType.JSON,typeof(string) },
            { MySqlDbType.NewDecimal,typeof(decimal) },
            { MySqlDbType.Enum,typeof(string) },
            { MySqlDbType.Set,typeof(string) },
            { MySqlDbType.TinyBlob,typeof(byte[]) },
            { MySqlDbType.MediumBlob,typeof(byte[]) },
            { MySqlDbType.LongBlob,typeof(byte[]) },
            { MySqlDbType.Blob,typeof(byte[]) },
            { MySqlDbType.VarChar ,typeof(string)},
            { MySqlDbType.String ,typeof(string)},
            { MySqlDbType.Geometry ,typeof(byte[])},
            { MySqlDbType.UByte,typeof(byte) },
            { MySqlDbType.UInt16 ,typeof(ushort)},
            { MySqlDbType.UInt24 ,typeof(uint)},
            { MySqlDbType.UInt32 ,typeof(uint)},
            { MySqlDbType.UInt64 ,typeof(ulong)},
            { MySqlDbType.Binary ,typeof(byte[])},
            { MySqlDbType.VarBinary,typeof(byte[]) },
            { MySqlDbType.TinyText,typeof(string) },
            { MySqlDbType.MediumText ,typeof(string)},
            { MySqlDbType.LongText,typeof(string) },
            { MySqlDbType.Text,typeof(string) },
            { MySqlDbType.Guid,typeof(Guid) },
        };
        public static Dictionary<Type, MySqlDbType> MySqlDbTypeNetTypeMapping = new Dictionary<Type, MySqlDbType>()
        {
            { typeof(string),MySqlDbType.String },
            { typeof(short),MySqlDbType.Int16 },
            { typeof(int),MySqlDbType.Int32 },
            { typeof(long),MySqlDbType.Int64 },
            { typeof(ushort),MySqlDbType.Int16 },
            { typeof(uint),MySqlDbType.UInt32 },
            { typeof(ulong),MySqlDbType.UInt64 },
            { typeof(float),MySqlDbType.Float },
            { typeof(decimal),MySqlDbType.Decimal },
            { typeof(double),MySqlDbType.Double },
            { typeof(TimeSpan),MySqlDbType.Time },
            { typeof(DateTime),MySqlDbType.DateTime },
            { typeof(Guid),MySqlDbType.Guid },
            { typeof(byte[]),MySqlDbType.Blob },
            { typeof(byte),MySqlDbType.Byte },
            { typeof(DateOnly),MySqlDbType.Date },
            { typeof(bool),MySqlDbType.Bit },
            { typeof(char),MySqlDbType.VarChar },
        };
        public static Dictionary<string, ClassModelWrapper> BackendTablesEx = new Dictionary<string, ClassModelWrapper>();
        public enum SQL_KEY_ART : int
        {
            NONE = 0,
            PRIMARY = 1,
            FOREIGN_KEY = 2,
            PRIMARY_AND_FOREIGN_KEY = 3
        }
        public enum SQL_FROM_CLAUSES : int
        {
            INVALID,
            FROM,
            JOIN,
            LEFT_JOIN,
            RIGHT_JOIN,
            LEFT_OUTER_JOIN,
            RIGHT_OUTER_JOIN,
            LEFT_INNER_JOIN,
            RIGHT_INNER_JOIN,
            WHERE
        }
        public enum SQL_STATEMENT_ART : int
        {
            NONE = 0,
            INSERT = 1,
            UPDATE = 2,
            DELETE = 3,
            SELECT = 4
        }

        /*public enum SQL_CARDINALITY : int
        {
            NONE = 0,
            OnetoOne = 1,
            OneToN = 2,
            MtoN = 3
        }*/

        public const string GeneralUuidFieldName = "uuid";
        public const string NULLValueStr = "null";

        public static readonly Dictionary<SQL_FROM_CLAUSES, string> SqlFromClauses = new Dictionary<SQL_FROM_CLAUSES, string>()
        {
            { SQL_FROM_CLAUSES.FROM,"FROM" },
            { SQL_FROM_CLAUSES.LEFT_INNER_JOIN,"LEFT INNER JOIN" },
            { SQL_FROM_CLAUSES.LEFT_OUTER_JOIN,"LEFT OUTER JOIN" },
            { SQL_FROM_CLAUSES.LEFT_JOIN,"LEFT JOIN" },
            { SQL_FROM_CLAUSES.RIGHT_INNER_JOIN,"RIGHT INNER JOIN" },
            { SQL_FROM_CLAUSES.RIGHT_OUTER_JOIN,"RIGHT OUTER JOIN" },
            { SQL_FROM_CLAUSES.RIGHT_JOIN,"RIGHT JOIN" },
            { SQL_FROM_CLAUSES.JOIN,"JOIN" },
        };
        public const string TableNameRelationTableIdentifier = "relation_to";
        public const string SqlSchemaDataCommandValueWildcard = "{!#}";//replace wildcard in schema_data_command sql queries

        #region Ctor & Dtor
        static MySqlDefinitionProperties()
        {
        }
        #endregion
        #region Methods
        public static Type GetBackendTableByTypeStr(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;
            if (BackendTablesEx.Keys.Count != 0)
            {
                ClassModelWrapper model = BackendTablesEx.Values.ToList().Find(x => x.ClassModel.NetName.ToLower() == typeName.ToLower());

                if (model != null)
                {
                    return model.NetType;
                }
            }
            return null;
        }
        public static SQL_FROM_CLAUSES GetSqlFromClause(string value)
        {
            foreach (SQL_FROM_CLAUSES key in SqlFromClauses.Keys)
            {
                string keyValue = SqlFromClauses[key];
                if (keyValue.ToUpper().Equals(value.ToUpper()))
                {
                    return key;
                }
            }
            return SQL_FROM_CLAUSES.INVALID;
        }
        public static MySqlDbType GetMySqlDbTypeFromNetType(Type type)
        {
            if (MySqlDbTypeNetTypeMapping.ContainsKey(type))
            {
                return MySqlDbTypeNetTypeMapping[type];
            }
            return MySqlDbType.String;
        }
        #endregion Methods
    }
}
