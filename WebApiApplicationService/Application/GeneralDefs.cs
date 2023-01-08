using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mime;

namespace WebApiApplicationService
{
    public static class GeneralDefs
    {
        public const int NotFoundResponseValue = -1;
        public const string ApiContentType = MediaTypeNames.Application.Json;
        public const string ImageContentType = "image/jpeg";
        public const string SvgXmlContentType = "image/svg+xml";
        public const string BinarayContentType = "application/octet-stream";//general
        public const string ZipContentType = "application/zip";//für driver (ddls+exe etc.)
        public const string MultipartFormData = "multipart/form-data";
        public const string ApiAreaV1 = "apiv1";
        public enum LANGUAGE : int
        {
            DE = 0,
            EN = 1
        }

        public enum JwtHashAlgorithm
        {
            RS256,
            HS384,
            HS512
        }

        static GeneralDefs()
        {

        }
    }
}
