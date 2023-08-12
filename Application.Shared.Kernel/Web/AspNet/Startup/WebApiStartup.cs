using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration;

namespace Application.Shared.Kernel.Web.AspNet.Startup
{
    public abstract class WebApiStartup : IWebApiStartup
    {
        public IConfiguration Configuration { get; protected set; }
        public WebApiStartup(IConfiguration previousConfig, IWebHostEnvironment env)
        {
            PreConfigurationInitialAction();
            var cfg = SetInitialConfiguration(env.ContentRootPath);
            var config = LoadConfiguration(previousConfig, env, cfg);

            Configuration = config;
            Console.WriteLine("Init completed");
        }
        public virtual void PreConfigurationInitialAction()
        {

        }

        public virtual IConfiguration LoadConfiguration(IConfiguration previousConfig, IWebHostEnvironment env, IMainConfigurationModel cfg)
        {
            throw new NotImplementedException();
        }

        public virtual IMainConfigurationModel SetInitialConfiguration(string rootDir)
        {
            throw new NotImplementedException();
        }
    }
}
