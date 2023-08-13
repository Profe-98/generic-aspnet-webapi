using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Shared.Kernel.Configuration.Model.Abstraction;

namespace Application.Shared.Kernel.Configuration.Model.ConcreteImplementation
{

    public class RoutesConfigurationModel : AbstractConfigurationModel
    {
        public class DownstreamHostAndPortModel
        {
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public class LoadBalancerOptionsModel
        {
            public string Type { get; set; }
            public string Key { get; set; }
            public int? Expiry { get; set; }
        }
        public class AuthenticationOptionsModel
        {
            public string AuthenticationProviderKey { get; set; }
        }
        public class RouteClaimsRequirementModel
        {
            public Dictionary<string,string> Role { get; set; } = new Dictionary<string, string>();
        }
        public class RouteModel
        {
            public string DownstreamPathTemplate { get; set; }
            public string DownstreamScheme { get; set; }
            public List<DownstreamHostAndPortModel> DownstreamHostAndPorts { get; set; }
            public string UpstreamPathTemplate { get; set; }
            public List<string> UpstreamHttpMethod { get; set; }
            public AuthenticationOptionsModel AuthenticationOptions { get; set; }
            public RouteClaimsRequirementModel RouteClaimsRequirement { get; set; }
            public LoadBalancerOptionsModel LoadBalancerOptions { get; set; }
        }

        public class GlobalConfigurationModel
        {
            public string BaseUrl { get; set; }
            public List<string> DelegatingHandlers { get; set; }
        }

        public List<RouteModel> Routes { get; set; }
        public GlobalConfigurationModel GlobalConfiguration { get; set; }

    }
}
