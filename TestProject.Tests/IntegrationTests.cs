using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using TestProject.WebAPI;
using TestProject.WebAPI.Data;
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
            var createForm0 = GenerateCreateForm("Dmitry", "Vasilyuk", 24, "just3f@yandex.ru", "12345678");
            var response0 = await Client.PostAsync("/api/restaurant", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));
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

        [Fact]
        public async Task Test1()
        {
            await SeedData();

            // Get All restaurants 
            var response0 = await Client.GetAsync("/api/users");
            response0.StatusCode.Should().BeEquivalentTo(200);
            var realData0 = JsonConvert.DeserializeObject(response0.Content.ReadAsStringAsync().Result);
            var expectedData0 = JsonConvert.DeserializeObject("[{\"id\":1,\"city\":\"Miami\",\"name\":\"Big Brewskey\",\"estimatedCost\":1500,\"averageRating\":\"4.8\",\"votes\":500},{\"id\":2,\"city\":\"Florida\",\"name\":\"Social\",\"estimatedCost\":1600,\"averageRating\":\"4.7\",\"votes\":400},{\"id\":3,\"city\":\"Miami\",\"name\":\"Social\",\"estimatedCost\":1000,\"averageRating\":\"4.2\",\"votes\":50},{\"id\":4,\"city\":\"Florida\",\"name\":\"CCD\",\"estimatedCost\":1000,\"averageRating\":\"3.8\",\"votes\":200},{\"id\":5,\"city\":\"Miami\",\"name\":\"CCD\",\"estimatedCost\":1100,\"averageRating\":\"4.1\",\"votes\":100}]");
            realData0.Should().BeEquivalentTo(expectedData0);
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
