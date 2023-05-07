using Microsoft.AspNetCore.Mvc.Testing;
using WebApiApplicationService.Controllers.APIv1;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Controller;
using WebApiApplicationService;
using MySqlX.XDevAPI;
using System.Text;
using Xunit;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiApplicationServiceV2_Test.Tests.Data.DataTransferObject;
using WebApiFunction.Application.Model.Database.MySql.Helix;

namespace WebApiApplicationServiceV2_Test.Tests.IntegrationTests
{
    [Collection("IntegrationTests-Controller-OdbcDriverController")]
    public class OdbcDriverControllerTest : CustomApiControllerBaseTest<OdbcDriverModel, CustomApiControllerBase<OdbcDriverModel, AbstractBackendModule<OdbcDriverModel>>>
    {
        public OdbcDriverControllerTest(WebApplicationFactory<Startup> factory) : base(factory)
        {

        }
        [Fact]
        public async Task GetAll_OkWithResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/apiv1/odbcdriver",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            var apiData = responseJsonStr?.ConvertJsonStringToApiRootNodeModel();
            ApiTestCaseResponseObject apiTestCaseResponse = null;
            string apiTestCaseErrorOccuredResponseStr = null;
            if (apiData != null)
            {
                apiTestCaseResponse = new ApiTestCaseResponseObject(apiData, response.StatusCode);
                apiTestCaseErrorOccuredResponseStr = apiTestCaseResponse.ToString();

            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (apiTestCaseResponse != null && !apiTestCaseResponse.HasError);
            Assert.True(checkCondition, apiTestCaseErrorOccuredResponseStr);
        }
        [Fact]
        public async Task GetSingle_OkWithResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/apiv1/odbcdriver/266b1137-a5b5-11eb-bac0-309c2364fdb6",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            var apiData = responseJsonStr?.ConvertJsonStringToApiRootNodeModel();
            ApiTestCaseResponseObject apiTestCaseResponse = null;
            string apiTestCaseErrorOccuredResponseStr = null;
            if (apiData != null)
            {
                apiTestCaseResponse = new ApiTestCaseResponseObject(apiData, response.StatusCode);
                apiTestCaseErrorOccuredResponseStr = apiTestCaseResponse.ToString();

            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (apiTestCaseResponse != null && !apiTestCaseResponse.HasError);
            Assert.True(checkCondition, apiTestCaseErrorOccuredResponseStr);
        }
        [Fact]
        public async Task GetSingleWithRelationOdbcDriverVersion_OkWithResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/apiv1/odbcdriver/266b1137-a5b5-11eb-bac0-309c2364fdb6/relation/odbcdriverversion",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            var apiData = responseJsonStr?.ConvertJsonStringToApiRootNodeModel();
            ApiTestCaseResponseObject apiTestCaseResponse = null;
            string apiTestCaseErrorOccuredResponseStr = null;
            if (apiData != null)
            {
                apiTestCaseResponse = new ApiTestCaseResponseObject(apiData, response.StatusCode);
                apiTestCaseErrorOccuredResponseStr = apiTestCaseResponse.ToString();

            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (apiTestCaseResponse != null && !apiTestCaseResponse.HasError);
            Assert.True(checkCondition, apiTestCaseErrorOccuredResponseStr);
        }
        [Fact]
        public async Task GetSingleWithPagination_OkWithResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/apiv1/odbcdriver?page=0&max-items-per-page=2",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            var apiData = responseJsonStr?.ConvertJsonStringToApiRootNodeModel();
            ApiTestCaseResponseObject apiTestCaseResponse = null;
            string apiTestCaseErrorOccuredResponseStr = null;
            if (apiData != null)
            {
                apiTestCaseResponse = new ApiTestCaseResponseObject(apiData, response.StatusCode);
                apiTestCaseErrorOccuredResponseStr = apiTestCaseResponse.ToString();

            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (apiTestCaseResponse != null && !apiTestCaseResponse.HasError);
            Assert.True(checkCondition, apiTestCaseErrorOccuredResponseStr);
        }
        [Fact]
        public async Task GetSingleWithSortAscending_OkWithResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/apiv1/odbcdriver?sort=name",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            var apiData = responseJsonStr?.ConvertJsonStringToApiRootNodeModel();
            ApiTestCaseResponseObject apiTestCaseResponse = null;
            string apiTestCaseErrorOccuredResponseStr = null;
            if (apiData != null)
            {
                apiTestCaseResponse = new ApiTestCaseResponseObject(apiData, response.StatusCode);
                apiTestCaseErrorOccuredResponseStr = apiTestCaseResponse.ToString();

            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (apiTestCaseResponse != null && !apiTestCaseResponse.HasError);
            Assert.True(checkCondition, apiTestCaseErrorOccuredResponseStr);
        }
        [Fact]
        public async Task GetSingleWithSpecifigRelationInclude_OkWithResponse()
        {
            // Arrange
            var response = await CustomApiControllerBaseTestExtension.ExecuteHttpMethod(_factory,_client,HttpMethod.Get,
                "/apiv1/odbcdriver/266b1137-a5b5-11eb-bac0-309c2364fdb6?include=odbcdriverversionmodel.odbcdriverversionrelationtoconnectiontypemodel.connectiontypemodel",
                null,
                "application/json");
            // Act
            string responseJsonStr = await response.Content.ReadAsStringAsync();
            var apiData = responseJsonStr?.ConvertJsonStringToApiRootNodeModel();
            ApiTestCaseResponseObject apiTestCaseResponse = null;
            string apiTestCaseErrorOccuredResponseStr = null;   
            if(apiData!= null) 
            {
                apiTestCaseResponse = new ApiTestCaseResponseObject(apiData,response.StatusCode);
                apiTestCaseErrorOccuredResponseStr = apiTestCaseResponse.ToString();    

            }
            bool checkCondition = !String.IsNullOrEmpty(responseJsonStr) && (apiTestCaseResponse != null && !apiTestCaseResponse.HasError);
            Assert.True(checkCondition, apiTestCaseErrorOccuredResponseStr);
        }


    }
}
