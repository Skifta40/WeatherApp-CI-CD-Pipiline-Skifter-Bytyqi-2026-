using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Repositories;

namespace WeatherApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWeatherRepository _weatherRepository;

        public AdminController(IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var weatherLogs = _weatherRepository.GetAll();
            return View("IndexAdmin", weatherLogs);
        }
    }
}
