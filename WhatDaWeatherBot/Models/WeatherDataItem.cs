using System;

namespace WhatDaWeatherBot.Models
{
    public class WeatherDataItem
    {
        public string Description { get; set; }

        public Temperature Temperature { get; set; }
        public Temperature TemperatureMin { get; set; }
        public Temperature TemperatureMax { get; set; }

        public decimal Pressure { get; set; }
        public decimal Humidity { get; set; }

        public DateTimeOffset Datetime { get; set; }
        public long Timestamp { get; set; }

        public string ToTempRangeString()
        {
            return $"{TemperatureMin}...{TemperatureMax}";
        }
    }
}