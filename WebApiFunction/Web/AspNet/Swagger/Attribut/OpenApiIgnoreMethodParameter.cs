﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Web.AspNet.Swagger.Attribut
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OpenApiIgnoreMethodParameter :System.Attribute
    {
    }
}
