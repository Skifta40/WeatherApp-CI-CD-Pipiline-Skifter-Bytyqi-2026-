using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class HourlyModel
    {
        [Key]
        public int HourlyId { get; set; }

        [Required]
        public TimeOnly Time { get; set; }

        [Required]
        public int Temperature { get; set; }

        [Required]
        public string WeatherCode { get; set; } = string.Empty;
    }
}
