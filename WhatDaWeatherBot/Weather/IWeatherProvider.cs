using System.Threading;
using System.Threading.Tasks;
using WhatDaWeatherBot.Models;

namespace WhatDaWeatherBot.Weather
{
    public interface IWeatherProvider
    {
        Task<CurrentWeather> GetCurrentAsync(
            string locationQuery, CancellationToken cancellationToken);

        Task<WeatherForecast> GetForecastAsync(
            string locationQuery, CancellationToken cancellationToken);
    }
}