using WeatherApp_.net_.Context;
using WeatherApp.Models;

namespace WeatherApp.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        public readonly ApplicationDbContext _context;

        public WeatherRepository(ApplicationDbContext context) {
            _context = context;

        }


        public List<WeatherModel> GetAll()
        {
            return _context.weatherLog.ToList();
        }
        public WeatherModel GetById(int id)
        {
            return _context.weatherLog.Find(id);
        }
        public void Update(WeatherModel weather)
        {
            _context.weatherLog.Update(weather);
            _context.SaveChanges();
        }

        public void Delete(WeatherModel weather)
        {
            _context.weatherLog.Remove(weather);
            _context.SaveChanges();
        }

        public void Insert(WeatherModel weather)
        {
            _context.weatherLog.Add(weather);
            _context.SaveChanges();
        }
    }
}
