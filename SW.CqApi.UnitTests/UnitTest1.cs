using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

using System.Threading.Tasks;
using SW.HttpExtensions;
using SW.CqApi.UnitTests.Resources.Cars;

namespace SW.CqApi.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        static TestServer server;


        [ClassInitialize]
        public static void ClassInitialize(TestContext tcontext)
        {
            server = new TestServer(new WebHostBuilder()
                .UseEnvironment("UnitTesting")
                .UseStartup<TestStartup>());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            server.Dispose();
        }

        /*
         * Testing each handler
         * The Gen refers to number of generic arguements 
         * To distinguish the handlers
        */


        [TestMethod]
        async public Task TestCommandHandlerGen1()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.PostAsync("cqapi/cars", new Resources.Cars.CreateCommand { Owner = "samer" });
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        [TestMethod]
        async public Task TestQueryHandler()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/cars");
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        [TestMethod]
        async public Task TestQueryHandlerGen1()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/cars?plate=12");
            var rs = await httpResponseMessage.Content.ReadAsAsync<CarDto>();
            Assert.AreEqual<int>(12, rs.Plate);
        }

        [TestMethod]
        async public Task TestGetHandlerGen1()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/cars/12");
            var rs = await httpResponseMessage.Content.ReadAsAsync<int>();
            Assert.AreEqual<int>(12, rs);
        }

        [TestMethod]
        async public Task TestSearchyHandler()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/cars/search?page=12");
            var rs = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.AreEqual<string>("page=12", rs);
        }

        [TestMethod]
        async public Task TestCommandHandlerGen2()
        {

            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.PostAsync("cqapi/cars/1", new CarDto { Plate = 2 });
            var rs = await httpResponseMessage.Content.ReadAsAsync<int>();
            Assert.AreEqual<int>(12, rs);
        }

        [TestMethod]
        async public Task TestDeleteHandler()
        {

            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("cqapi/cars/1"),
            });
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        





    }
}
