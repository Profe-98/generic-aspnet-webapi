using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebApiFunction.Data.Format.Json;
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

namespace WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1
{
    [ValidateNever]
    [Serializable]
    public class ApiDataModel : BaseModel
    {
        #region Private
        private ApiDataModel _parent = null;
        private int _maxDepth = GeneralDefs.NotFoundResponseValue;
        private bool _linkVisibility = true;
        private ApiLinkModel _link = null;
        private string _type = null;
        private Guid _relationIndexer = Guid.Empty;
        private bool _attributeVisibility = true;
        private bool _includeVisibility = true;
        private bool _relationsVisibility = true;
        private object _attributes = null;
        private int _nodeDepth = GeneralDefs.NotFoundResponseValue;
        private Dictionary<string, List<ApiRelationshipModel>> _relationsWithoutAttributes = new Dictionary<string, List<ApiRelationshipModel>>();
        private Dictionary<string, List<ApiRelationshipModel>> _relationsWithoutAttributesParent = new Dictionary<string, List<ApiRelationshipModel>>();
        private List<ApiDataModel> _includesWithAttributes = new List<ApiDataModel>();
        private List<ApiDataModel> _includesWithAttributesParent = new List<ApiDataModel>();
        private List<ApiDataModel> _relations = new List<ApiDataModel>();
        private List<ApiDataModel> _relationsParentDirection = new List<ApiDataModel>();
        #endregion
        #region Public
        [JsonPropertyName("id")]
        public Guid Id { get; set; }//id von objekt
        [JsonPropertyName("type")]
        public string Type
        {
            get
            {
                return string.IsNullOrEmpty(_type) ?
                    null : _type.ToLower();
            }
            set
            {
                _type = string.IsNullOrEmpty(value) ?
                    null : value.ToLower();
            }
        }//type von den attributes

        [JsonIgnore]
        public bool OnlyRelation { get; set; } = false;

        /*[JsonIgnore]
        public bool HideJsonRelation { get; set; } = false;*/

        [JsonIgnore]
        public Type NetType
        {
            get
            {
                return Type == null ?
                    null : SQLDefinitionProperties.GetBackendTableByTypeStr(Type);
            }
        }
        [JsonIgnore]
        public object Instance
        {
            get
            {
                return Activator.CreateInstance(NetType);
            }
        }
        [JsonPropertyName("attributes")]
        public object Attributes
        {
            get
            {
                return _attributeVisibility ?
                    _attributes : null;
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
                            if (NetType != null)
                            {
                                Type conversionType = NetType;
                                string jsonStr = jsonElement.GetRawText().Replace("\n", "").Replace("\r", ""); ;
                                value = _jsonHandler.JsonDeserialize(jsonStr, conversionType);
                            }
                            else
                            {
                                throw new NotSupportedException("Unknown type");
                            }
                        }
                    }
                }
                _attributes = value;
            }
        }
        //attribute von klasse die den attributes mitgegeben wird
        [JsonPropertyName("links")]
        public ApiLinkModel Links
        {
            get
            {
                if (_link == null || _link.Self == null)
                {
                    _link = new ApiLinkModel();
                    _link.Self = Attributes == null ?
                        "" : "/" + AreaParent + "/" + Type.Replace("model", "") + "/" + (Id == Guid.Empty ? string.Empty : Id) + "";
                }
                return _linkVisibility ?
                    _link : null;
            }
            set
            {

                _link = value;
            }
        }//links zum objekt oder in einer list zum nächsten

        [JsonPropertyName("depth")]
        public int NodeDepth
        {
            get
            {
                return _nodeDepth;
            }
            set
            {
                _nodeDepth = value;
            }
        }
        [JsonIgnore]
        public string AreaParent
        {
            get
            {
                return GeneralDefs.ApiAreaV1;
            }
        }
        [JsonIgnore]
        public int MaxDisplayNodeDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                _maxDepth = value;
            }
        }
        [JsonIgnore]
        public ApiDataModel Parent
        {
            get
            {
                ApiDataModel apiDataModel = _parent;
                return apiDataModel;
            }
            set
            {
                _parent = value;
            }
        }

        [JsonIgnore]
        public bool AttributeVisibility
        {
            get
            {
                return _attributeVisibility;
            }
            set
            {
                _attributeVisibility = value;
            }
        }
        [JsonIgnore]
        public bool LinkVisbility
        {
            get
            {
                return _linkVisibility;
            }
            set
            {
                _linkVisibility = value;
            }
        }
        [JsonIgnore]
        public bool IncludeVisibility
        {
            get
            {
                return _includeVisibility;
            }
            set
            {
                _includeVisibility = value;
            }
        }
        [JsonIgnore]
        public bool RelationsVisibility
        {
            get
            {
                return _relationsVisibility;
            }
            set
            {
                _relationsVisibility = value;
            }
        }
        [JsonIgnore]
        public List<ApiDataModel> RelationshipsInternal
        {
            get
            {
                return _relations;
            }
            set
            {
                _relations = value;
            }
        }
        [JsonIgnore]
        public List<ApiDataModel> RelationshipsInternalParent
        {
            get
            {
                return _relationsParentDirection;
            }
            set
            {
                _relationsParentDirection = value;
            }
        }
        /// <summary>
        /// Get a Dictionary<ApiDataModel[i].Type,Relationships of index [i]> of all childs aftercome this data
        /// </summary>
        [JsonPropertyName("relationships")]
        public Dictionary<string, List<ApiRelationshipModel>> Relationships
        {
            get
            {
                if (_relationsVisibility)
                {
                    SetRelationShips();
                }

                return _relationsVisibility ?
                    _relationsWithoutAttributes?.Count == 0 ?
                    null : _relationsWithoutAttributes : null;
            }
            set
            {
                _relationsWithoutAttributes = value;
            }
        }
        /// <summary>
        /// Get a Dictionary<ApiDataModel[i].Type,Relationships of index [i]> of all childs aftercome this data
        /// </summary>
        [JsonPropertyName("parent-relationships")]
        public Dictionary<string, List<ApiRelationshipModel>> RelationshipsParent
        {
            get
            {
                if (_relationsVisibility)
                {
                    SetRelationShipsParent();
                }

                return _relationsVisibility ?
                    _relationsWithoutAttributesParent?.Count == 0 ?
                    null : _relationsWithoutAttributesParent : null;
            }
            set
            {
                _relationsWithoutAttributesParent = value;
            }
        }

        public async void SetRelationShips()
        {
            if (_relationsWithoutAttributes == null)
            {
                _relationsWithoutAttributes = new Dictionary<string, List<ApiRelationshipModel>>();
            }
            if (_relationsWithoutAttributes != null)
            {
                if (_relationsWithoutAttributes.Keys.Count == 0 && _relations != null)
                {
                    List<ApiDataModel> tmp = new List<ApiDataModel>();
                    int i2 = 0;
                    foreach (ApiDataModel item in _relations)
                    {
                        ApiDataModel copy = item.Copy;

                        List<ApiDataModel> copyList = new List<ApiDataModel>();
                        if (copy.OnlyRelation)
                        {
                            List<ApiDataModel> t = await copy.GetRelationToList(new List<ApiDataModel> { copy });
                            if (t.Count > 1)
                            {
                                t = t.FindAll(x => !x.OnlyRelation && x.Id != copy.Id);
                                if (t.Count != 0)
                                    copyList = t.FindAll(x => x.NodeDepth == t[0].NodeDepth);
                            }
                            copy = null;
                        }
                        if (copyList.Count == 0 && copy != null)
                        {
                            copyList.Add(copy);
                        }
                        if (copyList.Count != 0)
                        {
                            foreach (var it in copyList)
                            {
                                tmp.Add(it.Copy);
                                tmp[i2].AttributeVisibility = false;
                                tmp[i2].IncludeVisibility = false;
                                tmp[i2].LinkVisbility = false;
                                //tmp[i].Included = null;
                                ApiMetaModel meta = null;
                                if (tmp[i2].RelationshipsInternal != null)
                                {
                                    meta = new ApiMetaModel { Count = tmp[i2].RelationshipsInternal.Count };
                                }
                                string key = tmp[i2].Type;//tmp[i2].Parent.OnlyRelation? tmp[i2].Parent .Type: tmp[i2].Type;


                                bool canAdd = true;
                                int maxParentDisplayDepth = MaxDisplayNodeDepth;
                                if (maxParentDisplayDepth != GeneralDefs.NotFoundResponseValue)
                                {
                                    if (tmp[i2].NodeDepth > maxParentDisplayDepth)
                                    {
                                        canAdd = false;
                                    }
                                }




                                if (canAdd)
                                {
                                    ApiRelationshipModel relation = new ApiRelationshipModel { Data = tmp[i2], Links = tmp[i2].Links, Meta = meta };
                                    if (!_relationsWithoutAttributes.ContainsKey(key))
                                    {
                                        _relationsWithoutAttributes.Add(key, new List<ApiRelationshipModel>());
                                        _relationsWithoutAttributes[key].Add(relation);
                                    }
                                    else
                                    {
                                        var find = _relationsWithoutAttributes[key].Find(x => ((ApiDataModel)x.Data).Id == tmp[i2].Id);
                                        if (find == null)
                                            _relationsWithoutAttributes[key].Add(relation);
                                    }
                                }
                                i2++;
                            }
                        }


                    }
                }
            }
        }
        public async void SetRelationShipsParent()
        {
            if (_relationsWithoutAttributesParent == null)
            {
                _relationsWithoutAttributesParent = new Dictionary<string, List<ApiRelationshipModel>>();
            }
            if (_relationsWithoutAttributesParent != null)
            {
                if (_relationsWithoutAttributesParent.Keys.Count == 0 && _relationsParentDirection != null)
                {
                    List<ApiDataModel> tmp = new List<ApiDataModel>();
                    int i2 = 0;
                    foreach (ApiDataModel item in _relationsParentDirection)
                    {
                        ApiDataModel copy = item.Copy;

                        List<ApiDataModel> copyList = new List<ApiDataModel>();
                        if (copy.OnlyRelation)
                        {
                            List<ApiDataModel> t = await copy.GetRelationToList(new List<ApiDataModel> { copy }, false);
                            if (t.Count > 1)
                            {
                                t = t.FindAll(x => !x.OnlyRelation && x.Id != copy.Id);
                                if (t.Count != 0)
                                    copyList = t.FindAll(x => x.NodeDepth == t[0].NodeDepth);
                            }
                            copy = null;
                        }
                        if (copyList.Count == 0 && copy != null)
                        {
                            copyList.Add(copy);
                        }
                        if (copyList.Count != 0)
                        {
                            foreach (var it in copyList)
                            {
                                tmp.Add(it.Copy);
                                tmp[i2].AttributeVisibility = false;
                                tmp[i2].IncludeVisibility = false;
                                tmp[i2].LinkVisbility = false;
                                //tmp[i].Included = null;
                                ApiMetaModel meta = null;
                                if (tmp[i2]._relationsParentDirection != null)
                                {
                                    meta = new ApiMetaModel { Count = tmp[i2]._relationsParentDirection.Count };
                                }
                                string key = tmp[i2].Type;//tmp[i2].Parent.OnlyRelation? tmp[i2].Parent .Type: tmp[i2].Type;


                                bool canAdd = true;
                                int maxParentDisplayDepth = MaxDisplayNodeDepth;
                                if (maxParentDisplayDepth != GeneralDefs.NotFoundResponseValue)
                                {
                                    if (tmp[i2].NodeDepth > maxParentDisplayDepth)
                                    {
                                        canAdd = false;
                                    }
                                }




                                if (canAdd)
                                {
                                    ApiRelationshipModel relation = new ApiRelationshipModel { Data = tmp[i2], Links = tmp[i2].Links, Meta = meta };
                                    if (!_relationsWithoutAttributesParent.ContainsKey(key))
                                    {
                                        _relationsWithoutAttributesParent.Add(key, new List<ApiRelationshipModel>());
                                        _relationsWithoutAttributesParent[key].Add(relation);
                                    }
                                    else
                                    {
                                        var find = _relationsWithoutAttributesParent[key].Find(x => ((ApiDataModel)x.Data).Id == tmp[i2].Id);
                                        if (find == null)
                                            _relationsWithoutAttributesParent[key].Add(relation);
                                    }
                                }
                                i2++;
                            }
                        }


                    }
                }
            }
        }

        //nur links zu relation und data {type+id}
        //[JsonIgnore]
        [JsonPropertyName("included")]
        public List<ApiDataModel> Included
        {
            get
            {
                if (_includeVisibility)
                {
                    SetIncludes();
                }
                return _includeVisibility ? _includesWithAttributes?.Count == 0 ?
                    null : _includesWithAttributes : null;
            }
            set
            {
                _includesWithAttributes = value;
            }
        }
        //nur links zu relation und data {type+id} upward from this data
        //[JsonIgnore]
        [JsonPropertyName("parent-included")]
        public List<ApiDataModel> IncludedParent
        {
            get
            {
                if (_includeVisibility)
                {
                    SetIncludesParent();
                }
                return _includeVisibility ? _includesWithAttributesParent?.Count == 0 ?
                    null : _includesWithAttributesParent : null;
            }
            set
            {
                _includesWithAttributesParent = value;
            }
        }

        public async void SetIncludes()
        {
            if (_includesWithAttributes == null)
            {
                _includesWithAttributes = new List<ApiDataModel>();
            }
            if (_includesWithAttributes != null)
            {
                if (_includesWithAttributes.Count == 0 && _relations != null)
                {
                    _includesWithAttributes = new List<ApiDataModel>();
                    foreach (ApiDataModel item in _relations)
                    {
                        ApiDataModel copy = item.Copy;
                        List<ApiDataModel> copyList = new List<ApiDataModel>();
                        if (copy.OnlyRelation)
                        {
                            List<ApiDataModel> t = await copy.GetRelationToList(new List<ApiDataModel> { copy });
                            if (t.Count > 1)
                            {
                                t = t.FindAll(x => !x.OnlyRelation && x.Id != copy.Id);
                                if (t.Count != 0)
                                    copyList = t.FindAll(x => x.NodeDepth == t[0].NodeDepth);
                            }
                            copy = null;
                        }
                        if (copyList.Count == 0 && copy != null)
                        {
                            copyList.Add(copy);
                        }
                        if (copyList.Count != 0)
                        {
                            foreach (var it in copyList)
                            {
                                it.AttributeVisibility = true;
                                it.IncludeVisibility = true;
                                it.RelationsVisibility = false;

                                var find = _includesWithAttributes.Find(x => x.Id == it.Id);
                                if (find == null)
                                    _includesWithAttributes.Add(it);
                            }
                        }

                    }
                }
            }
        }
        public async void SetIncludesParent()
        {
            if (_includesWithAttributesParent == null)
            {
                _includesWithAttributesParent = new List<ApiDataModel>();
            }
            if (_includesWithAttributesParent != null)
            {
                if (_includesWithAttributesParent.Count == 0 && _relationsParentDirection != null)
                {
                    _includesWithAttributesParent = new List<ApiDataModel>();
                    foreach (ApiDataModel item in _relationsParentDirection)
                    {
                        ApiDataModel copy = item.Copy;
                        List<ApiDataModel> copyList = new List<ApiDataModel>();
                        if (copy.OnlyRelation)
                        {
                            List<ApiDataModel> t = await copy.GetRelationToList(new List<ApiDataModel> { copy }, false);
                            if (t.Count > 1)
                            {
                                t = t.FindAll(x => !x.OnlyRelation && x.Id != copy.Id);
                                if (t.Count != 0)
                                    copyList = t.FindAll(x => x.NodeDepth == t[0].NodeDepth);
                            }
                            copy = null;
                        }
                        if (copyList.Count == 0 && copy != null)
                        {
                            copyList.Add(copy);
                        }
                        if (copyList.Count != 0)
                        {
                            foreach (var it in copyList)
                            {
                                it.AttributeVisibility = true;
                                it.IncludeVisibility = true;
                                it.RelationsVisibility = false;

                                var find = _includesWithAttributesParent.Find(x => x.Id == it.Id);
                                if (find == null)
                                    _includesWithAttributesParent.Add(it);
                            }
                        }

                    }
                }
            }
        }

        //beinhaltet alle relation objects
        [JsonPropertyName("meta")]
        public ApiMetaModel Meta { get; set; }

        [JsonIgnore]
        public ApiDataModel Copy
        {
            get
            {
                ApiDataModel model = new ApiDataModel();
                model.Attributes = Attributes;
                model.AttributeVisibility = AttributeVisibility;
                model.Id = Id;
                //model.Included = Included;//wichtig nicht setzen, da sonst internal loop, da get accessor sich immer wieder selbst aufrufen würde
                model.IncludeVisibility = IncludeVisibility;
                model.Links = Links;
                model.LinkVisbility = LinkVisbility;
                model.MaxDisplayNodeDepth = MaxDisplayNodeDepth;
                model.Meta = Meta;
                model.NodeDepth = NodeDepth;
                model.Parent = Parent;
                model.OnlyRelation = OnlyRelation;
                //model.Relationships = Relationships;//wichtig nicht setzen, da sonst internal loop, da get accessor sich immer wieder selbst aufrufen würde
                model.RelationshipsInternal = RelationshipsInternal;
                model.RelationsVisibility = RelationsVisibility;
                model.Type = Type;
                return model;
            }
        }

        #endregion

        #region Ctor
        public ApiDataModel()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a collection of relations of given object currentDepthNode dependend of the setted maxdepth of relation-chain
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="currentDepthNode"></param>
        /// <param name="currentProgress"></param>
        /// <returns></returns>
        public async Task<List<ApiDataModel>> SetRelationMaxDepth(int maxDepth, ApiDataModel currentDepthNode, List<ApiDataModel> currentProgress)
        {
            List<ApiDataModel> responseValue = currentProgress;
            currentDepthNode.MaxDisplayNodeDepth = maxDepth;
            currentDepthNode.LinkVisbility = false;
            currentDepthNode.SetRelationShips();
            if (RelationshipsInternal != null)
            {
                if (currentDepthNode.RelationshipsInternal.Count != 0)
                {
                    foreach (ApiDataModel item in currentDepthNode.RelationshipsInternal)
                    {
                        item.MaxDisplayNodeDepth = _maxDepth;
                        item.LinkVisbility = false;
                        responseValue.Add(item);
                        await item.SetRelationMaxDepth(maxDepth, item, responseValue);
                    }
                }
            }
            return responseValue;

        }
        /// <summary>
        /// Get all Relations of current ApiDataModel Instance as List
        /// </summary>
        /// <param name="currentProgress"></param>
        /// <returns></returns>
        public async Task<List<ApiDataModel>> GetRelationToList(List<ApiDataModel> currentProgress, bool directionDownwardToChild = true, int depth = 0)
        {
            List<ApiDataModel> responseValue = currentProgress;
            if (directionDownwardToChild ? RelationshipsInternal != null : RelationshipsInternalParent != null)
            {
                if (directionDownwardToChild ? RelationshipsInternal.Count != 0 : RelationshipsInternalParent.Count != 0)
                {
                    foreach (ApiDataModel item in directionDownwardToChild ? RelationshipsInternal : RelationshipsInternalParent)
                    {
                        item.MaxDisplayNodeDepth = _maxDepth;
                        item.LinkVisbility = false;
                        responseValue.Add(item);
                        await item.GetRelationToList(responseValue, directionDownwardToChild, depth++);
                    }
                }
            }
            if (responseValue.Count != 0 && depth == 0)
                responseValue.RemoveAt(0);

            return responseValue;

        }
        #endregion
    }
}
