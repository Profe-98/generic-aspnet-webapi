using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiFunction.Database
{
    public class QueryException : EventArgs
    {
        public Exception Exception { get; set; }    
        public object Sender { get; set; }  
    }
}
