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

        [TestMethod]
        async public Task TestMethod1()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.PostAsync("cqapi/cars", new Resources.Cars.CreateCommand { Owner = "samer" });
            httpResponseMessage.EnsureSuccessStatusCode();

        }




    }
}
