using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                NotFound();

            return Ok(user);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromBody]Filters filters)
        {
            var users = await _usersService.Get(null, filters);
            return Ok(users);
        }

        [HttpGet("/test")]
        public async Task<IActionResult> GetAll2()
        {
            
            return Ok("sdadasdasd");
        }

        [HttpPost]
        public async Task<IActionResult> Add(User user)
        {
            await _usersService.Add(user);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(User user)
        {
            await _usersService.Add(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = (await _usersService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                NoContent();

            await _usersService.Delete(user);
            return Ok(user);
        }
    }
}
