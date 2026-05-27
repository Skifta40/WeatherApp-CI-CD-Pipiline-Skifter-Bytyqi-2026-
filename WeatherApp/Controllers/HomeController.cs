using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WeatherApp.Models;
using WeatherApp_.net_.Models;

namespace WeatherApp_.net_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new WHDViewModel
            {
                Weather = new WeatherModel  
                {
                    CityName = string.Empty,
                    Temperature = 0,
                    RequestTimeStamp = DateTime.Now,
                    Precipitation = 0,
                    Humidity = 0,
                    WindSpeed = 0,
                    WeatherCode = "none.webp"  
                },
                Hourly = new List<HourlyModel>() 
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
