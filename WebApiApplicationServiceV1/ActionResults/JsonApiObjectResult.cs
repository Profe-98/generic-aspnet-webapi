using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiApplicationService.Models;
using System.Net;
using WebApiApplicationService.Handler;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace WebApiApplicationService
{
    public class JsonApiAbstractObjectResult<T> : OkObjectResult, IDisposable
    {
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public override string ToString()
        {
            string jsonStr = null;
            using (JsonHandler jsonHandler = new JsonHandler())
            {
                jsonStr = jsonHandler.JsonSerialize(this.Value);
            }
            return jsonStr;
        }
        public async Task<HttpResponse> AppendToHttpResponse(HttpResponse httpResponse)
        {
            string jsonStr = this.ToString();
            if (String.IsNullOrEmpty(jsonStr))
                return httpResponse;
            httpResponse.StatusCode = this.StatusCode??500;
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);
            await httpResponse.Body.WriteAsync(data,0, data.Length);
            return httpResponse;
        }
        public string ResultMsg { get; private set; }

        public T ResultValue { get; private set; }

        public JsonApiAbstractObjectResult(T value,string message = null) : base(value)
        {
            ResultValue = value;
            ResultMsg = message;
        }
        ~JsonApiAbstractObjectResult()
        {
            this.Dispose();
        }
    }
    public class JsonApiObjectResult : JsonApiAbstractObjectResult<List<ApiDataModel>>
    {

        public JsonApiObjectResult(List<ApiDataModel> value, string message = null) : base(value,message)
        {

        }
    }
    public class JsonApiErrorResult : JsonApiAbstractObjectResult<List<ApiErrorModel>>
    {

        public JsonApiErrorResult(List<ApiErrorModel> value, string message = null) : base(value, message)
        {

        }
    }
}
