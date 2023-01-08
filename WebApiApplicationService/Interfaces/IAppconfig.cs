using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;

namespace WebApiApplicationService
{
    public interface IAppconfig
    {
        public AppServiceConfigurationModel AppServiceConfiguration { get; set; }
        public void Save();
        public void Load();
    }
}
