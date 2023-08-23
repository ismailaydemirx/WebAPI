using ETicaretProject.API.DataAccess;
using ETicaretProject.API.Entities;
using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ETicaretProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin,Merchant")]

    public class ProductController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;

        public ProductController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(Resp<List<ProductModel>>), 200)] // 200 kodu aldığımda döndüğüm nesne
        public IActionResult List()
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();

            //int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

            List<ProductModel> list = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Accounts)
                //.Where(x => x.AccountId == accountId)
                .Select(x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountPrice = x.DiscountPrice,
                    IsContinued = x.IsContinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Accounts.CompanyName
                }).ToList();

            response.Data = list;

            return Ok(response);
        }

        [HttpGet("list/{accountId}")]
        [ProducesResponseType(typeof(Resp<List<ProductModel>>), 200)] // 200 kodu aldığımda döndüğüm nesne
        public IActionResult ListByAccountId([FromRoute] int accountId)
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();

            //int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

            List<ProductModel> list = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Accounts)
                .Where(x => x.AccountId == accountId)
                .Select(x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountPrice = x.DiscountPrice,
                    IsContinued = x.IsContinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Accounts.CompanyName
                }).ToList();

            response.Data = list;

            return Ok(response);
        }


        [HttpGet("get/{productId}")]
        [ProducesResponseType(typeof(Resp<ProductModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 404)] // hatalı durumda döndüğüm nesne
        public IActionResult GetById([FromRoute] int productId)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();

            Product product = _db.Products
                .Include(x => x.Category)
                .Include(x => x.Accounts)
                .SingleOrDefault(x => x.Id == productId);

            if (product == null)
                return NotFound(response);

            ProductModel data = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                DiscountPrice = product.DiscountPrice,
                IsContinued = product.IsContinued,
                CategoryId = product.CategoryId,
                AccountId = product.AccountId,
                CategoryName = product.Category.Name,
                AccountCompanyName = product.Accounts.CompanyName
            };

            response.Data = data;

            return Ok(response);
        }


        [HttpPost("create")]
        [ProducesResponseType(typeof(Resp<ProductModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 400)] // hatalı durudma döndüğüm nesne
        public IActionResult Create([FromBody] ProductCreateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
            string productName = model.Name.Trim().ToLower();

            if (_db.Products.Any(x => x.Name.ToLower() == productName))
            {
                response.AddError(nameof(model.Name), "Bu ürün adı zaten mevcuttur.");
                return BadRequest(response);
            }
            else
            {
                int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

                // 1- Öncelikle product'ı oluşturduk
                Product product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    UnitPrice = model.UnitPrice,
                    DiscountPrice = model.DiscountPrice,
                    IsContinued = model.IsContinued,
                    CategoryId = model.CategoryId,
                    AccountId = accountId
                };

                // 2- Burada insert ettik. Bir product ID'si oluştu
                _db.Products.Add(product);
                _db.SaveChanges();

                // 3- Burada da insert ettiğim product'ın kategori ve account hesaplarını da detaylarıyla getir dedik ve product nesnesine atadık.

                // !!!ÖNEMLİ!!! eğer lazy loading'i açsaydık burada category ve accounts entitylerini include etmemize gerek yoktu direkt product.Category veya product.Accounts şeklinde kullanabilirdik.
                product = _db.Products
                    .Include(x => x.Category)
                    .Include(x => x.Accounts)
                    .SingleOrDefault(x => x.Id == product.Id);

                // ardından da bunu en son eklediğimiz categoryName ve AccountCompanyName ile birlikte çekiyoruz
                ProductModel data = new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    DiscountPrice = product.DiscountPrice,
                    IsContinued = product.IsContinued,
                    CategoryId = product.CategoryId,
                    AccountId = product.AccountId,
                    CategoryName = product.Category.Name,
                    AccountCompanyName = product.Accounts.CompanyName
                };

                response.Data = data;
                return Ok(response);
            }
        }


        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(Resp<ProductModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 400)] // hatalı durumda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 404)] // hatalı durumda döndüğüm nesne
        public IActionResult Update([FromRoute] int id, [FromBody] ProductUpdateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();


            int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);
            string role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            Product product = _db.Products
                .SingleOrDefault(x => x.Id == id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId)));

            if (product == null) // category null ise response 'u geri döneceğiz.
                return NotFound(response); // yukarıda 404 kodu ile hatalı durumu döndürdüğümüz için burada NotFound methodunu kullandık.

            // create ile kategori ismi aynı olmasın diye belirlemiştik anca update kısmında bu yöntem aşılabilir bunun önüne geçmek için burada da kontrol yapmamız gerekiyor.

            string productName = model.Name.Trim()?.ToLower();

            if (_db.Products
                .Any(x => x.Name.ToLower() == productName && x.Id != id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId))))
            {
                response.AddError(nameof(model.Name), "Bu ürün adı zaten mevcuttur.");
                return BadRequest(response); // 400 kodunu döndürüyoruz.
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.UnitPrice = model.UnitPrice;
            product.DiscountPrice = model.DiscountPrice;
            product.IsContinued = model.IsContinued;
            product.CategoryId = model.CategoryId;
            product.AccountId = accountId;

            _db.SaveChanges();

            product = _db.Products
                                .Include(x => x.Category)
                                .Include(x => x.Accounts)
                                .SingleOrDefault(x => x.Id == id);

            // ardından da bunu en son eklediğimiz categoryName ve AccountCompanyName ile birlikte çekiyoruz
            ProductModel data = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                DiscountPrice = product.DiscountPrice,
                IsContinued = product.IsContinued,
                CategoryId = product.CategoryId,
                AccountId = product.AccountId,
                CategoryName = product.Category.Name,
                AccountCompanyName = product.Accounts.CompanyName
            };

            response.Data = data;

            return Ok(response);

        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(Resp<object>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<object>), 404)] // hatalı durumda döndüğüm nesne
        public IActionResult Delete([FromRoute] int id)
        {
            // Response yani geri dönüş değerimizi belirtelim.
            Resp<object> response = new Resp<object>();
            int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);
            string role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            Product product = _db.Products
                .SingleOrDefault(x => x.Id == id && (role == "Admin" || (role != "Admin" && x.AccountId == accountId)));

            if (product == null) // product null ise response 'u geri döneceğiz.
                return NotFound(response); // yukarıda 404 kodu ile hatalı durumu döndürdüğümüz için burada NotFound methodunu kullandık.

            _db.Products.Remove(product);
            _db.SaveChanges();

            return Ok(response);
        }
    }
}
