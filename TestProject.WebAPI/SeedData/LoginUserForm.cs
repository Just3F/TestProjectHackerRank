using Newtonsoft.Json;

namespace TestProject.WebAPI.SeedData
{
    public class LoginUserForm
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
