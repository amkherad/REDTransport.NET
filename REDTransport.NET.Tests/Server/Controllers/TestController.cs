using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace REDTransport.NET.Tests.Server.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]/[action]")]
    public class TestController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<object>> TestData()
        {
            return new
            {
                Id = 100001,
                FirstName = "Ali",
                LastName = "Mousavi Kherad",
                Age = 19,
            };
        }
    }
}