using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TodoApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "092f0185652d400bb4d85641242104";

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherData?> GetWeatherAsync(double latitude, double longitude)
        {
            var url = $"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={latitude},{longitude}";
            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);

            if (response?.Current == null) return null;

            // Map to WeatherData
            return new WeatherData
            {
                TemperatureC = response.Current.TempC,
                Condition = response.Current.Condition.Text 
            };
        }
    }

    public class WeatherResponse
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public class Current
    {
        public double TempC { get; set; }
        public ConditionData Condition { get; set; }
    }

    public class ConditionData
    {
        public string Text { get; set; }
        public string Icon { get; set; } // Optional
    }
}
