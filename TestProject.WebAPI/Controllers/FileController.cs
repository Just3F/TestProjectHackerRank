﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestProject.WebAPI.Models;
using TestProject.WebAPI.SeedData;

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
        public async Task<IActionResult> AddUserToFile(AddUsersToFileModelForm model)
        {
            string content;

            await using (var memoryStream = new MemoryStream(Convert.FromBase64String(model.Content)))
            using (var reader = new StreamReader(memoryStream))
            {
                content = reader.ReadToEnd();
            }

            XDocument xmlDoc = XDocument.Parse(content);
            foreach (var userForAddModelForm in model.Users)
            {
                XElement userXmlElement = new XElement("User");
                userXmlElement.Add(new XElement("id", userForAddModelForm.Id));
                userXmlElement.Add(new XElement("first_name", userForAddModelForm.FirstName));
                userXmlElement.Add(new XElement("last_name", userForAddModelForm.LastName));
                userXmlElement.Add(new XElement("email", userForAddModelForm.Email));
                userXmlElement.Add(new XElement("rate", userForAddModelForm.Rate));

                xmlDoc.Element("UserCollection")?.Add(userXmlElement);
            }

            string resultXml;
            await using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                resultXml = stringWriter.GetStringBuilder().ToString();
            }

            return new ContentResult
            {
                Content = resultXml,
                ContentType = "text/xml",
                StatusCode = 200
            };
        }
    }
}