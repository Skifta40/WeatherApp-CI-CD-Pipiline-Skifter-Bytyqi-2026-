using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{ 
    public class WeatherModel
    {
        [Key]
        public int LogID { get; set; }

        public string CityName { get; set; } = string.Empty;

        [Required]
        public float Temperature { get; set; }

        [Required]
        public DateTime RequestTimeStamp { get; set; }

        [Required]
        public float Precipitation { get; set; }

        [Required]
        public float Humidity { get; set; }

        [Required]
        public int WindSpeed { get; set; }

        [Required]
        public string WeatherCode { get; set; } = string.Empty;
    }
}
