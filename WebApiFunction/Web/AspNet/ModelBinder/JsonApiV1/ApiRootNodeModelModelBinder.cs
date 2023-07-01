using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;

namespace WebApiFunction.Web.AspNet.ModelBinder.JsonApiV1
{
    public class ApiRootNodeModelModelBinder<T> : IModelBinder
        where T : class
    {
        private readonly IScopedJsonHandler _jsonHandler;
        public ApiRootNodeModelModelBinder(IScopedJsonHandler scopedJsonHandler)
        {
            _jsonHandler = scopedJsonHandler;
        }
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                var body = await reader.ReadToEndAsync();

                var data = _jsonHandler.JsonDeserialize<ApiRootNodeModel>(body);

                if (data == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }
                if(data.Meta == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                if (!data.HasAnyData)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                var allTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();
                int i = 0;
                bool[] foundTypes = new bool[data.RootNodes.Count];
                data.RootNodes.ForEach(x => {
                    if(allTypes.Find(y => y == x.GetType())!= null)
                    {
                        foundTypes[i] = true;   
                    }
                    i++;
                });

                if(foundTypes.ToList().FindAll(x=> x).Count() != data.RootNodes.Count)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                Type compareType = typeof(T);
                var dataFromFirstDepthLevel = data.ExtractByType<T>();
                if (dataFromFirstDepthLevel == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }
                if (data.Meta.Count == null || data.Meta.Count != dataFromFirstDepthLevel.Count)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }
                if (bindingContext.ModelType == typeof(List<T>))
                {

                    bindingContext.Result = ModelBindingResult.Success(dataFromFirstDepthLevel);
                }
                else if (dataFromFirstDepthLevel.Count == 1)
                {
                    bindingContext.Result = ModelBindingResult.Success(dataFromFirstDepthLevel.First());
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                return;

            }

        }
    }
}
