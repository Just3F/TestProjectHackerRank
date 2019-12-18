using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using TestProject.WebAPI.Models;

namespace TestProject.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        [HttpPost("analyze")]
        public async Task<IActionResult> ProcessFile()
        {
            var users = new List<XMLUser>();

            using (var reader = new StreamReader(Request.Body))
            {
                var content = await reader.ReadToEndAsync();
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

            return Ok(new StatisticalModel
            {
                AverageRate = (float)users.Sum(x => x.Rate) / users.Count,
                MaxRate = users.Max(x => x.Rate),
                MinRate = users.Min(x => x.Rate)
            });
        }

        [HttpPost("adduser")]
        public async Task<IActionResult> AddUserToFile([FromBody]XMLUser user)
        {
            var users = new List<XMLUser>();
            string content;

            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            return new ContentResult
            {
                Content = xmlDoc.ToString(),
                ContentType = "text/xml",
                StatusCode = 200
            };
        }
    }
}
