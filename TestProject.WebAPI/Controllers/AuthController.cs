using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Models;

namespace TestProject.WebAPI.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TestProjectContext _testProjectContext;

        public AuthController(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }

        [Authorize]
        [HttpGet("/currentuser")]
        public async Task<IActionResult> CheckUserAuthrize()
        {
            var userEmail = User.Identity.Name;
            var user = await _testProjectContext.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
            return Ok(user);
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token(LoginModel loginModel)
        {
            var identity = GetIdentity(loginModel.Email, loginModel.Password);
            if (identity == null)
            {
                await Response.WriteAsync("Invalid username or password.");
                return StatusCode(404);
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromHours(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new LoginResponseModel
            {
                AccessToken = encodedJwt,
                Username = identity.Name
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(string email, string password)
        {
            var user = _testProjectContext.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
