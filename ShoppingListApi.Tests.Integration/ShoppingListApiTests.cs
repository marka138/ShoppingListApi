using NUnit.Framework;
using Owin;
using Microsoft.Owin.Testing;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Text;
using ShoppingListApi.Data;
using ShoppingListApi.Models.ResponseModels;
using System.Collections.Generic;

namespace ShoppingListApi.Tests.Integration
{
    [TestFixture]
    public class ShoppingListApiTests
    {
        private class TestStartup : Startup
        {
            public override void Configuration(IAppBuilder app)
            {
                var config = new HttpConfiguration();
                config.MapHttpAttributeRoutes();
                app.UseWebApi(config);
                UnityConfig.Register(config);
            }
        }

        List<ShoppingListItem> _dataStore;

        [SetUp]
        public void SetUp()
        {
            _dataStore = InMemoryDataStore.GetInstance();
            _dataStore.Clear();
        }

        [Test]
        public void should_return_ok_when_getting_shopping_list()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var response = server.HttpClient.GetAsync("api/shoppinglist").Result;

                // then
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Test]
        public void should_return_bad_request_when_creating_invalid_shopping_list_item()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var response = server.HttpClient.PostAsync("api/shoppinglist", null).Result;

                // then
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Test]
        public void should_return_created_when_psoting_valid_shopping_list_item()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var request = new StringContent("{ 'ItemName':'Item1', 'Quantity':1 }", Encoding.Default, "application/json");
                var response = server.HttpClient.PostAsync("api/shoppinglist", request).Result;

                // then
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }
        }

        [Test]
        public void should_return_not_found_when_getting_shopping_list_item_which_does_not_exist()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var request = new StringContent("{ 'ItemName':'Item1', 'Quantity':1 }", Encoding.Default, "application/json");
                var response = server.CreateRequest("api/shoppinglist?itemname=Item1").GetAsync().Result;

                // then
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Test]
        public void should_return_ok_when_getting_shopping_list_item_which_exists()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var request = new StringContent("{ 'ItemName':'Item1', 'Quantity':1 }", Encoding.Default, "application/json");
                var response = server.HttpClient.PostAsync("api/shoppinglist", request).Result;
                response = server.CreateRequest("api/shoppinglist?itemname=Item1").GetAsync().Result;

                // then
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Test]
        public void should_return_ok_with_correct_data_when_getting_shopping_list_item_which_exists()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var request = new StringContent("{ 'ItemName':'Item1', 'Quantity':1 }", Encoding.Default, "application/json");
                var response = server.HttpClient.PostAsync("api/shoppinglist", request).Result;
                response = server.CreateRequest("api/shoppinglist?itemname=Item1").GetAsync().Result;

                var content = response.Content.ReadAsStringAsync().Result;

                // then
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.That(content, Is.EqualTo("{\"ItemName\":\"Item1\",\"Quantity\":1}"));
            }
        }

        [Test]
        public void should_return_ok_with_correct_data_when_updating_shopping_list_item()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var createRequest = new StringContent("{ 'ItemName':'Item1', 'Quantity':1 }", Encoding.Default, "application/json");
                var createResponse = server.HttpClient.PostAsync("api/shoppinglist", createRequest).Result;

                var updateRequest = new StringContent("{ 'ItemName':'Item1', 'Quantity':5 }", Encoding.Default, "application/json");
                var updateResponse = server.HttpClient.PutAsync("api/shoppinglist", updateRequest).Result;

                var getResponse = server.CreateRequest("api/shoppinglist?itemname=Item1").GetAsync().Result;

                var content = getResponse.Content.ReadAsStringAsync().Result;

                // then
                Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
                Assert.That(content, Is.EqualTo("{\"ItemName\":\"Item1\",\"Quantity\":5}"));
            }
        }

        [Test]
        public void should_delete_shopping_list_item_and_return_not_found_when_try_to_get_after()
        {
            // given
            using (var server = TestServer.Create<TestStartup>())
            {
                // when
                var createRequest = new StringContent("{ 'ItemName':'Item1', 'Quantity':1 }", Encoding.Default, "application/json");
                var createResponse = server.HttpClient.PostAsync("api/shoppinglist", createRequest).Result;
                
                var deleteResponse = server.HttpClient.DeleteAsync("api/shoppinglist?itemname=Item1").Result;

                var getResponse = server.CreateRequest("api/shoppinglist?itemname=Item1").GetAsync().Result;

                // then
                Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
            }
        }
    }
}
