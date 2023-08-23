using ETicaretProject.API.DataAccess;
using ETicaretProject.API.Entities;
using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MyServices.API.Services;

namespace ETicaretProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class PaymentController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;

        public PaymentController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }

        // Ödeme ekranı,
        // Pay : ödeme methodu


        [HttpPost("Pay/{cartId}")]
        [ProducesResponseType(200, Type = typeof(Resp<PaymentResponseModel>))]
        [ProducesResponseType(400, Type = typeof(Resp<string>))]
        public IActionResult Pay([FromBody] int cartid, [FromRoute] PayModel model)
        {
            Resp<PaymentModel> result = new Resp<PaymentModel>();
            Cart cart = _db.Carts.Include(x => x.CartProducts).SingleOrDefault(x => x.Id == cartid);
            string paymnetApiEndpoint = _configuration["PaymentAPI:EndPoint"];

            if (!cart.IsClosed)
            {
                decimal totalPrice =
                    model.TotalPriceOverride ?? cart.CartProducts.Sum(x => x.Quantity * x.DiscountedPrice); // eğer model kısmındaki totalprice boş ise cart içindeki cartproducts'a git oradaki toplam adet ile indirimli fiyatı çarp ve topla en son totalPrice'a bu değeri ver.
                HttpClientService client = new HttpClientService(paymnetApiEndpoint);

                AuthRequestModel authRequestModel =
                    new AuthRequestModel
                    {
                        Username = _configuration["PaymentAPI:Username"],
                        Password = _configuration["PaymentAPI:Password"]
                    };

                HttpClientServiceResponse<AuthResponseModel> authResponse =
                    client.Post<AuthRequestModel, AuthResponseModel>("/Pay/authenticate", authRequestModel);


                if (authResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string token = authResponse.Data.Token;

                    PaymentRequestModel paymentRequestModel = new PaymentRequestModel
                    {
                        CardNumber = model.CardNumber,
                        CardName = model.CardName,
                        ExpireDate = model.ExpireDate,
                        CVV = model.CVV,
                        TotalPrice = totalPrice
                    };

                    HttpClientServiceResponse<PaymentResponseModel> paymentResponse =
                        client.Post<PaymentRequestModel, PaymentResponseModel>("/Pay/Payment", paymentRequestModel, token);


                    if (paymentResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        if (paymentResponse.Data.Result == "ok")
                        {
                            string transactionId = paymentResponse.Data.TransactionId;

                            Payment payment = new Payment
                            {
                                CartId = cartid,
                                AccountId = cart.AccountId,
                                InvoiceAddress = model.InvoiceAddress,
                                ShippedAddress = model.ShippedAddress,
                                Type = model.Type,
                                TransactionId = transactionId,
                                Date = DateTime.Now,
                                IsCompleted = true,
                                TotalPrice = totalPrice
                            };

                            cart.IsClosed = true;

                            _db.Payments.Add(payment);
                            _db.SaveChanges();

                            PaymentModel data = new PaymentModel
                            {
                                Id = payment.Id,
                                AccountId = payment.AccountId,
                                CartId = payment.CartId,
                                Date = payment.Date,
                                InvoiceAddress = payment.InvoiceAddress,
                                IsCompleted = payment.IsCompleted,
                                ShippedAddress = payment.ShippedAddress,
                                TotalPrice = payment.TotalPrice,
                                Type = payment.Type
                            };

                            result.Data = data;

                            return Ok(result);
                        }
                        else
                        {
                            Resp<string> paymentOkResult = new Resp<string>();
                            paymentOkResult.AddError("payment", "Ödeme alınamadı.");

                            return BadRequest(paymentOkResult);
                        }
                    }
                    else
                    {
                        Resp<string> paymentResult = new Resp<string>();
                        paymentResult.AddError("payment", paymentResponse.ResponseContent);

                        return BadRequest(paymentResult);
                    }
                }
                else
                {
                    Resp<string> authResult = new Resp<string>();
                    authResult.AddError("auth", authResponse.ResponseContent);

                    return BadRequest(authResult);
                }
            }
            else
            {
                Payment payment = _db.Payments.SingleOrDefault(x => x.Id == cart.Id);

                if (payment == null)
                {
                    result.AddError("cart", $"Sepet kapalı ama ödemesi yapılmamış görünmektedir. Olası sorun tespit edildi. Lütfen sistem sağlayıcı ile iletişime geçiniz. Cart Id:{cartid}");

                    return BadRequest(result);
                }

                PaymentModel data = new PaymentModel
                {
                    Id = payment.Id,
                    AccountId = payment.AccountId,
                    CartId = payment.CartId,
                    Date = payment.Date,
                    InvoiceAddress = payment.InvoiceAddress,
                    IsCompleted = payment.IsCompleted,
                    ShippedAddress = payment.ShippedAddress,
                    TotalPrice = payment.TotalPrice,
                    Type = payment.Type
                };

                result.Data = data;
                return Ok(result);
            }

        }

    }
}
