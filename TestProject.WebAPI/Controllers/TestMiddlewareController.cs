using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestProject.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestMiddlewareController : ControllerBase
    {
        [HttpGet("token")]
        public async Task<IActionResult> GetToken()
        {
            return Ok();
        }

        [HttpGet("notoken")]
        public async Task<IActionResult> GetNoToken()
        {
            return Ok();
        }
    }
}
