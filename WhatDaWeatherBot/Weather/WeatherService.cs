using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WhatDaWeatherBot.Weather
{
    public class WeatherData
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Temp { get; set; }
        public string Humidity { get; set; }

    }

    public class Forecast
    {
        public string City { get; set; }
        public string Country { get; set; }

        public List<ForecastItem> Items { get; set; }
    }

    public class ForecastItem
    {
        public DateTimeOffset ForecastOn { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public string Description { get; set; }
    }

    public class WeatherService
    {
        private readonly IWeatherProvider _weatherProvider;

        public WeatherService(IWeatherProvider weatherProvider)
        {
            _weatherProvider = weatherProvider;
        }

        public async Task<CurrentWeather> GetCurrentWeather(
            string locationQuery, CancellationToken cancellationToken)
        {
            var currentWeather = await _weatherProvider
                .GetCurrentAsync(locationQuery, cancellationToken);

            return currentWeather;
        }

        public async Task<WeatherForecast> GetWeatherForecast(
            string locationQuery, DateRange dateRange,
            CancellationToken cancellationToken)
        {
            var fullWeatherForecast = await _weatherProvider
                .GetForecastAsync(locationQuery, cancellationToken);

            var sixHoursInSeconds = Convert.ToInt64(TimeSpan.FromHours(6).TotalSeconds);
            var startTs = dateRange.StartTs + sixHoursInSeconds;
            var endTs = dateRange.EndTs + sixHoursInSeconds;

            //var startDate = dateRange.Start.Date.AddHours(6);
            //var endDate = dateRange.End.Date.AddDays(1).AddHours(6);

            var weatherForecast = new WeatherForecast
            {
                Location = fullWeatherForecast.Location,
                Items = new List<WeatherDataItem>()
            };

            foreach (var item in fullWeatherForecast.Items)
            {
                System.Diagnostics.Trace.TraceInformation($"Item date = {item.Datetime}");

                if (item.Timestamp < startTs) continue;
                if (item.Timestamp >= endTs) break;

                //if (item.Datetime <= startDate) continue;
                //if (item.Datetime > endDate) break;

                weatherForecast.Items.Add(item);
            }

            return weatherForecast;
        }





        public async Task<WeatherConditions> CurrentConditions(
            string locationQuery, CancellationToken cancellationToken)
        {
            var conditions = await _weatherProvider.GetCurrentConditionsAsync(
                locationQuery, cancellationToken);

            return conditions;
        }

        
        public async Task<Forecast> CheckWeather(string location)
        {
            dynamic forecastJson = await _weatherProvider
                .GetForecastAsync(location, CancellationToken.None);

            var forecast = new Forecast
            {
                City = forecastJson["city"]["name"],
                Country = forecastJson["city"]["country"],
                Items = new List<ForecastItem>()
            };

            var maxDate = DateTimeOffset.Now.AddDays(1);

            for (int i = 0; i < forecastJson["list"].Count; i++)
            {
                var item = forecastJson["list"][i];
                var timestamp = (int)item["dt"];
                var forecastOn = DateTimeOffset.FromUnixTimeSeconds(timestamp);

                if (forecastOn > maxDate) break;

                var weather = item["weather"].First;
                string description = weather["description"];

                forecast.Items.Add(new ForecastItem
                {
                    ForecastOn = forecastOn,
                    Temperature = ((float) item["main"]["temp"]) - 273,
                    Humidity = (float)item["main"]["humidity"],
                    Description = description
                });
            }

            return forecast;
        }
    }
}