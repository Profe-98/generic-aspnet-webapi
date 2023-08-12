using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;

namespace Application.Shared.Kernel.Application.Model.Internal
{
    public class HttpStatusException : Exception
    {
        public bool IsResponseObjectSetted
        {
            get
            {
                return ResponseObject != null ? true : false;
            }
        }
        public HttpStatusCode Status { get; private set; }
        public ApiErrorModel.ERROR_CODES ErrCode { get; private set; }
        public ApiRootNodeModel ResponseObject { get; private set; }
        public string ResponseMsg { get; private set; }
        public string ResponseDetailMsg { get; private set; }

        public HttpStatusException(HttpStatusCode status, ApiErrorModel.ERROR_CODES errCode, string msg, string detailMsg = null, ApiRootNodeModel responseObj = null) : base(msg)
        {
            ErrCode = errCode;
            ResponseMsg = msg;
            ResponseDetailMsg = msg;
            Status = status;
            ResponseObject = responseObj ?? ApiRootNodeModel.PrepareErrorResponse(null, new List<ApiErrorModel> { new ApiErrorModel { Detail = ResponseDetailMsg, HttpStatus = status.ToString() } }, msg);
        }
    }
}
