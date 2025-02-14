using Newtonsoft.Json.Linq;

namespace BlockedCountriesAPI.Services
{
    public class GeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        public GeolocationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["GeolocationAPI:BaseUrl"];
            _apiKey = configuration["GeolocationAPI:ApiKey"];
        }

        public async Task<string> GetCountryCodeByIpAsync(string ipAddress)
        {
            var url = $"{_apiBaseUrl}/{ipAddress}/json?key={_apiKey}";

            var response = await _httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);
            return json["country_code"]?.ToString();
        }
    }
}
