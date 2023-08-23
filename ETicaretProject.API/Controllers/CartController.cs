using ETicaretProject.API.DataAccess;
using ETicaretProject.API.Entities;
using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ETicaretProject.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Resp<CartModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
    [ProducesResponseType(typeof(Resp<CartModel>), 400)] // hatalı durudma döndüğüm nesne

    public class CartController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;

        public CartController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }

        // GetOrCreate : sepet getir ya da oluştur
        // AddToCart : Sepet e ürün ekleme

        [HttpGet("GetOrCreate/{accountId}")]
        public IActionResult GetOrCreate([FromRoute] int accountId)
        {
            Resp<CartModel> response = new Resp<CartModel>();

            Cart cart = _db.Carts
                .Include(x => x.CartProducts)
                .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = DateTime.Now,
                    IsClosed = false,
                    CartProducts = new List<CartProduct>()
                };

                _db.Carts.Add(cart);
                _db.SaveChanges();
            }

            CartModel data = CartToCartModel(cart);

            response.Data = data;
            return Ok(response);

        }

        private static CartModel CartToCartModel(Cart cart)
        {
            CartModel data = new CartModel
            {
                Id = cart.Id,
                AccountId = cart.AccountId,
                Date = DateTime.Now,
                IsClosed = false,
                CartProducts = new List<CartProductModel>()
            };

            foreach (CartProduct cartProduct in cart.CartProducts)
            {
                data.CartProducts.Add(new CartProductModel
                {
                    Id = cartProduct.Id,
                    CartId = cartProduct.CartId.Value,
                    UnitPrice = cartProduct.UnitPrice,
                    DiscountedPrice = cartProduct.DiscountedPrice,
                    ProductId = cartProduct.ProductId.Value,

                });
            }

            return data;
        }

        [HttpPost("AddToCart/{accountId}")]
        public IActionResult AddToCart([FromRoute] int accountId, [FromBody] AddToCartModel model)
        {
            Resp<CartModel> response = new Resp<CartModel>();

            Cart cart = _db.Carts
                .Include(x => x.CartProducts)
                .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = accountId,
                    Date = DateTime.Now,
                    IsClosed = false,
                    CartProducts = new List<CartProduct>()
                };

                _db.Carts.Add(cart);
            }

            Product product = _db.Products.Find(model.ProductId);

            cart.CartProducts.Add(new CartProduct
            {
                CartId = cart.Id,
                ProductId = product.Id,
                UnitPrice = product.UnitPrice,
                DiscountedPrice = product.DiscountPrice,
                Quantity = model.Quantity,
            });

            _db.SaveChanges();

            CartModel date = CartToCartModel(cart);
            response.Data = date;

            return Ok(response);
        }

    }
}
