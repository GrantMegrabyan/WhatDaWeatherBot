using WhatDaWeatherBot.Models;

namespace WhatDaWeatherBot.Weather
{
    public interface IWeatherResponseBuilder
    {
        string Build(CurrentWeather currentWeather);
        string Build(WeatherForecast weatherForecast, DateRange dateRange);
    }
}