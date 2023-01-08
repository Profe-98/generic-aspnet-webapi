﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebApiApplicationService.Handler;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApiApplicationService.Models
{
    [ValidateNever]
    [Serializable]
    public class ApiRootNodeModel : JsonApiDataHandler, IDisposable
    {
        #region Private
        private ApiMetaModel _meta = null;
        private object _data = null;
        private int _maxItemsPerPage = BackendAPIDefinitionsProperties.PaginationItemPerPage;
        #endregion
        #region Public
        [JsonIgnore]
        public static readonly ApiInformationModel ApiInformation = new ApiInformationModel
        {
            Author = "Joel Mika Roos for HelixDBM GmbH",
            Company = "HelixDBM GmbH",
            Copyright = "HelixDBM GmbH",
            Rfc = "RFC 7159",
            Use = "Backend API for Helix DMBS Tool and the Official Helix DBM Website",
            Version = "1.0"
        };
        [JsonIgnore]
        public Type[] NotExportedTypes { get; set; }    
        [JsonIgnore]
        public string RootModelType
        {
            get
            {
                string response = null;
                if(this.RootNodes != null)
                {
                    response = this.RootNodes[0] != null?this.RootNodes[0].Type:null;
                }
                return response;
            }
        }
        [JsonIgnore]
        public int MaxItemsPerPage
        {
            get { return _maxItemsPerPage; }
            set { _maxItemsPerPage = value <= 0?BackendAPIDefinitionsProperties.PaginationItemPerPage: value; }
        }
        //Data as a single object of Type ApiDataModel or List<ApiDataModel>
        [JsonPropertyName("data")]
        public object Data 
        { 
            get
            {

                return _data;

            }
            set
            {
                if (value != null)
                {
                    Type t = value.GetType();
                    if (t == typeof(JsonElement))
                    {
                        JsonElement jsonElement = (JsonElement)value;
                        using (JsonHandler _jsonHandler = new JsonHandler(true))
                        {
                            Type conversionType = null;
                            switch(jsonElement.ValueKind)
                            {
                                case JsonValueKind.Array:
                                    conversionType = typeof(List<ApiDataModel>);
                                    break;
                                case JsonValueKind.Object:
                                    conversionType = typeof(ApiDataModel);
                                    break;
                                default:
                                    conversionType = typeof(ApiDataModel);
                                    break;
                            }
                            string jsonStr = jsonElement.GetRawText().Replace("\n", "").Replace("\r", ""); ;
                            value = _jsonHandler.JsonDeserialize(jsonStr, conversionType) ;
                        }
                    }
                }
                _data = value;
            }
        }
        [JsonPropertyName("errors")]
        public List<ApiErrorModel> Errors { get; set; }

        [JsonPropertyName("meta")]
        public ApiMetaModel Meta
        {
            get
            {
                if(_meta == null)
                {
                    if(Data == null)
                    {
                        _meta = new ApiMetaModel();
                        _meta.Count = 0;
                    }
                    else
                    {
                        if (IsDataApiDataModelList)
                        {
                            _meta = new ApiMetaModel();
                            _meta.Count = ((List<ApiDataModel>)Data).Count;
                        }
                        else
                        {
                            _meta = new ApiMetaModel();
                            _meta.Count = 1;
                        }
                    }
                }
                return _meta;
            }
            set
            {
                _meta = value;
            }
        }
        [JsonPropertyName("jsonapi")]
        public ApiInformationModel Jsonapi { get; set; } = ApiRootNodeModel.GetApiInformation();//api information meta
        [JsonIgnore]
        public List<ApiDataModel> RootNodes
        {
            get
            {
                if (Utils.IsList(this.Data))
                {
                    return (List<ApiDataModel>)this.Data;
                }
                else
                {
                    return new List<ApiDataModel>() { (ApiDataModel)this.Data };
                }
            }
        }
        [JsonIgnore]
        public bool HasErrors
        {
            get
            {
                return this.Errors != null && this.Errors.Count != 0;
            }
        }
        [JsonIgnore]
        public bool HasAnyData
        {
            get
            {
                return this.Data != null;
            }
        }
        [JsonIgnore]
        public bool IsDataApiDataModelList
        {
            get
            {
                return Utils.IsList<ApiDataModel>(this.Data);
            }
        }
        [JsonIgnore]
        public bool IsDataApiDataModel
        {
            get
            {
                return !Utils.IsList<ApiDataModel>(this.Data) && this.Data != null && this.Data.GetType() == typeof(ApiDataModel);
            }
        }

        [JsonIgnore]
        public Dictionary<int,List<ApiDataModel>> PaginatedDataList
        {
            get
            {
                Dictionary<int, List<ApiDataModel>> responseValue = null;
                List<ApiDataModel> allItems = null;
                if (IsDataApiDataModelList)
                {
                    allItems = (List<ApiDataModel>)Data;
                    
                }
                else if(HasAnyData)
                {
                    allItems = new List<ApiDataModel>() { (ApiDataModel)Data };
                }
                if(allItems != null)
                {
                    responseValue = new Dictionary<int, List<ApiDataModel>>();
                    int pageIndex = 0;
                    int pageItemOffset = 0;
                    for (int i = 0; i < allItems.Count; i++, pageItemOffset++)
                    {

                        if (pageItemOffset >= this.MaxItemsPerPage)
                        {
                            pageItemOffset = 0;

                            pageIndex++;
                        }

                        if (!responseValue.ContainsKey(pageIndex))
                        {
                            responseValue.Add(pageIndex, new List<ApiDataModel>());
                        }
                        responseValue[pageIndex].Add(allItems[i]);
                    }
                }
                return responseValue;
            }
        }
        #endregion

        #region Ctor
        public ApiRootNodeModel()
        {

        }
        public ApiRootNodeModel(IScopedDatabaseHandler databaseHandler, IScopedJsonHandler jsonHandler, IAuthHandler authHandler, ICachingHandler cachingHandler) : 
            base(databaseHandler, jsonHandler, authHandler, cachingHandler)
        {

        }
        #endregion
        #region Methods

        public static ApiInformationModel GetApiInformation()
        {
            return ApiInformation;
        }
        public static ApiRootNodeModel PrepareErrorResponse(object data,List<ApiErrorModel> errors,string optionalMetaMsg = null)
        {
            return new ApiRootNodeModel()
            {
                Data = data,
                Errors = errors,
                Meta = new ApiMetaModel
                {
                    Count = errors != null?errors.Count:0,
                    OptionalMessage = optionalMetaMsg
                },
                Jsonapi = ApiInformation
            };
        }
        public Dictionary<int,List<ApiDataModel>> GetPaginatedRange(int start,int end = GeneralDefs.NotFoundResponseValue)
        {
            Dictionary<int, List<ApiDataModel>> allPages = PaginatedDataList;
            if(start > end && end != GeneralDefs.NotFoundResponseValue)
            {
                int tmp = end;
                end = start;
                start = end;
            }
            if(start < 0)
            {
                return null;
            }
            if(end == GeneralDefs.NotFoundResponseValue)
            {
                end = allPages.Keys.ToList().Last();//wenn kein End angegeben wird das Ende gleich dem letzten Key in Dictionary gesetzt
            }
            if(allPages.Keys.Count != 0)
            {
                List<int> itemInGivenRange = allPages.Keys.ToList().FindAll(x => x <= end && x >= start);
                int existItemInGivenRange = itemInGivenRange.Count;
                Dictionary<int, List<ApiDataModel>> tmp = new Dictionary<int, List<ApiDataModel>>();
                if (existItemInGivenRange != 0)
                {
                    foreach (int key in itemInGivenRange)
                    {
                        tmp.Add(key,allPages[key]);
                    }
                }
                return tmp;
            }
            return null;
        }

        public void Dispose()
        {
             GC.SuppressFinalize(this);
        }
        #endregion
    }
}
