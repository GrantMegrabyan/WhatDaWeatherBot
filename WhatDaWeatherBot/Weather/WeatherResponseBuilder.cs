using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhatDaWeatherBot.Weather
{
    public class WeatherResponseBuilder : IWeatherResponseBuilder
    {
        public string Build(CurrentWeather currentWeather)
        {
            var response = $"Now in {currentWeather.Location} | {currentWeather.Temperature.ToCelcius()} | {currentWeather.Description}";

            return response;
        }

        public string Build(WeatherForecast weatherForecast, DateRange dateRange)
        {
            if (!weatherForecast.Items.Any())
            {
                return $"Sorry, I have no weather data for {dateRange} in {weatherForecast.Location}";
            }

            if (dateRange.IsOneDay)
            {
                var sb = new StringBuilder($"{dateRange} in {weatherForecast.Location}")
                    .AppendLine(Environment.NewLine);

                foreach (var item in weatherForecast.Items)
                {
                    switch (item.Datetime.Hour)
                    {
                        case 9:
                            sb.AppendLine($"`Morning    | {item.Temperature.ToCelcius()} | {item.Description}`")
                                .AppendLine(Environment.NewLine);
                            break;

                        case 15:
                            sb.AppendLine($"`Afternoon | {item.Temperature.ToCelcius()} | {item.Description}`")
                                .AppendLine(Environment.NewLine);
                            break;

                        case 21:
                            sb.AppendLine($"`Evening     | {item.Temperature.ToCelcius()} | {item.Description}`")
                                .AppendLine(Environment.NewLine);
                            break;

                        case 3:
                            sb.AppendLine($"`Night         | {item.Temperature.ToCelcius()} | {item.Description}`")
                                .AppendLine(Environment.NewLine);
                            break;
                    }
                }

                return sb.ToString();
            }
            else
            {
                var sb = new StringBuilder($"{weatherForecast.Location} on {dateRange}")
                    .AppendLine(Environment.NewLine);

                var days = new Dictionary<string, WeatherDataItem>();
                //var days = new Dictionary<long, WeatherDataItem>();
                var sixHoursInSeconds = Convert.ToInt64(TimeSpan.FromHours(6).TotalSeconds);

                foreach (var item in weatherForecast.Items)
                {
                    //var day = item.Datetime.AddHours(-6).Date;
                    var timestamp = item.Timestamp - sixHoursInSeconds;
                    var dt = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                    var day = dt.DayOfWeek.ToString();

                    if (!days.ContainsKey(day))
                    {
                        days.Add(day, new WeatherDataItem
                        {
                            TemperatureMin = item.TemperatureMin,
                            TemperatureMax = item.TemperatureMax
                        });
                    }
                    else
                    {
                        if (days[day].TemperatureMin > item.TemperatureMin)
                        {
                            days[day].TemperatureMin = item.TemperatureMin;
                        }

                        if (days[day].TemperatureMax < item.TemperatureMax)
                        {
                            days[day].TemperatureMax = item.TemperatureMax;
                        }
                    }
                }

                foreach (var day in days.Keys)
                {
                    var tempMin = days[day].TemperatureMin.ToCelcius();
                    var tempMax = days[day].TemperatureMax.ToCelcius();
                    
                    sb.AppendLine($"{day} | {tempMin}...{tempMax}")   
                        .AppendLine(Environment.NewLine);
                }

                return sb.ToString();
            }
        }





        public string Build(Forecast forecast)
        {
            var response = new StringBuilder($"Forecast in {forecast.City}, {forecast.Country} for 24 hours:");
            response.AppendLine(Environment.NewLine);

            forecast.Items.ForEach(f =>
            {
                var date = f.ForecastOn.ToString("d");
                var time = f.ForecastOn.ToString("t");
                if (time.Length == 4)
                {
                    time = $"0{time}";
                }

                var datetime = $"{time}";
                response.Append($"{datetime}: {f.Temperature:0.0}°C, {f.Description}");
                response.AppendLine(Environment.NewLine);
            });

            return response.ToString();
        }

        public string Build(WeatherConditions conditions)
        {
            var response = $"Now in {conditions.Location} | {conditions.Temperature.ToCelcius()} | {conditions.Description}";

            return response;
        }
    }
}