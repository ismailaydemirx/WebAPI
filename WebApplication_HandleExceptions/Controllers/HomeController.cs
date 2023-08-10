using Microsoft.AspNetCore.Diagnostics;
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error-development")]
        public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment host)
        {
            if (host.IsDevelopment() == false)
                return NotFound();

            var exceptionHandleFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: exceptionHandleFeature.Error.StackTrace,
                title: exceptionHandleFeature.Error.Message);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError()
        {

            return Problem();

        }
    }
}
