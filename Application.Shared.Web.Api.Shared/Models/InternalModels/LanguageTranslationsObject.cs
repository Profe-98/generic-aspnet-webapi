using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Application.Shared.Web.Api.Shared.Models.InternalModels
{
    public class LanguageTranslationsObject
    {
        public string Language = null;
        public Encoding Encoding = null;
        public List<ValueTuple> Translations = new List<ValueTuple>();
    }
}
