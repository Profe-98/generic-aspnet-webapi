using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration;

namespace Application.Shared.Kernel.Web.AspNet.Startup
{
    public interface IWebApiStartup
    {
        public static string[] DatabaseEntityNamespaces { get; }
        public IMainConfigurationModel SetInitialConfiguration(string rootDir);
        public IConfiguration LoadConfiguration(IConfiguration previousConfig, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, IMainConfigurationModel cfg);

    }
}
