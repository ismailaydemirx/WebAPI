using Castle.Core.Configuration;
using ETicaretProject.API.DataAccess;
using ETicaretProject.API.Entities;
using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyServices.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ETicaretProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")] // gelen istekteki token'da rolü burada belirlemiş olduğumuz kişiler aşağıdaki methodları çağırabilir dedik.
    public class AccountController : ControllerBase
    {
        // Applyment : Satıcı Başvuru
        // Register : Üye Kaydı
        // Authenticate : Kimlik Doğrulaması

        private DatabaseContext _db;
        private IConfiguration _configuration;

        public AccountController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }

        [HttpPost("merchant/applyment")]
        [ProducesResponseType(typeof(Resp<ApplymentAccountResponseModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ApplymentAccountResponseModel>), 400)] // hatalı durudma döndüğüm nesne
        public IActionResult Applyment([FromBody] ApplymentAccountRequestsModel model)
        {
            Resp<ApplymentAccountResponseModel> response = new Resp<ApplymentAccountResponseModel>();

            //if (ModelState.IsValid)
            //{
            model.Username = model.Username?.Trim().ToLower();
            if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))
            {
                response.AddError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor.");
                return BadRequest(response);
            }
            // Password ve repassword eşit mi değil mi onun kontrolü
            //if (model.Password != model.RePassword)
            //{
            //    ModelState.AddModelError(nameof(model.Password), "Girdiğiniz parolalar eşleşmiyor.");
            //    return BadRequest(ModelState);
            //}
            else
            {
                Account account = new Account
                {
                    Username = model.Username,
                    Password = model.Password,
                    CompanyName = model.CompanyName,
                    ContactEmail = model.ContactEmail,
                    ContactName = model.ContactName,
                    Type = AccountType.Merchant.ToString(),
                    IsAppleyment = true,
                };

                _db.Accounts.Add(account);
                _db.SaveChanges();

                ApplymentAccountResponseModel applymentAccountResponseModel = new ApplymentAccountResponseModel
                {
                    Id = account.Id,
                    Username = account.Username,
                    ContactName = account.ContactName,
                    CompanyName = account.CompanyName,
                    ContactEmail = account.ContactEmail,
                };

                response.Data = applymentAccountResponseModel;

                return Ok(response);
            }
            //}
            //List<string> errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList();
            //return BadRequest(errors);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(Resp<AuthenticateResponseModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<AuthenticateResponseModel>), 400)] // hatalı durudma döndüğüm nesne
        public IActionResult Register([FromBody] RegisterRequestModel model)
        {
            Resp<RegisterResponseModel> response = new Resp<RegisterResponseModel>();

            model.Username = model.Username?.Trim().ToLower();
            if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))
            {
                response.AddError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor.");
                return BadRequest(response);
            }
            else
            {
                Account account = new Account
                {
                    Username = model.Username,
                    Password = model.Password,
                    Type = AccountType.Member.ToString(),
                };

                _db.Accounts.Add(account);
                _db.SaveChanges();

                RegisterResponseModel data = new RegisterResponseModel
                {
                    Id = account.Id,
                    Username = account.Username,
                };

                response.Data = data;
                return Ok(response);
            }
        }

        [AllowAnonymous] // anonymous yapmamızdan dolayı sadece admin değil herkes authenticate methodunu kullanabilecek.
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(Resp<AuthenticateRequestModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<AuthenticateRequestModel>), 400)] // hatalı durudma döndüğüm nesne
        public IActionResult Authenticate([FromBody] AuthenticateRequestModel model)
        {
            Resp<AuthenticateResponseModel> response = new Resp<AuthenticateResponseModel>();

            model.Username = model.Username.Trim().ToLower();

            Account account = _db.Accounts.SingleOrDefault(
                x => x.Username.ToLower() == model.Username && x.Password == model.Password); // bir tane kayıt çektik


            if (account != null)
            {
                if (account.IsAppleyment)
                {
                    response.AddError("*", "Henüz satıcı başvurusu tamamlanmamıştır.");
                    return BadRequest(response);
                }
                else
                {
                    string key = _configuration["JwtOptions:Key"];

                    List<Claim> claims = new List<Claim>
                        {
                            new Claim("id",account.Id.ToString()),
                            new Claim(ClaimTypes.Name,account.Username),
                            new Claim("type",account.Type.ToString()),
                            new Claim(ClaimTypes.Role,account.Type.ToString()),
                        };

                    string token = TokenService.GenerateToken(key, DateTime.Now.AddDays(30), claims);

                    AuthenticateResponseModel data = new AuthenticateResponseModel { Token = token };
                    response.Data = data;

                    return Ok(response);
                }
            }
            else
            {
                response.AddError("*", "Kullanıcı adı ya da şifre eşleşmiyor");
                return BadRequest(response);
            }


        }

    }
}
