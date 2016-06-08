using System.Threading;
using System.Threading.Tasks;

namespace WhatDaWeatherBot.Weather
{
    public interface IWeatherProvider
    {
        Task<CurrentWeather> GetCurrentAsync(
            string locationQuery, CancellationToken cancellationToken);

        Task<WeatherForecast> GetForecastAsync(string locationQuery, CancellationToken cancellationToken);

        Task<WeatherConditions> GetCurrentConditionsAsync(
            string locationQuery, CancellationToken cancellationToken);
    }
}