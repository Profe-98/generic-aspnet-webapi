using Dapper;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Configuration;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;

namespace WebApiFunction.Application.Model.Database.MySql.Dapper.TypeMapper
{
    public static class DataMapperForDapperExtension
    {
        public static void UseCustomDataMapperForDapper()
        {
            List<SqlMapper.ITypeMap> mappers = new List<SqlMapper.ITypeMap>();
            var classesFromNameSpace = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && t.Namespace == BackendAPIDefinitionsProperties.DatabaseModelNamespace).ToList();
            foreach (var type in classesFromNameSpace)
            {
                var typeMap = new CustomPropertyTypeMap(type, (type2, name) =>
                {
                    var t = type2.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(prop =>
                        prop.GetCustomAttributes()
                            .OfType<DatabaseColumnPropertyAttribute>()
                            .Any(attr => attr.ColumnName == name)
                        );
                    return t;
                });
                SqlMapper.SetTypeMap(type, typeMap);
            }

        }
    }
}
