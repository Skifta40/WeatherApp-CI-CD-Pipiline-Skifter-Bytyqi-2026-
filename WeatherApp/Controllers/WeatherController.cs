using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WeatherApp.Models;
using WeatherApp.Repositories;
using WeatherApp_.net_.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;

namespace WeatherApp_.net_.Controllers
{

    public class WeatherController : Controller
    {
        private readonly string OpenGate_apiKey;
        private readonly string OpenMeteo_baseUrl;
        private readonly string GeoCodeAPI_baseUrl;

        public readonly IWeatherRepository _weatherRepository;


        private static readonly HttpClient _httpClient = new HttpClient();
        public WeatherController(IWeatherRepository weatherRepository, IConfiguration configuration)
        {
            _weatherRepository = weatherRepository;
            OpenGate_apiKey = configuration["GeoCodeAPI:ApiKey"]
                ?? throw new ArgumentNullException("API Key is missing from configuration!");
            OpenMeteo_baseUrl = configuration["OpenMeteo:BaseUrl"]
                ?? throw new ArgumentNullException("Base URL is missing!");
            GeoCodeAPI_baseUrl = configuration["OpenCage:BaseUrl"]
                ?? throw new ArgumentNullException("GeoCode API Base URL is missing!");
        }
        private static readonly string[] Codes = CreateCodes();

        private static string[] CreateCodes()
        {
            var arr = Enumerable.Repeat("unknown", 100).ToArray();

            arr[0] = "sunny";
            arr[1] = "partly-cloudy";
            arr[2] = "partly-cloudy";
            arr[3] = "overcast";
            arr[45] = "fog";
            arr[48] = "fog";
            arr[51] = "rain";
            arr[53] = "rain";
            arr[55] = "rain";
            arr[56] = "rain";
            arr[57] = "rain";
            arr[61] = "rain";
            arr[63] = "rain";
            arr[65] = "rain";
            arr[66] = "rain";
            arr[67] = "rain";
            arr[71] = "snow";
            arr[73] = "snow";
            arr[75] = "snow";
            arr[77] = "snow";
            arr[85] = "snow";
            arr[86] = "snow";
            arr[80] = "rain";
            arr[81] = "rain";
            arr[82] = "rain";
            arr[95] = "storm";
            arr[96] = "storm";
            arr[99] = "storm";

            return arr;
        }

        public IActionResult Submit(WHDViewModel model)
        {
            var vm = new WHDViewModel
            {
                Weather = model.Weather,
                Hourly = model.Hourly
            };
            return View("~/Views/Home/Index.cshtml", vm);
        }

        [HttpPost]
        public async Task<IActionResult> GetWeatherData(WHDViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Weather.CityName))
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Please enter a city name."
                });

            var geocodeKey = OpenGate_apiKey;
            var searchCity = model.Weather.CityName;

            var geoCodeQueryParams = new Dictionary<string, string?>
            {
                { "q", searchCity },
                { "key", geocodeKey },
                { "language", "en" },
                { "pretty", "1" }
            };

            var geocodeUrl = QueryHelpers.AddQueryString("https://api.opencagedata.com/geocode/v1/json", geoCodeQueryParams);
            var geocodeResponse = await _httpClient.GetAsync(geocodeUrl);

            if (!geocodeResponse.IsSuccessStatusCode)
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = $"Geocoding service error: {geocodeResponse.StatusCode}"
                });

            var geocodeData = await geocodeResponse.Content.ReadAsStringAsync();

            JObject geocodeJson;
            try
            {
                geocodeJson = JObject.Parse(geocodeData);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Failed to parse geocode data"
                });
            }

            var results = geocodeJson["results"] as JArray;
            if (results == null || results.Count == 0)
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "City not found. Please try a different search." + geocodeUrl
                });

            var firstResult = results[0];
            var latitude = firstResult["geometry"]?["lat"]?.ToString();
            var longitude = firstResult["geometry"]?["lng"]?.ToString();

            if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Invalid geocode response"
                });

            var weatherQueryParams = new Dictionary<string, string?>
            {
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "current_weather", "true" },
                { "hourly", "temperature_2m,weathercode,precipitation,relative_humidity_2m" },
                { "daily", "temperature_2m_max,temperature_2m_min,weathercode" },
                { "timezone", "auto" }
            };
            var tempUrl = QueryHelpers.AddQueryString($"{OpenMeteo_baseUrl}forecast", weatherQueryParams);
            var tempResponse = await _httpClient.GetAsync(tempUrl);

            if (!tempResponse.IsSuccessStatusCode)
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = $"Weather service error: {tempResponse.StatusCode}"
                });

            var tempData = await tempResponse.Content.ReadAsStringAsync();


            JObject tempJson;
            try
            {
                tempJson = JObject.Parse(tempData);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Failed to parse temperature data"
                });
            }

            var currentWeather = tempJson["current_weather"];
            if (currentWeather == null)
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Missing current weather data"
                });



            double tempNow = (double?)currentWeather["temperature"] ?? 0;
            double windSpeedValue = (double?)currentWeather["windspeed"] ?? 0;
            int weatherCodeValue = (int?)currentWeather["weathercode"] ?? 0;


            JArray temperatures = (JArray)tempJson["hourly"]?["temperature_2m"];
            JArray humidities = (JArray)tempJson["hourly"]?["relative_humidity_2m"];
            JArray precipitations = (JArray)tempJson["hourly"]?["precipitation"];
            JArray weatherCodes = (JArray)tempJson["hourly"]?["weathercode"];
            JArray timeStamps = (JArray)tempJson["hourly"]?["time"];
            JArray dates = (JArray)tempJson["daily"]?["time"];
            JArray maxTemps = (JArray)tempJson["daily"]?["temperature_2m_max"];
            JArray minTemps = (JArray)tempJson["daily"]?["temperature_2m_min"];
            JArray dailyWeatherCodes = (JArray)tempJson["daily"]?["weathercode"];


            if (temperatures == null || humidities == null || precipitations == null || weatherCodes == null || timeStamps == null || dates == null || maxTemps == null || minTemps == null || dailyWeatherCodes == null)
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = "Weather data missing"
                });


            double temperature = (double)temperatures[0];
            int humidityValue = (int)humidities[0];
            double precipitationValue = (double)precipitations[0];
            string weathercodeS = GetWeatherDescription(weatherCodeValue);

            DateTime[] DailyDate = dates.Select(t => DateTime.Parse((string)t)).ToArray();
            int maxTempValue = (int)Math.Round((double)maxTemps[0]);
            int minTempValue = (int)Math.Round((double)minTemps[0]);


            WeatherModel weather = new WeatherModel
            {
                CityName = model.Weather.CityName,
                Temperature = (int)Math.Round(tempNow),
                RequestTimeStamp = DateTime.Now,
                Humidity = humidityValue,
                Precipitation = (int)Math.Round(precipitationValue),
                WindSpeed = (int)Math.Round(windSpeedValue),
                WeatherCode = weathercodeS + ".webp"
            };


            DateTime[] HTime = timeStamps.Select(t => DateTime.Parse((string)t)).ToArray();

            List<HourlyModel> hourly = new List<HourlyModel>();
            for (int i = 0; i < HTime.Length; i++)
            {
                int weatherCodesINT = (int)weatherCodes[i];
                string weatherDescription = GetWeatherDescription(weatherCodesINT);

                var hourlyModel = new HourlyModel
                {
                    Time = TimeOnly.FromDateTime(HTime[i]),
                    Temperature = (int)Math.Round((double)temperatures[i]),
                    WeatherCode = weatherDescription + ".webp"
                };
                hourly.Add(hourlyModel);
            }

            List<DailyModel> daily = new List<DailyModel>();
            for (int i = 0; i < DailyDate.Length; i++)
            {
                int weatherCodesINT = (int)dailyWeatherCodes[i];
                string weatherDescription = GetWeatherDescription(weatherCodesINT);
                string dayname = DailyDate[i].DayOfWeek.ToString();

                int AvgTemp = (int)Math.Round(((double)maxTemps[i] + (double)minTemps[i]) / 2);

                var dailyModel = new DailyModel
                {
                    Date = dayname,
                    Temperature = AvgTemp,
                    WeatherCodeDaily = weatherDescription + ".webp"
                };
                daily.Add(dailyModel);
            }



            await InsertWeatherAsync(weather);
            var vm = new WHDViewModel
            {
                Weather = weather,
                Hourly = hourly,
                Daily = daily
            };

            return View("~/Views/Home/Index.cshtml", vm);
        }

        private static string GetWeatherDescription(int code)
        {
            if (code >= 0 && code < Codes.Length)
            {
                return Codes[code];
            }
            return "unknown";
        }

        [HttpPost]
        public async Task<IActionResult> InsertWeatherAsync(WeatherModel weather)
        {
            _weatherRepository.Insert(weather);
            return View();
        }

    }
}