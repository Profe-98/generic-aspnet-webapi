using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.Shared.Kernel.Application
{
    public class ClientErrorHandler : IClientErrorFactory
    {
        public IActionResult GetClientError(ActionContext actionContext, IClientErrorActionResult clientError)
        {
            return null;
        }
    }
}
