using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Configuration.Const
{
    public struct DataValidationMessageStruct
    {
        public const string StringMinLengthExceededMsg = "min string length not reached";
        public const string StringMaxLengthExceededMsg = "max string length exceeded";
        public const string WrongDateTimeFormatMsg = "wrong date-time/date format";
        public const string MemberIsRequiredButNotSetMsg = "required value";
        public const string WrongDataTypeGivenMsg = "wrong data-type given";
        public const string OnlyCharsInStringAllowedMsg = "only chars allowed in string";


    }
}
