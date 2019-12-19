using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TestProject.WebAPI.SeedData
{
    public class AddUsersToFileModelForm
    {
        public IEnumerable<UserForAddModelForm> Users { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class UserForAddModelForm
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("rate")]
        public int Rate { get; set; }
    }
}
