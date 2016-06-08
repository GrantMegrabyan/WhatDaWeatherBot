using System.Collections.Generic;

namespace WhatDaWeatherBot.Models
{
    public class WeatherForecast
    {
        public Location Location { get; set; }

        public IList<WeatherDataItem> Items { get; set; }
    }
}