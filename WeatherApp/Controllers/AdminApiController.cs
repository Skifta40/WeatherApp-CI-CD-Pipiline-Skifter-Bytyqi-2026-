using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Repositories;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IWeatherRepository _weatherRepository;

        public AdminApiController(IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }

        [HttpGet("GetRecords")]
        public IActionResult GetRecords()
        {
            var records = _weatherRepository.GetAll();
            return Ok(records);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRecord(int id)
        {
            try
            {
                var weather = _weatherRepository.GetById(id);
                if (weather == null)
                {
                    return NotFound(new { success = false, message = "Record not found" });
                }
                _weatherRepository.Delete(weather);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Deletion failed" });
            }
        }

        [HttpPost("Edit")]
        public IActionResult Edit(WeatherModel updatedRecord)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
            }
            _weatherRepository.Update(updatedRecord);
            return Ok(new { success = true });
        }
    }
}