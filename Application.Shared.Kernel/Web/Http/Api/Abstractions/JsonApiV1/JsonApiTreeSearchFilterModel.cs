using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1
{
    public class JsonApiTreeSearchFilterModel
    {
        public string EntityName { get; set; }
        public List<JsonApiTreeSearchFilterModel> FilterRelationNames { get; set; } = null;
        public bool AreFilterRelationNamesGiven
        {
            get { return this.FilterRelationNames != null && this.FilterRelationNames.Count != 0; }
        }

        public JsonApiTreeSearchFilterModel()
        {

        }

    }
}
