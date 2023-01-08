using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Data;
using System.Net;
using WebApiApplicationService.Models;
using WebApiApplicationService.Handler;
using WebApiApplicationService.InternalModels;


namespace WebApiApplicationService.Handler
{
    public class HttpContextHandler : IHttpContextHandler
    {
        #region Private
        private readonly IHttpContextAccessor _httpContextAccessor = null;
        #endregion
        #region Private
        #endregion
        #region Ctor & Dtor
        public HttpContextHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion
        #region Methods
        public class RouteDescriptor
        {
            private string _route = null;
            private string[] _segments = null;
            public string[] Segments
            {
                get
                {

                    return _segments;
                }
            }
            public string Route
            {
                get
                {
                    return _route;
                }
            }
            public RouteDescriptor(string route)
            {
                _route = route;
                _segments = route.Split(new string[] {"/" },StringSplitOptions.None);
            }
        }
        public string GetRoute()
        {
            if(_httpContextAccessor.HttpContext != null)
            {
                PathString pathString = _httpContextAccessor.HttpContext.Request.Path;
                
                if (pathString != null)
                {
                    if (pathString.Value != null)
                    {
                        string uri = pathString.Value.TrimStart('/');
                        return uri;
                    }
                }
            }
            return null;
        }
        public HttpContext CurrentContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        #endregion

    }
}
