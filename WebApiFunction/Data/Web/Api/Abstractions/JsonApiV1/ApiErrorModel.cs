using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1
{
    [Serializable]
    public class ApiErrorModel :BaseModel
    {
        public enum ERROR_CODES : int
        {
            HTTP_REQU_BAD = 400,
            INTERNAL = 500,
            ERROR_OCCURRED = 0,
            HTTP_REQU_UNPROCESSABLE_ENTITY = 422,//given entity in requ is not processable because duo some errors 
            JSON_SYNTHAX_IN_REQU = HTTP_REQU_UNPROCESSABLE_ENTITY,//JSON Synthax Fehler in Request
            HTTP_REQU_QUERY = 400,//Fehler in REQU-URL, e.g. falsche Sort/Pagination Parameter etc.
            HTTP_REQU_RESOURCE_NOT_FOUND = 404,//Resource nicht gefunden
            HTTP_REQU_FORBIDDEN = 401,//not permitted
            HTTP_REQU_CONFLICT = 409,//e.g. Relation or Resource exists already and would try to add another time
            HTTP_REQU_UNAUTHORIZED = HTTP_REQU_FORBIDDEN,//Nicht authorisiert für Zugriff auf Zielresource / e.g. Keine Berechtigung oder kein Login...
            HTTP_REQU_GONE = 410,//Resource no longer avaible e.g. a deleted Resource that exists before
            HTTP_REQU_TO_MANY_REQU = 429,//classical 429
            HTTP_REQU_PAYLOAD_TO_LARGE = 413,//too big payload
            HTTP_REQU_CONTENT_LEN_REQUIRED = 411,//content-length required in http requ. header
            HTTP_REQU_URI_TO_LONG = 414,//request uri is too long
            HTTP_REQU_MEDIA_TYPE_NOT_SUPPORTED = 415,//classical 415
            HTTP_REQU_HEADER_FIELD_TO_LARGE = 431
        }
        //Data as a single object of Type ApiDataModel or List<ApiDataModel>

        [JsonPropertyName("id")]
        public string Id_External
        {
            get
            {
                return Id == Guid.Empty ? null : Id.ToString();
            }
        }
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonPropertyName("links")]
        public ApiLinkModel Links { get; set; }
        [JsonPropertyName("status")]
        public string HttpStatus { get; set; }
        [JsonPropertyName("code")]
        public ERROR_CODES Code { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
        [JsonPropertyName("source")]
        public ApiErrorSourceModel Source { get; set; }
        [JsonPropertyName("meta")]
        public ApiMetaModel Meta { get; set; }


    }
}
