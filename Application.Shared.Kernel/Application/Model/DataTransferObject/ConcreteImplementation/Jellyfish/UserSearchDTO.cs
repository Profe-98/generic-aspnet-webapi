using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Shared.Kernel.Application.Model.DataTransferObject;
using Application.Shared.Kernel.Configuration.Const;

namespace Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish
{
    public class UserSearchDTO : DataTransferModelAbstract
    {
        [JsonIgnore]
        public override Guid Uuid { get => base.Uuid; set => base.Uuid = value; }

        [JsonPropertyName("search_user")]
        [Required(AllowEmptyStrings = false, ErrorMessage = DataValidationMessageStruct.MemberIsRequiredButNotSetMsg)]
        public string SearchUser { get; set; }
    }
}
