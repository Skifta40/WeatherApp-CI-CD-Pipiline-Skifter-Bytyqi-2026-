namespace WeatherApp.Models
{
    public class WHDViewModel
    {
        public WeatherModel Weather { get; set; }
        public List<HourlyModel> Hourly { get; set; }
        public List<DailyModel> Daily { get; set; }
    }
}
