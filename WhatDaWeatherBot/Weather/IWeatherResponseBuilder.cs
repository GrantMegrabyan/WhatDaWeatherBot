namespace WhatDaWeatherBot.Weather
{
    public interface IWeatherResponseBuilder
    {
        string Build(CurrentWeather currentWeather);
        string Build(WeatherForecast weatherForecast, DateRange dateRange);

        string Build(Forecast forecast);
        string Build(WeatherConditions conditions);
    }
}