using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class DailyModel
    {
        [Key]
        public int DailyId { get; set; }

        [Required]
        public string WeatherCodeDaily { get; set; } = string.Empty;

        [Required]
        public string Date { get; set; } = string.Empty;

        [Required]
        public int Temperature { get; set; }

    }
}
