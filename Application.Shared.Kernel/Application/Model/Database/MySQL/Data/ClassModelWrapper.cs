using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table;

namespace Application.Shared.Kernel.Application.Model.Database.MySQL.Data
{
    public class ClassModelWrapper : AbstractModel
    {
        public ControllerActionDescriptor Controller { get; private set; }
        public object Instance { get; }
        public Type NetType { get; }
        public ClassModel ClassModel { get; }
        public List<ClassRelationModel> Relations = new List<ClassRelationModel>();


        public List<KeyValuePair<string, string>> LinkTableMembers
        {
            get
            {
                var findVals = Relations.Select(x =>
                {
                    string responseValueTable = null;
                    string responseValueField = null;
                    if (x.EntityOne == ClassModel.TableName)
                    {
                        responseValueTable = x.EntityTwo;
                        responseValueField = x.EntityTwoKeyCol;
                    }
                    else
                    {
                        responseValueTable = x.EntityOne;
                        responseValueField = x.EntityOneKeyCol;
                    }
                    return new KeyValuePair<string, string>(responseValueTable, responseValueField);
                }).ToList();
                return ClassModel.TableName.ToLower().Contains(MySqlDefinitionProperties.TableNameRelationTableIdentifier) ? findVals : null;
            }
        }

        public List<string> Columns_Pk_Sql { get; } = new List<string>();
        public List<DatabaseColumnPropertyAttribute> Columns_WithAttributation { get; } = new List<DatabaseColumnPropertyAttribute>();
        public Dictionary<string, PropertyInfo> Columns_NetProperties { get; } = new Dictionary<string, PropertyInfo>();
        public Dictionary<string, PropertyInfo> Fk_NetProperties { get; } = new Dictionary<string, PropertyInfo>();
        public Dictionary<string, PropertyInfo> Pk_NetProperties { get; } = new Dictionary<string, PropertyInfo>();
        public Dictionary<string, string> Columns_Fk_Sql { get; } = new Dictionary<string, string>();
        public List<string> AllKeys
        {
            get

            {
                List<string> keys = new List<string>(Columns_Fk_Sql.Keys?.ToList());
                keys.AddRange(Columns_Pk_Sql);
                return keys;
            }
        }

        public List<DatabaseColumnPropertyAttribute> AllKeys_FromAttributation { get; } = new List<DatabaseColumnPropertyAttribute>();

        public ClassModelWrapper(ClassModel classModel, Type type)
        {
            ClassModel = classModel;
            NetType = type;
            Instance = GetInstance();
            Columns_WithAttributation = GetDatabaseColumns();
            Relations.ForEach((x) =>
            {
                if (x.EntityOne != ClassModel.TableName)
                {
                    Columns_Fk_Sql.Add(x.EntityOneKeyCol, x.EntityOne);
                }
                else if (x.EntityTwo != ClassModel.TableName)
                {
                    Columns_Fk_Sql.Add(x.EntityTwoKeyCol, x.EntityTwo);
                }
            });
            var properties = NetType.GetProperties()?.ToList();
            properties.ForEach(x =>
            {
                var attr = x.GetCustomAttribute<DatabaseColumnPropertyAttribute>();
                if (attr != null)
                {
                    Columns_WithAttributation.Add(attr);
                    string sqlCol = attr.ColumnName;

                    Columns_NetProperties.Add(sqlCol, x);
                    if (Columns_Fk_Sql.Keys?.ToList()?.Find(x => x == sqlCol) != null)
                    {
                        Fk_NetProperties.Add(sqlCol, x);
                        AllKeys_FromAttributation.Add(attr);

                    }
                    if (sqlCol == MySqlDefinitionProperties.GeneralUuidFieldName)
                    {
                        Pk_NetProperties.Add(sqlCol, x);
                        Columns_Pk_Sql.Add(sqlCol);
                        AllKeys_FromAttributation.Add(attr);
                    }
                }
            });

        }

        public object GetInstance()
        {
            return Activator.CreateInstance(NetType);
        }

        public void SetController(ControllerActionDescriptor controller)
        {
            Controller = controller;
        }

        public bool IsKeyColumn(DatabaseColumnPropertyAttribute databaseColumnPropertyAttribute)
        {
            string col = databaseColumnPropertyAttribute.ColumnName;
            return AllKeys.Find(x => x == col) != null;
        }
        public override string ToString()
        {
            return ClassModel.TableName;
        }

    }
}
