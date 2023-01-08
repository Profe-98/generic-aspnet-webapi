﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
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
using WebApiFunction.Healthcheck;
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

namespace WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1
{
    public class JsonApiDataHandler : IJsonApiDataHandler
    {
        #region Private
        private readonly IScopedJsonHandler _jsonHandler = null;
        private readonly IScopedDatabaseHandler _db = null;
        private readonly IAuthHandler _authHandler = null;
        private readonly ICachingHandler _cachingHandler = null;
        #endregion Private 
        #region Public
        #endregion Public
        #region Ctor
        public JsonApiDataHandler()
        {

        }
        public JsonApiDataHandler(IScopedDatabaseHandler databaseHandler, IScopedJsonHandler jsonHandler, IAuthHandler authHandler, ICachingHandler cachingHandler)
        {
            _jsonHandler = jsonHandler;
            _db = databaseHandler;
            _authHandler = authHandler;
            _cachingHandler = cachingHandler;
        }
        #endregion Ctor
        #region Methods
        /// <summary>
        /// Gets the ApiRootNodeModel from JsonDocument, additional it convert the Data of ApiRootNodeModel to List<ApiDataModel> for better internal processing to one declared type
        /// </summary>
        /// <param name="jsonDocument">The json document object with an included ApiRootNodeModel/param>
        /// <returns>ApiRootNodeModel</returns>
        public ApiRootNodeModel GetFromJsonBody(JsonDocument jsonDocument)
        {
            ApiRootNodeModel model = jsonDocument != null ?
                _jsonHandler.JsonDeserialize<ApiRootNodeModel>(jsonDocument.RootElement.GetRawText()) : null;
            if (model != null)
            {
                List<ApiDataModel> bodyValues = new List<ApiDataModel>();
                if (Utils.IsList<ApiDataModel>(model.Data))
                {
                    bodyValues = (List<ApiDataModel>)model.Data;
                }
                else
                {
                    if (model.Data != null)
                    {
                        bodyValues.Add((ApiDataModel)model.Data);
                    }
                }
                model.Data = bodyValues;
            }
            return model;
        }
        public async Task<ApiRootNodeModel> CreateApiRootNodeFromModel<T>(string area, List<T> data, int maxDepth = 1, List<JsonApiTreeSearchFilterModel> jsonApiTreeSearchFilterModel = null) where T : AbstractModel
        {
            ApiRootNodeModel apiRootNodeModel = new ApiRootNodeModel(_db, _jsonHandler, _authHandler, _cachingHandler);

            List<ApiDataModel> apiDataModels = new List<ApiDataModel>();

            apiDataModels = await GetConvertedFromModel<T>(area, data, true, 0, maxDepth,jsonApiTreeSearchFilterModel);

            apiRootNodeModel.Jsonapi = ApiRootNodeModel.GetApiInformation();
            apiRootNodeModel.Data = apiDataModels.Count == 1 ? apiDataModels[0] : apiDataModels;
            apiRootNodeModel.Meta = new ApiMetaModel
            {
                Count = apiDataModels.Count
            };
            return apiRootNodeModel;

        }
        public async Task<ApiRootNodeModel> CreateApiRootNodeFromApiData<T>(string area, List<T> data) where T : ApiDataModel
        {
            if (_db == null)
                throw new InvalidOperationException();

            ApiRootNodeModel apiRootNodeModel = new ApiRootNodeModel(_db, _jsonHandler, _authHandler, _cachingHandler);

            List<T> apiDataModels = new List<T>();

            apiDataModels = GetConvertedFromApiData<T>(area, data);

            apiRootNodeModel.Jsonapi = ApiRootNodeModel.GetApiInformation();
            apiRootNodeModel.Data = apiDataModels.Count == 1 ? apiDataModels[0] : apiDataModels;
            apiRootNodeModel.Meta = new ApiMetaModel
            {
                Count = apiDataModels.Count
            };
            return apiRootNodeModel;

        }
        public async Task<List<ApiDataModel>> GetConvertedFromModel<T>(string area, List<T> data, bool relatedObject, int depth = 0, int maxDepth = 1, List<JsonApiTreeSearchFilterModel> jsonApiTreeSearchFilterModel = null) where T : AbstractModel
        {
            List<ApiDataModel> apiDataModels = new List<ApiDataModel>();
            if (Utils.IsList<T>(data))
            {
                List<T> abstractModels = data;
                int i = 0;
                foreach (T datam in abstractModels)
                {
                    if (datam.Uuid == Guid.Empty)
                        continue;

                    ApiDataModel apiDataModel = new ApiDataModel();

                    apiDataModel.Id = datam.Uuid;
                    apiDataModel.Type = datam.GetType().Name;
                    apiDataModel.NodeDepth = depth;

                    if (relatedObject && datam.DatabaseRelations != null && datam.DatabaseRelations.Count != 0)
                    {
                        var rel = await GetRelations(datam, area, depth, maxDepth,jsonApiTreeSearchFilterModel);
                        if (rel.Count != 0)
                            apiDataModel.RelationshipsInternal.AddRange(rel);
                    }
                    apiDataModel.Attributes = datam;


                    apiDataModel.Meta = new ApiMetaModel { Count = apiDataModel.RelationshipsInternal == null ? 0 : apiDataModel.RelationshipsInternal.Count };

                    apiDataModels.Add(apiDataModel);
                    i++;
                }
            }
            else
            {
                T item = data[0];
                ApiDataModel dataModel = new ApiDataModel();
                dataModel.Id = item.Uuid;
                dataModel.Type = item.GetType().Name;
                dataModel.Attributes = item;
                apiDataModels.Add(dataModel);
            }
            return apiDataModels;
        }
        public object GetForeignObject<T>(T datam, ClassRelationModel rel)
            where T : AbstractModel
        {

            string type = null;
            ClassModelWrapper classModelWrapperRel = null;
            ClassModelWrapper classModelWrapperCurrentNode = null;
            Dictionary<DatabaseColumnPropertyAttribute, DatabaseColumnPropertyAttribute> keyValuePairs = new Dictionary<DatabaseColumnPropertyAttribute, DatabaseColumnPropertyAttribute>();
            if (rel.EntityOne == datam.DatabaseTable)
            {

                type = rel.EntityTwo;
                classModelWrapperRel = SQLDefinitionProperties.BackendTablesEx[rel.EntityTwo];
                classModelWrapperCurrentNode = SQLDefinitionProperties.BackendTablesEx[rel.EntityOne];

                var fAttr = classModelWrapperRel.Columns_WithAttributation.Find(x => x.ColumnName == rel.EntityTwoKeyCol);
                var pAttr = classModelWrapperCurrentNode.Columns_WithAttributation.Find(x => x.ColumnName == rel.EntityOneKeyCol);
                keyValuePairs.Add(pAttr, fAttr);
            }
            else if (rel.EntityTwo == datam.DatabaseTable)
            {
                type = rel.EntityOne;
                classModelWrapperCurrentNode = SQLDefinitionProperties.BackendTablesEx[rel.EntityTwo];
                classModelWrapperRel = SQLDefinitionProperties.BackendTablesEx[rel.EntityOne];
                var fAttr = classModelWrapperRel.Columns_WithAttributation.Find(x => x.ColumnName == rel.EntityOneKeyCol);
                var pAttr = classModelWrapperCurrentNode.Columns_WithAttributation.Find(x => x.ColumnName == rel.EntityTwoKeyCol);
                keyValuePairs.Add(pAttr, fAttr);
            }

            object instanceRel = (AbstractModel)classModelWrapperRel.GetInstance();
            foreach (DatabaseColumnPropertyAttribute key in keyValuePairs.Keys)
            {
                object valueParent = datam.GetPropertyValueByColumnName(datam, key);
                instanceRel = ((AbstractModel)instanceRel).SetPropertyValueByColumnName(instanceRel, keyValuePairs[key], valueParent);
            }
            return instanceRel;
        }
        public async Task<List<object>> GetorSetCacheData(object model)
        {


            string key = ((AbstractModel)model).DatabaseTable;
            string json = await _cachingHandler.GetStringAsync(key);
            var query = ((AbstractModel)model).GenerateQuery(SQLDefinitionProperties.SQL_STATEMENT_ART.SELECT, model);
            string selectAllStatement = "SELECT * FROM " + key + "";
            List<object> resultSet = new List<object>();
            QueryResponseData<object> queryResponseData = null;
        JUMPER:
            if (json == null)
            {

                queryResponseData = await _db.ExecuteQueryWithMap(selectAllStatement, model, model.GetType());
                if (queryResponseData.HasData)
                {

                    json = _jsonHandler.JsonSerialize(queryResponseData.DataStorage);
                    await _cachingHandler.SetStringAsync(key, json, distributedCacheEntryOptions: new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 10, 0) });

                }
                else
                {
                    return null;
                }
            }
            List<bool> regexMatches = new List<bool>();
            foreach (var item in query.WhereClauseValues)
            {
                string pattern = "(\"" + item.Key + "\":[^\"]*\"" + item.Value + "\")";
                regexMatches.Add(Regex.Match(json, pattern, RegexOptions.IgnoreCase).Success);
            }
            if (regexMatches.Count == query.WhereClauseValues.Keys.Count)
            {
                Type type = typeof(List<>);
                type = type.MakeGenericType(model.GetType());
                var tmpObj = (List<object>)_jsonHandler.JsonDeserialize(json, typeof(List<object>));
                foreach (var item in tmpObj)
                {
                    var tt = item.GetType();
                    JsonElement itemValue = (JsonElement)item;
                    object val = itemValue.Deserialize(model.GetType());
                    var propertyInfos = val.GetType().GetProperties();
                    List<bool> whereClauseChecks = new List<bool>(query.WhereClauseValues.Keys.Count);
                    foreach (var propertyInfo in propertyInfos)
                    {
                        var attr = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
                        if (attr != null && query.WhereClauseValues.ContainsKey(attr.Name))
                        {
                            object valueWhere = query.WhereClauseValues[attr.Name];
                            object valueObj = propertyInfo.GetValue(val);
                            whereClauseChecks.Add(valueWhere.ToString() == valueObj.ToString());
                        }
                    }
                    if (whereClauseChecks.FindAll(x => x).Count() == query.WhereClauseValues.Keys.Count)
                    {

                        resultSet.Add(val);
                    }
                }
            }
            else
            {
                json = null;
                goto JUMPER;
            }
            return resultSet;
        }
        public async Task<List<ApiDataModel>> GetRelations<T>(T datam, string area, int depth, int maxDepth, List<JsonApiTreeSearchFilterModel> jsonApiTreeSearchFilterModel = null)
            where T : AbstractModel
        {
            var relationshipsInternal = new List<ApiDataModel>();

            if (datam.DatabaseRelations == null || datam.DatabaseRelations.Count == 0)
                return relationshipsInternal;

            depth++;
            JsonApiTreeSearchFilterModel foundJsonApiTreeSearchFilterModel = null;
            foreach (var rel in datam.DatabaseRelations)
            {
                bool iterationContinues = false;
                if(jsonApiTreeSearchFilterModel != null&&jsonApiTreeSearchFilterModel.Count != 0)
                {
                    string compareName = rel.fGenerateNetClassNameFromTableName(rel.EntityOne == datam.DatabaseTable ? rel.EntityTwo : rel.EntityOne).ToLower();

                    var findFilter = jsonApiTreeSearchFilterModel.Find(x => x.EntityName.Equals(compareName));
                    if (findFilter != null)
                    {
                        iterationContinues = true;
                        foundJsonApiTreeSearchFilterModel = findFilter;
                    }
                }
                else
                {
                    iterationContinues = true;
                }
                if(iterationContinues)
                {

                    object instanceRel = (AbstractModel)GetForeignObject<T>(datam, rel);
                    var resultSet = await GetorSetCacheData(instanceRel);
                    if (resultSet.Count != 0)
                    {

                        Type currentInstance = GetType();
                        MethodInfo methodConvertedModel = currentInstance.GetMethod("GetConvertedFromModel");
                        methodConvertedModel = methodConvertedModel.MakeGenericMethod(typeof(AbstractModel));
                        var t = resultSet.
                            Select(item => Convert.ChangeType(item, instanceRel.GetType())).
                            OfType<AbstractModel>().
                            ToList();

                        List<JsonApiTreeSearchFilterModel> searchFilterModels = null;
                        if(foundJsonApiTreeSearchFilterModel !=null)
                        {
                            searchFilterModels = foundJsonApiTreeSearchFilterModel.FilterRelationNames;
                        }
                        bool relationSearchParam = depth < maxDepth ? true : false;
                        var methodInvokeParams = new object[] { area, t, relationSearchParam, depth, maxDepth, searchFilterModels };
                        var lenMethodParams = methodConvertedModel.GetParameters()?.Length;
                        if (methodInvokeParams.Length != lenMethodParams)
                        {
                            throw new InvalidOperationException("param count from var 'methodInvokeParams' is not the same size like required invoke method need, params ("+ methodInvokeParams + "!="+lenMethodParams+")");
                        }
                        var apiRelationShipModels = (Task<List<ApiDataModel>>)methodConvertedModel.Invoke(this, methodInvokeParams); ;

                        relationshipsInternal.AddRange(await apiRelationShipModels);
                    }
                }
            }
            return relationshipsInternal;
        }
        public List<T> GetConvertedFromApiData<T>(string area, List<T> data) where T : ApiDataModel
        {
            List<T> apiDataModels = new List<T>();
            Type type = data.GetType();
            if (Utils.IsList<T>(data))
            {
                apiDataModels = data;
                int i = 0;
                foreach (ApiDataModel apiData in apiDataModels)
                {
                    Type t = SQLDefinitionProperties.GetBackendTableByTypeStr(apiData.Type);
                    if (t != null)
                    {
                        string attrJson = apiData.Attributes.ToString();
                        object tmpobj = _jsonHandler.JsonDeserialize(attrJson, t);
                        apiData.Attributes = tmpobj;
                        apiDataModels[i].Attributes = tmpobj;
                    }
                    i++;
                }
            }
            return apiDataModels;
        }
        #endregion Methods
    }
}
