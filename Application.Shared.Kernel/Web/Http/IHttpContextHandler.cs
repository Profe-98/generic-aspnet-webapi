﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Application.Shared.Kernel.Web.Http
{

    public interface IHttpContextHandler
    {
        public string GetRoute();
        public HttpContext CurrentContext();
    }
}
