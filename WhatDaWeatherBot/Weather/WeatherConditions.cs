namespace WhatDaWeatherBot.Weather
{
    public class WeatherConditions
    {
        public Location Location { get; set; }

        public string Description { get; set; }

        public Temperature Temperature { get; set; }
        public Temperature TemperatureMin { get; set; }
        public Temperature TemperatureMax { get; set; }
    }
}