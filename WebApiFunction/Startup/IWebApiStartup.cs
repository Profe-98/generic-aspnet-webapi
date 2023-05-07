using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Startup
{
    public interface IWebApiStartup
    {
        public static string[] DatabaseEntityNamespaces { get; }
        
    }
}
