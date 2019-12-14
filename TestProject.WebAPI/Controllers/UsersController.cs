using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Services;

namespace TestProject.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = (await _usersService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery]Filters filters)
        {
            var users = await _usersService.Get(null, filters);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Add(User user)
        {
            await _usersService.Add(user);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            await _usersService.Update(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = (await _usersService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                return NotFound();

            await _usersService.Delete(user);
            return NoContent();
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export()
        {
            string content;
            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);

            users = await _usersService.AddRange(users);

            return Ok(users);
        }
    }
}
