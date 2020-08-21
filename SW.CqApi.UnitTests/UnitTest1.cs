using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var httpResponseMessage = await httpClient.GetAsync("cqapi/cars?plate=12&cartype=sedan");
            var rs = await httpResponseMessage.Content.ReadAsAsync<CarDto>();
            Assert.AreEqual<int>(12, rs.Plate);
        }

        [TestMethod]
        async public Task TestQueryHandlerGen2()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/cars/6?multiplier=2");
            var rs = await httpResponseMessage.Content.ReadAsAsync<int>();
            Assert.AreEqual<int>(12, rs);
        }

        [TestMethod]
        async public Task TestGetHandlerGen1()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/bikes/12");
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
        async public Task TestQueryHandlerGen2WithHandle()
        {
            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.GetAsync("cqapi/bikes/6/wheel?q=6");
            var rs = await httpResponseMessage.Content.ReadAsAsync<int>();
            Assert.AreEqual<int>(12, rs);
        }

        [TestMethod]
        async public Task TestDeleteHandler()
        {

            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.DeleteAsync("cqapi/cars/1");
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        [TestMethod]
        async public Task TestProtectedHandlerFail()
        {

            var httpClient = server.CreateClient();
            var httpResponseMessage = await httpClient.PostAsync("cqapi/cars/protectedget", new ProtectedGetCommand());
            Assert.IsTrue(httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
