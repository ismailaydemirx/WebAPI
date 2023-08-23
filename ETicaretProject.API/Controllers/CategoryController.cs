using ETicaretProject.API.DataAccess;
using ETicaretProject.API.Entities;
using ETicaretProject.Core;
using ETicaretProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ETicaretProject.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Resp<CategoryModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
    [ProducesResponseType(typeof(Resp<CategoryModel>), 400)] // hatalı durudma döndüğüm nesne
    public class CategoryController : ControllerBase
    {
        private DatabaseContext _db;
        private IConfiguration _configuration;

        public CategoryController(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _db = databaseContext;
            _configuration = configuration;
        }
        [HttpPost("create")]
        public IActionResult Create([FromBody] CategoryCreateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
            string categoryName = model.Name.Trim().ToLower();

            if (_db.Categories.Any(x => x.Name.ToLower() == categoryName))
            {
                response.AddError(nameof(model.Name), "Bu kategori adı zaten mevcuttur.");
                return BadRequest(response);
            }
            else
            {
                Category category = new Category
                {
                    Name = model.Name,
                    Description = model.Description
                };
                _db.Categories.Add(category);
                _db.SaveChanges();

                ProductModel data = new ProductModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };

                response.Data = data;
                return Ok(response);
            }
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(Resp<List<ProductModel>>), 200)] // 200 kodu aldığımda döndüğüm nesne
        public IActionResult List()
        {
            Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();
            List<ProductModel> list = _db.Categories.Select(
                x => new ProductModel { Id = x.Id, Name = x.Name, Description = x.Description }).ToList();

            response.Data = list;

            return Ok(response);
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType(typeof(Resp<ProductModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 404)] // hatalı durumda döndüğüm nesne
        public IActionResult GetById([FromRoute] int id)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();
            Category category = _db.Categories.SingleOrDefault(x => x.Id == id);
            ProductModel data = null;

            if (category == null)
                return NotFound(response);

            data = new ProductModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            response.Data = data;

            return Ok(response);
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(Resp<ProductModel>), 200)] // 200 kodu aldığımda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 400)] // hatalı durumda döndüğüm nesne
        [ProducesResponseType(typeof(Resp<ProductModel>), 404)] // hatalı durumda döndüğüm nesne
        public IActionResult Update([FromRoute] int id, [FromBody] CategoryUpdateModel model)
        {
            Resp<ProductModel> response = new Resp<ProductModel>();

            Category category = _db.Categories.Find(id);

            if (category == null) // category null ise response 'u geri döneceğiz.
                return NotFound(response); // yukarıda 404 kodu ile hatalı durumu döndürdüğümüz için burada NotFound methodunu kullandık.

            // create ile kategori ismi aynı olmasın diye belirlemiştik anca update kısmında bu yöntem aşılabilir bunun önüne geçmek için burada da kontrol yapmamız gerekiyor.

            string categoryName = model.Name.Trim()?.ToLower();

            if (_db.Categories.Any(x => x.Name.ToLower() == categoryName && x.Id != id))
            {
                response.AddError(nameof(model.Name), "Bu kategori adı zaten mevcuttur.");
                return BadRequest(response); // 400 kodunu döndürüyoruz.
            }

            category.Name = model.Name;
            category.Description = model.Description;

            _db.SaveChanges();

            ProductModel data = new ProductModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
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

            // Önce silmek istediğimiz kategoriyi bulmalıyız.
            Category category = _db.Categories.Find(id);

            if (category == null)
                return NotFound(response);

            _db.Categories.Remove(category);
            _db.SaveChanges();

            return Ok(response);
        }
    }
}
