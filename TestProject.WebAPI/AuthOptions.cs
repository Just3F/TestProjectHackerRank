using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestProject.WebAPI
{
    public class AuthOptions
    {
        public const string ISSUER = "AuthServerName";
        public const string AUDIENCE = "http://localhost:54598/";
        const string KEY = "this is my custom Secret key for authnetication!123";
        public const int LIFETIME = 1;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
