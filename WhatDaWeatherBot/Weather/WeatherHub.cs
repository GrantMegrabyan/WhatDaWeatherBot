using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WhatDaWeatherBot.Models;

namespace WhatDaWeatherBot.Weather
{
    public static class WeatherHub
    {
        public static async Task<string> GetWeather(WeatherQuery query)
        {
            var service = GetWeatherService();
            var responseBuilder = GetResponseBuilder();

            if (query.IsForDate)
            {
                var weatherForecast = await service.GetWeatherForecast(
                    query.Location, query.DateRange, CancellationToken.None);

                var message = responseBuilder.Build(weatherForecast, query.DateRange);

                return message;
            }
            else
            {
                var currentWeather = await service.GetCurrentWeather(
                    query.Location, CancellationToken.None);

                var message = responseBuilder.Build(currentWeather);

                return message;
            }
        }
        
        private static WeatherService GetWeatherService()
        {
            var owmKey = ConfigurationManager.AppSettings["OwmKey"];

            IWeatherResponseParser responseParser = new OwmResponseParser();
            IWeatherProvider provider = new OwmWeatherProvider(owmKey, responseParser);
            return new WeatherService(provider);
        }

        private static IWeatherResponseBuilder GetResponseBuilder()
        {
            return new WeatherResponseBuilder();
        }
    }
}