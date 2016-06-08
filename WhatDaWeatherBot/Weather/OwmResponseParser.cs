using System;
using System.Collections.Generic;
using WhatDaWeatherBot.Models;

namespace WhatDaWeatherBot.Weather
{
    /// <summary>
    /// OpenWeatherMap json response parser
    /// </summary>
    public class OwmResponseParser : IWeatherResponseParser
    {
        public CurrentWeather ParseCurrentWeather(dynamic json)
        {
            var location = new Location(json["name"].ToString(), json["sys"]["country"].ToString());
            var description = json["weather"].First["description"];

            var temp = Temperature.FromKelvin(decimal.Parse(json["main"]["temp"].ToString()));
            var tempMin = Temperature.FromKelvin(decimal.Parse(json["main"]["temp_min"].ToString()));
            var tempMax = Temperature.FromKelvin(decimal.Parse(json["main"]["temp_max"].ToString()));

            var currentWeather = new CurrentWeather
            {
                Location = location,
                Description = description,
                Temperature = temp,
                TemperatureMin = tempMin,
                TemperatureMax = tempMax
            };

            return currentWeather;
        }

        public WeatherForecast ParseForecast(dynamic json)
        {
            var location = new Location(json["city"]["name"].ToString(), json["city"]["country"].ToString());
            
            var forecast = new WeatherForecast
            {
                Location = location,
                Items = new List<WeatherDataItem>()
            };

            for (int i = 0; i < json["list"].Count; i++)
            {
                var item = json["list"][i];
                var timestamp = (long)item["dt"];
                var forecastOn = DateTimeOffset.FromUnixTimeSeconds(timestamp);

                var weather = item["weather"].First;
                string description = weather["description"];

                var temp = Temperature.FromKelvin(decimal.Parse(item["main"]["temp"].ToString()));
                var tempMin = Temperature.FromKelvin(decimal.Parse(item["main"]["temp_min"].ToString()));
                var tempMax = Temperature.FromKelvin(decimal.Parse(item["main"]["temp_max"].ToString()));
                var humidity = decimal.Parse(item["main"]["humidity"].ToString());
                var pressure = decimal.Parse(item["main"]["pressure"].ToString());

                forecast.Items.Add(new WeatherDataItem
                {
                    Datetime = forecastOn,
                    Timestamp = timestamp,
                    Description = description,

                    Temperature = temp,
                    TemperatureMax = tempMax,
                    TemperatureMin = tempMin,

                    Humidity = humidity,
                    Pressure = pressure
                });
            }

            return forecast;
        }
    }
}