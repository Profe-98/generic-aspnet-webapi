using Ocelot.Middleware;
using Ocelot.Responses;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json.Serialization;
using Ocelot.DownstreamRouteFinder;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.LoadBalancer;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.DownstreamPathManipulation;
using Ocelot.DownstreamPathManipulation.Middleware;
using Ocelot.DownstreamUrlCreator;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Infrastructure.Extensions;
using Ocelot.Request;
using Ocelot.RequestId;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Ocelot.ServiceDiscovery;
using Ocelot.Authentication.Middleware;
using Ocelot.Authorization.Middleware;
using Ocelot.Multiplexer;
using Ocelot.Requester.Middleware;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Request.Middleware;
using Ocelot.QueryStrings.Middleware;
using Ocelot.RateLimit.Middleware;
using Ocelot.Security.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.Claims.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Microsoft.AspNetCore.Http;
using Ocelot.WebSockets.Middleware;
using Ocelot.Authorization;
using Ocelot.Configuration;
using Ocelot.Errors;
using Ocelot.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.DependencyInjection;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Application.Shared.Kernel.Infrastructure.Metric.Influxdb;

namespace Application.Web.Gateway.Middleware
{

    public class GeneralMiddlewareDelegate : DelegatingHandler
    {

        private readonly IInfluxDbHandlerInterface _metricHandlerInterface;
        public GeneralMiddlewareDelegate(IInfluxDbHandlerInterface metricHandlerInterface)
        {
            _metricHandlerInterface = metricHandlerInterface;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_metricHandlerInterface != null)
            {

                bool reachableMetricService = await _metricHandlerInterface.IsReachable();
                if (reachableMetricService)
                {
                    var results = await _metricHandlerInterface.Read(async query =>
                    {
                        var flux = "from(bucket:\"api_gateway\") |> range(start: -1h) |> filter(fn: (r) => r._measurement == \"requestss\" and r._field == \"http://localhost:5009/apiv1/authentification/login\" and r.url == \"counter\")";
                        var tables = await query.QueryAsync(flux, "csharp_webapi");
                        return tables.SelectMany(table =>
                            table.Records.Select(record =>
                            {
                                return record.GetValue();
                            })).ToList();
                    });
                    _metricHandlerInterface.Write(async (x) =>
                    {
                        string url = request.RequestUri.ToString();
                        var data = await request.Content.ReadAsByteArrayAsync();

                        int currentCount = results.Count;

                        var point2 = PointData.Measurement("requestss").Tag("url", "counter").Field(url, currentCount++).Timestamp(DateTime.UtcNow, WritePrecision.Ns);
                        var point = PointData.Measurement("requests").Tag("packet", "byte-len").Field("value", data.Length).Timestamp(DateTime.UtcNow, WritePrecision.Ns);
                        x.WritePoints(new List<PointData> { point, point2 }, "api_gateway", "csharp_webapi");
                    });
                }
            }
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
