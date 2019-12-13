using Newtonsoft.Json;

namespace TestProject.WebAPI.SeedData
{
    public class CreateUserForm
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")] 
        public string Password { get; set; }

        [JsonProperty("age")]
        public uint Age { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }
}
