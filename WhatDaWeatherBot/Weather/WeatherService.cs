using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhatDaWeatherBot.Models;

namespace WhatDaWeatherBot.Weather
{
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
                if (item.Timestamp < startTs) continue;
                if (item.Timestamp >= endTs) break;

                //if (item.Datetime <= startDate) continue;
                //if (item.Datetime > endDate) break;

                weatherForecast.Items.Add(item);
            }

            return weatherForecast;
        }
    }
}