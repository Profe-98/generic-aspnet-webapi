using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using WebApiFunction.Application.Model.Internal;
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
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Web.AspNet.Filter;
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
using WebApiFunction.Application.Model.Database.MySQL.Table;

namespace WebApiFunction.Application.Model.Database.MySQL.Data
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
                return ClassModel.TableName.ToLower().Contains(SQLDefinitionProperties.TableNameRelationTableIdentifier) ? findVals : null;
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
                    if (sqlCol == SQLDefinitionProperties.GeneralUuidFieldName)
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
