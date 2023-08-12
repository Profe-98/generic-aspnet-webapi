using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Extension;

namespace Application.Shared.Kernel.Configuration.Service
{
    public interface IAppconfig
    {
        public MainConfigurationModel AppServiceConfiguration { get; set; }
        public void Save();
        public void Load();
    }
}
