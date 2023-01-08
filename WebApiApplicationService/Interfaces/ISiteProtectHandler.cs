using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace WebApiApplicationService
{
    public interface ISiteProtectHandler
    {
        public Task<SiteProtectHandler.ObserveResult> ObserveEndpoint(HttpContext context, IPEndPoint endPoint, IDatabaseHandler _databaseHandler);
    }
    public interface ISingletonSiteProtectHandler : ISiteProtectHandler
    {
    }
    public interface IScopedSiteProtectHandler : ISiteProtectHandler
    {
    }
}
