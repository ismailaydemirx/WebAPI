using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication_BasicAuth.Controllers
{
    [Authorize(Roles ="admin,manager,user")] // Bu kontroller içindeki tüm methodlarda doğrulama yap anlamına gelir. Roller de ekleyebiliyoruz örneğin eklediğimiz role claim'lerinde kullanıcının role = admin ya da manager ise doğrulama sağlanır.
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //[AllowAnonymous] // bu controller'da herhangi bir doğrulama yapılmasın anlamına gelir yani herkes kullanabilir.
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
