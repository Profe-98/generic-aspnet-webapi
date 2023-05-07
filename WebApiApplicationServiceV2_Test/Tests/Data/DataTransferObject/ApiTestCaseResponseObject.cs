using Autofac.Features.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;

namespace WebApiApplicationServiceV2_Test.Tests.Data.DataTransferObject
{
    public class ApiTestCaseResponseObject
    {
        private readonly ApiRootNodeModel _model;
        private readonly HttpStatusCode _httpStatusCode;
        public ApiTestCaseResponseObject(ApiRootNodeModel apiRootNodeModel,HttpStatusCode httpStatusCode)
        {
            _model = apiRootNodeModel;
            _httpStatusCode = httpStatusCode;
        }

        public bool HasError
        {
            get
            {
                return _model.HasErrors;
            }
        }

        public override string ToString()
        {
            if(_model !=null)
            {
                return null;
            }
            string apiInformation = _model.Jsonapi.ToString();
            string apiDataType = "data: " + (_model.IsDataApiDataModelList ? "list" : "object") + "";
            string apiMeta = _model.Meta.ToString();
            string apiErrorSummary = null;
            string apiDataSummary = null;
            string jsonStr = null;
            using (JsonHandler jsonHandler= new JsonHandler())
            {
                apiDataSummary = _model.HasAnyData?jsonHandler.JsonSerialize(_model.Data):null;
                var serilizeObj = new
                {
                    httpStatusCode = _httpStatusCode,
                    apiInformation = apiInformation,
                    apiDataType = apiDataType,
                    apiMeta = apiMeta,
                    apiDataSummary = apiDataSummary,
                    apiErrorSummary = apiErrorSummary,
                    hasData = _model.HasAnyData,
                    hasErrors = _model.HasErrors,
                }; 
                
                jsonStr = jsonHandler.JsonSerialize(serilizeObj);

            }
            return jsonStr;
        }
    }
}
