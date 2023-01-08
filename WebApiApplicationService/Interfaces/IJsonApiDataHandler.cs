using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Models;
using Microsoft.AspNetCore.Http;
using WebApiApplicationService.Handler;
using System.Text.Json;
using WebApiApplicationService.Models.Database;   

namespace WebApiApplicationService
{
    public interface IJsonApiDataHandler
    {
        public Task<ApiRootNodeModel> CreateApiRootNodeFromModel<T>(string area, List<T> data,int maxDepth=1) where T : AbstractModel;
        public Task<ApiRootNodeModel> CreateApiRootNodeFromApiData<T>(string area, List<T> data) where T : ApiDataModel;
        public Task<List<object>> GetorSetCacheData(object model);
        public ApiRootNodeModel GetFromJsonBody(JsonDocument jsonDocument);
        public object GetForeignObject<T>(T datam, ClassRelationModel rel) where T : AbstractModel;
        public Task<List<ApiDataModel>> GetConvertedFromModel<T>(string area, List<T> data, bool relatedObject, int depth = 0, int maxDepth=1) where T : AbstractModel;
        public List<T> GetConvertedFromApiData<T>(string area, List<T> data) where T : ApiDataModel;
        public Task<List<ApiDataModel>> GetRelations<T>(T datam, string area, int depth, int maxDepth) where T : AbstractModel;
    }
}
