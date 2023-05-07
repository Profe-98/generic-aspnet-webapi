using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApiApplicationService.InternalModels
{
    public class CustomProducesFilter : ProducesAttribute
    {
        public CustomProducesFilter(Type type) : 
            base(type)
        {
        }
        public CustomProducesFilter(string contentType, params string[] otherTypes) : 
            base(contentType,otherTypes)
        {

        }
        public void OnResourceExecuted(ActionExecutingContext context) 
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    }
}
