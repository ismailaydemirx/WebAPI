using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebApplication_ActionFilterKullanimi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [LogActionFilter] // aşağıdaki controller'ın içerisindeki tüm metodlarda bizim burada eklediğimiz LogActionFilter sınıfı çalışsın dedik.
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

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

    public class LogActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string controller = context.ActionDescriptor.RouteValues["controller"];
            string action = context.ActionDescriptor.RouteValues["action"];
            string path = context.HttpContext.Request.Path.Value;

            Debug.WriteLine($"My Log(OnActionExecuting): Controller: {controller}, Action: {action}, Path: {path}, Date: {DateTime.Now}");

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string controller = context.ActionDescriptor.RouteValues["controller"];
            string action = context.ActionDescriptor.RouteValues["action"];
            string path = context.HttpContext.Request.Path.Value;

            Debug.WriteLine($"My Log(OnActionExecuted): Controller: {controller}, Action: {action}, Path: {path}, Date: {DateTime.Now}");

            base.OnActionExecuted(context);
        }
    }
}
