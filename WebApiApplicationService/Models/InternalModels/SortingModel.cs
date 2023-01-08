using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace WebApiApplicationService.InternalModels
{
    public class SortingModel
    {
        public enum SORTING_DIRECTION : int
        {
            ASC,
            DESC
        }
        //Json Nodename != MemberInfo.Name, Nodename is an Attribute of an Member of a specific AbstractModelType
        public string NodeName { get; set; }
        public SORTING_DIRECTION Sort { get; set; }
        public PropertyInfo TargetMember { get; set; }
    }
}
