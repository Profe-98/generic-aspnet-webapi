using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplicationService.InternalModels
{
    public class ExceptionObject : EventArgs
    {
        public string ExceptionEnvironment = null;
        public Exception Exception = null;
        public DateTime ExceptionDateTime;
        public AppManager.MESSAGE_LEVEL ExceptionLevel;
    }
}
