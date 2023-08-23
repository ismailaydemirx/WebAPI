using AdminApp.Models;
using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyServices.API.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdminApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MerchantApplyment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult MerchantApplyment(ApplymentAccountRequestsModel model)
        {
            if (ModelState.IsValid)
            {
                string endpoint = _configuration["ETicaretAPI:EndPoint"];
                HttpClientService client = new HttpClientService(endpoint);

                AuthenticateRequestModel authenticateRequestModel = new AuthenticateRequestModel
                {
                    Username = _configuration["ETicaretAPI:AdminUid"],
                    Password = _configuration["ETicaretAPI:AdminPwd"]
                };

                HttpClientServiceResponse<Resp<AuthenticateResponseModel>> authResponse =
                    client.Post<AuthenticateRequestModel, Resp<AuthenticateResponseModel>>("/Account/Authenticate", authenticateRequestModel);

                if (authResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string token = authResponse.Data.Data.Token;

                    HttpClientServiceResponse<Resp<ApplymentAccountResponseModel>> applyResponse =
                                client.Post<ApplymentAccountRequestsModel, Resp<ApplymentAccountResponseModel>>("/Account/merchant/applyment", model, token);

                    if (applyResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewData["success"] = "Satıcı başvurusu alınmıştır.";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, applyResponse.ResponseContent);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, authResponse.ResponseContent);
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
