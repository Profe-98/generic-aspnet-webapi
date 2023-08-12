using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Infrastructure.Database
{
    public class QueryException : EventArgs
    {
        public Exception Exception { get; set; }
        public object Sender { get; set; }
    }
}
