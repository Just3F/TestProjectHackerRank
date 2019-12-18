using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;

namespace TestProject.WebAPI.Controllers
{
    class XMLUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Rate { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpPost("process")]
        public async Task<IActionResult> ProcessFile()
        {
            string content;
            var users = new List<XMLUser>();

            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(content);
                var userCollection = xmlDoc.SelectSingleNode("UserCollection");
                var usersNodes = userCollection.SelectNodes("User");
                foreach (XmlElement xmlElement in usersNodes)
                {
                    users.Add(new XMLUser
                    {
                        Email = xmlElement.SelectSingleNode("email").InnerText,
                        FirstName = xmlElement.SelectSingleNode("first_name").InnerText,
                        LastName = xmlElement.SelectSingleNode("last_name").InnerText,
                        Rate = Convert.ToInt32(xmlElement.SelectSingleNode("rate").InnerText)
                    });
                }
            }

            var bytes = Encoding.ASCII.GetBytes(content);
            return File(bytes, "application/xml");
        }
    }
}
