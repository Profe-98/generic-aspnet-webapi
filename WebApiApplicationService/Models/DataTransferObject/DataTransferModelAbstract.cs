
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApplicationService.Models.DataTransferObject
{
    public  abstract class DataTransferModelAbstract 
    {
        public DataTransferModelAbstract()
        {

        }

        public string TransformString(string input)
        {
            return input == null?null:input.ToLower().Trim();
        }
    }
}
