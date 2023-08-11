using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication_JwtAuth.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous] // Eğer kullanıcı buraya erişip token üretemezse alıp onu kullanıp bize valid yani geçerli bir istek atamaz o yüzden herkese açık yapıyoruz.
        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthModel model)
        {
            // kullanıcı girişini kontrol ediyoruz.
            if (model.Username == "test" && model.Password == "123123")
            {
                // şimdi de token üretiyoruz.
                return Ok(TokenService.GenerateToken(model.Username));

            }

            return BadRequest("Kullanıcı adı ya da şifre hatalı");

        }
    }

    public class AuthModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
