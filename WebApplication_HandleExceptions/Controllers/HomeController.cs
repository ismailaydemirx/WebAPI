using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace WebApplication_HandleExceptions.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        // localhost:[port]/error-development
        // action seviyesinde bir dependency injection
        [Route("/error-development")]
        public IActionResult HandleErrorDevelopment([FromServices]IHostEnvironment host)
        {

        }
    }
}
