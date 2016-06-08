using System.Collections.Generic;

namespace WhatDaWeatherBot.Weather
{
    public class WeatherForecast
    {
        public Location Location { get; set; }

        public IList<WeatherDataItem> Items { get; set; }
    }
}