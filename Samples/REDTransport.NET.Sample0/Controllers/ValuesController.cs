using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace REDTransport.NET.Sample0.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]/[action]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<object>> Test()
        {
            return new
            {
                Id = 100001,
                FirstName = "Ali",
                LastName = "Mousavi Kherad",
                Age = 19,
            };
        }
        
        [HttpGet]
        public async Task<ActionResult<object>> Test1()
        {
            return new
            {
                Id = 100050,
                FirstName = "John",
                LastName = "Doe",
                Age = 24,
            };
        }
    }
}