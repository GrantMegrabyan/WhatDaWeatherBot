using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhatDaWeatherBot.Weather
{
    public class OwmWeatherProvider : IWeatherProvider
    {
        private readonly IWeatherResponseParser _responseParser;

        private string _apiBaseUrl = "http://api.openweathermap.org/data/2.5/";
        private readonly string _appKey;

        public OwmWeatherProvider(
            string appKey,
            IWeatherResponseParser responseParser)
        {
            _appKey = appKey;
            _responseParser = responseParser;
        }

        public async Task<CurrentWeather> GetCurrentAsync(
            string locationQuery, CancellationToken cancellationToken)
        {
            var url = BuildCurrentUrl(locationQuery);
            var json = await ExecuteApiCall(url);

            return _responseParser.ParseCurrentWeather(json);
        }

        public async Task<WeatherConditions> GetCurrentConditionsAsync(
            string locationQuery, CancellationToken cancellationToken)
        {
            var url = BuildCurrentUrl(locationQuery);
            var json = await ExecuteApiCall(url);

            var location = new Location(json["name"].ToString(), json["sys"]["country"].ToString());
            var description = json["weather"].First["description"];

            var temp = Temperature.FromKelvin(decimal.Parse(json["main"]["temp"].ToString()));
            var tempMin = Temperature.FromKelvin(decimal.Parse(json["main"]["temp_min"].ToString()));
            var tempMax = Temperature.FromKelvin(decimal.Parse(json["main"]["temp_max"].ToString()));

            var conditions = new WeatherConditions
            {
                Location = location,
                Description = description,
                Temperature = temp,
                TemperatureMin = tempMin,
                TemperatureMax = tempMax
            };

            return conditions;
        }

        public async Task<WeatherForecast> GetForecastAsync(
            string locationQuery, CancellationToken cancellationToken)
        {
            var url = BuildForecastUrl(locationQuery);
            var json = await ExecuteApiCall(url);

            return _responseParser.ParseForecast(json);
        }

        private async Task<dynamic> ExecuteApiCall(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);

                var reader = new JsonTextReader(new StringReader(response));
                var jsonSerializer = new JsonSerializer();
                dynamic json = jsonSerializer.Deserialize<dynamic>(reader);

                return json;
            }
        }

        private string BuildCurrentUrl(string locationQuery)
        {
            return $"{_apiBaseUrl}weather?appid={_appKey}&q={locationQuery}";
        }

        private string BuildForecastUrl(string locationQuery)
        {
            return $"{_apiBaseUrl}forecast?appid={_appKey}&q={locationQuery}";
        }
    }
}