using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MySqlX.XDevAPI;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService.Attribute;


namespace Application.Shared.Kernel.Web.AspNet.Swagger.SignalR
{
    public class SignalRDocumentationFilter : IDocumentFilter
    {
        private readonly List<EndpointDataSource> _endpoints;
        private readonly ISingletonJsonHandler _singletonJsonHandler;
        public SignalRDocumentationFilter(ISingletonJsonHandler singletonJsonHandler, IEnumerable<EndpointDataSource> endpointSources)
        {
            _singletonJsonHandler = singletonJsonHandler;
            _endpoints = endpointSources.ToList();
        }
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var endpoints = _endpoints.SelectMany(es => es.Endpoints).OfType<RouteEndpoint>().ToList();
            foreach (var item in endpoints)
            {
                var hubServiceAttr = item.Metadata.OfType<HubServiceRouteAttribute>();
                if (!hubServiceAttr.Any())
                {
                    continue;
                }
                var hubMeta = item.Metadata.OfType<HubMetadata>().First();
                var t = item.GetType();

                string route = item.RoutePattern.RawText;

                string hubName = hubServiceAttr.First().Route.TrimStart('/');
                string tagName = "SignalR/WebSocket Endpoint - " + hubName[0].ToString().ToUpper() + hubName.Substring(1, hubName.Length - 1);
                OpenApiPathItem openApiPathItem = new OpenApiPathItem();
                var hubMethods = hubMeta.HubType.GetMethods().ToList().FindAll(x => x.IsPublic && x.GetBaseDefinition() == null);

                SignalRInitMessage signalRInitMessage = new SignalRInitMessage() {  protocol = "json", version=1};
                string contentInit = _singletonJsonHandler.JsonSerialize(signalRInitMessage);
                //string contentExampleInit = _singletonJsonHandler.JsonSerialize();
                var encodingDict = new Dictionary<string, OpenApiEncoding>() { { "UTF8",new OpenApiEncoding { ContentType="application/json" } } };
                openApiPathItem.AddOperation(OperationType.Get, new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody 
                    { 
                        Content= new Dictionary<string, OpenApiMediaType> 
                        {
                             {
                                 "application/json",new OpenApiMediaType
                                 {
                                     Encoding=encodingDict,
                                     /*Example= new OpenApiRequestBody
                                     {
                                         Content=new Dictionary<string, OpenApiMediaType>
                                         {
                                             {
                                                 contentExampleInit,new OpenApiMediaType
                                                 {
                                                     Encoding =encodingDict
                                                 }
                                             }
                                         }
                                     }*/
                                 }
                             }
                        },
                    },
                    Description = "Init",
                    Tags = new List<OpenApiTag>()
                        {

                            new OpenApiTag
                            {
                                Name= tagName
                            }
                        }
                });
                foreach (var action in hubMethods)
                {
                    var p = action.GetParameters().ToList();
                    var args = new string[p?.Count??0];
                    if(p!=null&&p.Any())
                    {
                        for(int i=0; i<p.Count; i++)
                        {
                            args[i] = p[i].ToString();  
                        }
                    }
                    SignalRInvokeMethodModel signalRInvokeMethodModel = new SignalRInvokeMethodModel() { arguments= args,invocationId = "0", target=action.Name, type=0 };
                    string contentAction = _singletonJsonHandler.JsonSerialize(signalRInvokeMethodModel);
                    openApiPathItem.AddOperation(OperationType.Get, new OpenApiOperation
                    {
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                        {
                             {
                                 "application/json",new OpenApiMediaType
                                 {
                                     Encoding=encodingDict,
                                 }
                             }
                        },
                        },
                        Description = action.Name,
                        Tags = new List<OpenApiTag>()
                        {

                            new OpenApiTag
                            {
                                Name= tagName
                            }
                        }
                    });
                }
                openApiPathItem.Description = route;
                openApiPathItem.Summary = "SignalR Hub Method";

                
                if (!swaggerDoc.Paths.ContainsKey(route))
                {
                    swaggerDoc.Paths.Add(route, openApiPathItem);
                }
            }
        }

        public class SignalRInitMessage
        {
            public string protocol { get; set; }
            public int version { get; set; }
        }

        public class SignalRInvokeMethodModel
        {
            public string[] arguments { get; set; }
            public string invocationId { get; set; }
            public string target { get; set; }
            public int type { get; set; }
        }

    }
}
