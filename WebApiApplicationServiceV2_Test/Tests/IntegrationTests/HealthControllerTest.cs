using Microsoft.AspNetCore.Mvc.Testing;
using WebApiApplicationService.Controllers.APIv1;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Controller;
using WebApiApplicationService;
using MySqlX.XDevAPI;
using System.Text;
using Xunit;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiApplicationServiceV2_Test.Tests.Data.DataTransferObject;
using WebApiFunction.Data.Format.Json;
using System.Collections.ObjectModel;

namespace WebApiApplicationServiceV2_Test.Tests.IntegrationTests
{
    [Collection("IntegrationTests-Healthcheck")]
    public class HealthControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public HealthControllerTest(WebApplicationFactory<Startup> factory) 
        {
            _factory=factory;
            _client=factory.CreateClient();
        }
        [Fact]
        public async Task GetHealth_OkResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/health",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            Dictionary<string, object> healthCheckResponse = null;
            using (JsonHandler jsonHandler = new JsonHandler())
            {
                healthCheckResponse = jsonHandler.JsonDeserialize<Dictionary<string, object>>(responseJsonStr);
            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (healthCheckResponse!=null&&healthCheckResponse.Keys.Count != 0) && response.StatusCode == System.Net.HttpStatusCode.OK;
            Assert.True(checkCondition, "healthcheck not normal");
        }



    }
}
