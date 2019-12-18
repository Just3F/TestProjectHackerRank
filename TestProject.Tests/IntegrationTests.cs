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

        private async Task SeedData()
        {
            var createForm0 = GenerateCreateForm("Mike", "Emil", 24, "testemail1@mail.com", "12345678");
            var response0 = await Client.PostAsync("/api/users", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));

            var createForm1 = GenerateCreateForm("Daniel", "Johnson", 19, "testemail2@mail.com", "12345678");
            var response1 = await Client.PostAsync("/api/users", new StringContent(JsonConvert.SerializeObject(createForm1), Encoding.UTF8, "application/json"));

            var createForm2 = GenerateCreateForm("Daniel", "Olson", 19, "testemail2@mail.com", "12345678");
            var response2 = await Client.PostAsync("/api/users", new StringContent(JsonConvert.SerializeObject(createForm2), Encoding.UTF8, "application/json"));

            var createForm3 = GenerateCreateForm("Olga", "Verso", 19, "testemail21s2@mail.com", "12345678");
            var response3 = await Client.PostAsync("/api/users", new StringContent(JsonConvert.SerializeObject(createForm3), Encoding.UTF8, "application/json"));
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

        // TEST NAME - processFile
        // TEST DESCRIPTION - In this test User should send byte array to the web api and get processed data back
        [Fact]
        public async Task TestCase1()
        {
            //Here data is exporting to the end point
            var myJsonString = File.ReadAllBytes("UserCollection.xml");
            var content = new ByteArrayContent(myJsonString);
            var response0 = await Client.PostAsync("/api/test/process", content);
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var testContent = response0.Content;
        }

        private CreateUserForm GenerateCreateForm(string firstName, string lastName, uint age, string email, string password)
        {
            return new CreateUserForm()
            {
                FirstName = firstName,
                Age = age,
                Email = email,
                LastName = lastName,
                Password = password
            };
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
