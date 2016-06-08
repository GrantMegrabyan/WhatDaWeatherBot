namespace WhatDaWeatherBot.Weather
{
    public interface IWeatherResponseParser
    {
        CurrentWeather ParseCurrentWeather(dynamic json);
        WeatherForecast ParseForecast(dynamic json);
    }
}