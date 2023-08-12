using Dapper;
using Microsoft.AspNetCore.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration;
using Application.Shared.Kernel.Infrastructure.Database;

namespace Application.Shared.Kernel.Application.Model.Dapper.TypeMapper
{
    public static class DataMapperForDapperExtension
    {
        public static void UseCustomDataMapperForDapper(string[] entityNamespace)
        {
            List<SqlMapper.ITypeMap> mappers = new List<SqlMapper.ITypeMap>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var classesFromNameSpace = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.IsClass && entityNamespace.ToList().IndexOf(t.Namespace) != -1).ToList();
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
