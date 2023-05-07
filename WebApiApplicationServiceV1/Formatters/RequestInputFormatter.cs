using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace WebApiApplicationService.Formatters
{
    public class RequestInputFormatter : TextInputFormatter
    {
        public RequestInputFormatter()
        {
            var header = MediaTypeHeaderValue.Parse(GeneralDefs.ApiContentType);
            SupportedMediaTypes.Add(header);
            header = MediaTypeHeaderValue.Parse("text/plain");
            SupportedMediaTypes.Add(header);
            SupportedEncodings.Add(Encoding.UTF8);
        }
        protected override bool CanReadType(Type type)
        {
            return type == typeof(object);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            return InputFormatterResult.Success(null);
        }
    }
}
