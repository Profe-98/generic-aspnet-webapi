using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;

namespace Application.Shared.Kernel.Configuration
{
    public class MainConfigurationModelSingleton
    {
        private static MainConfigurationModel _mailConfigurationModel;
        public MainConfigurationModel Get()
        {
            if( _mailConfigurationModel == null )
            {
                _mailConfigurationModel = new MainConfigurationModel();
            }
            return _mailConfigurationModel;
        }
    }
}
