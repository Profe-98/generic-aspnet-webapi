using System;
using Microsoft.AspNetCore.Authentication;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;

namespace Application.Shared.Kernel.Web.AspNet.CustomActionResult
{
    public static class AuthenticateResultExtension
    {
        public static AuthenticateResult FailEx(string message, IJsonHandler jsonHandler)
        {

            return CreateErrorAuthentificateResult(message, jsonHandler);
        }
        private static AuthenticateResult CreateErrorAuthentificateResult(string message, IJsonHandler jsonHandler)
        {
            var model = new ApiRootNodeModel()
            {
                Data = null,
                Errors = new List<ApiErrorModel> { new ApiErrorModel { Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_UNAUTHORIZED, Detail = message } },
                Meta = new ApiMetaModel
                {
                    Count = 1,
                    OptionalMessage = message,
                },
                Jsonapi = ApiRootNodeModel.GetApiInformation()
            };
            string json = jsonHandler.JsonSerialize(model);
            return AuthenticateResult.Fail(json);
        }
    }
}
