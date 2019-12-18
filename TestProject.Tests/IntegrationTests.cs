using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using TestProject.WebAPI;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Models;
using TestProject.WebAPI.SeedData;
using Xunit;

namespace TestProject.Tests
{
    public class IntegrationTests
    {
        private TestServer _server;

        public HttpClient Client { get; private set; }

        public IntegrationTests()
        {
            SetUpClient();
        }

        // TEST NAME - checkMiddleware
        // TEST DESCRIPTION - Forbidden if there is no token in request
        [Fact]
        public async Task TestCase0()
        {
            var response0 = await Client.GetAsync("/api/testmiddleware/token");
            response0.StatusCode.Should().BeEquivalentTo(403);

            var response1 = await Client.GetAsync("/api/testmiddleware/notoken");
            response1.StatusCode.Should().BeEquivalentTo(200);

            var response2 = await Client.GetAsync("/api/testmiddleware/token/?token=12345678");
            response2.StatusCode.Should().BeEquivalentTo(200);
        }

        // TEST NAME - analyzeFile
        // TEST DESCRIPTION - In this test User should send byte array to the web api and get analytic
        [Fact]
        public async Task TestCase1()
        {
            //Here data is exporting to the end point
            var myJsonString = File.ReadAllBytes("UserCollection.xml");
            var content = new ByteArrayContent(myJsonString);
            var response0 = await Client.PostAsync("/api/file/analyze", content);
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var statisticalData = JsonConvert.DeserializeObject<StatisticalModel>(response0.Content.ReadAsStringAsync().Result);

            statisticalData.MinRate.Should().Be(10);
            statisticalData.MaxRate.Should().Be(60);
            statisticalData.AverageRate.Should().Be((float)35.828);
        }

        // TEST NAME - addUserToFile
        // TEST DESCRIPTION - In this test User should send byte array to the web api and get analytic
        [Fact]
        public async Task TestCase2()
        {
            //Here data is exporting to the end point
            var myJsonString = File.ReadAllBytes("UserCollection.xml");
            var content = new ByteArrayContent(myJsonString);
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(content);
            var user = new XMLUser {Email = "sadasd@email.com"};
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            multipartContent.Add(stringContent);
            var response0 = await Client.PostAsync("/api/file/adduser", stringContent);
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var statisticalData = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
        }

        private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var context = new TestProjectContext(new DbContextOptionsBuilder<TestProjectContext>()
                        .UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

                    services.RemoveAll(typeof(TestProjectContext));
                    services.AddSingleton(context);

                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    context.SaveChanges();

                    // Clear local context cache
                    foreach (var entity in context.ChangeTracker.Entries().ToList())
                    {
                        entity.State = EntityState.Detached;
                    }
                });

            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }
    }
}
