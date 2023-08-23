using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyServices.API.Services;
using PaymentAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PaymentAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class PayController : ControllerBase
    {
        private IConfiguration _configuration;

        public PayController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Öncelikle Authenticate olacağımız method ekliyoruz 

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(200, Type = typeof(AuthResponseModel))]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult Authenticate([FromBody] AuthRequestModel model)
        {
            string uid = _configuration["Auth:Uid"];
            string pass = _configuration["Auth:Pass"];

            if (model.Username == uid && model.Password == pass)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("uid", uid));

                string token = TokenService.GenerateToken(
                    _configuration["JwtOptions:Key"],
                    DateTime.Now.AddDays(30),
                    claims,
                    _configuration["JwtOptions:Issuer"],
                    _configuration["JwtOptions:Audience"]);

                return Ok(new AuthResponseModel { Token = token }); // kod 200
            }
            else
            {
                return BadRequest("Kullanıcı adı ve şifre eşleşmiyor!"); // kod 400
            }
        }

        [HttpPost("payment")]
        [ProducesResponseType(200, Type = typeof(PaymentResponseModel))]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult Payment([FromBody] PaymentRequestModel model)
        {
            string cardno = _configuration["CardTest:No"];
            string name = _configuration["CardTest:Name"];
            string exp = _configuration["CardTest:Exp"];
            string cvv = _configuration["CardTest:CVV"];

            if (model.CardNumber == cardno && model.CardName == name && model.ExpireDate == exp && model.CVV == cvv)
            {
                return Ok(new PaymentResponseModel { Result = "ok", TransactionId = Guid.NewGuid().ToString() });
            }
            else
            {
                return BadRequest("Kart bilgileri geçersiz. Ödeme alınamadı.");
            }
        }
    }
}
