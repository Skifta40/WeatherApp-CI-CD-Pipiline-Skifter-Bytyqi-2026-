using WeatherApp.Models;

namespace WeatherApp.Repositories
{
    public interface IWeatherRepository
    {
        List<WeatherModel> GetAll();
        WeatherModel GetById(int id);
        void Update(WeatherModel weather);
        void Delete(WeatherModel weather);
        void Insert(WeatherModel weather);
    }
}
