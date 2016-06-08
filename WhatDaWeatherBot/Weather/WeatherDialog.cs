using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using WhatDaWeatherBot.General;
using WhatDaWeatherBot.Helpers;
using WhatDaWeatherBot.Models;

namespace WhatDaWeatherBot.Weather
{
    [Serializable]
    public class WeatherDialog : LuisDialog<object>
    {
        public WeatherDialog(ILuisService service) 
            : base(service)
        {
            
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var query = new WeatherQuery
            {
                Location = result.Query
            };

            var message = await WeatherHub.GetWeather(query);

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.weather.check_weather")]
        public async Task CheckWeather(IDialogContext context, LuisResult result)
        {
            var weatherQuery = GetWeatherQueryFromLuisResult(result);

            var message = await WeatherHub.GetWeather(weatherQuery);

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        private WeatherQuery GetWeatherQueryFromLuisResult(LuisResult result)
        {
            var weatherQuery = new WeatherQuery();

            var locationEntity = result.Entities
                .SingleOrDefault(a => a.Type == "builtin.weather.absolute_location");
            
            var dateRangeEntity = result.Entities
                .SingleOrDefault(a => a.Type == "builtin.weather.date_range");

            weatherQuery.Location = locationEntity?.Entity;

            if (dateRangeEntity != null)
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                
                weatherQuery.DateRange = new DateRange
                {
                    Name = textInfo.ToTitleCase(dateRangeEntity.Entity)
                };

                var dateValue = dateRangeEntity.Resolution["date"];

                DateTimeOffset dt;
                if (DateTimeOffset.TryParse(dateValue, out dt))
                {
                    var startTs = dt.ToUnixTimeSeconds();
                    var endTs = startTs + Convert.ToInt64(TimeSpan.FromDays(1).TotalSeconds);

                    weatherQuery.DateRange.StartTs = startTs;
                    weatherQuery.DateRange.EndTs = endTs;
                }
                else
                {
                    Regex weekendRegex = new Regex("(\\d{4})-W(\\d{2})-WE");
                    Regex weekRegex = new Regex("(\\d{4})-W(\\d{2})");

                    var weekendMatch = weekendRegex.Match(dateValue);
                    var weekMatch = weekRegex.Match(dateValue);

                    if (weekendMatch.Success)
                    {
                        var year = int.Parse(weekendMatch.Groups[1].Value);
                        var weekNumber = int.Parse(weekendMatch.Groups[2].Value);

                        var firstDayOfWeek = DateTimeHelper.FirstDayOfWeek(year, weekNumber);
                        var timestamp = firstDayOfWeek.ToUnixTimeSeconds();

                        var startTs = timestamp + Convert.ToInt64(TimeSpan.FromDays(5).TotalSeconds);
                        var endTs = timestamp + Convert.ToInt64(TimeSpan.FromDays(7).TotalSeconds);

                        weatherQuery.DateRange.StartTs = startTs;
                        weatherQuery.DateRange.EndTs = endTs;
                    }
                    else if (weekMatch.Success)
                    {
                        var year = int.Parse(weekMatch.Groups[1].Value);
                        var weekNumber = int.Parse(weekMatch.Groups[2].Value);

                        var firstDayOfWeek = DateTimeHelper.FirstDayOfWeek(year, weekNumber);
                        var timestamp = firstDayOfWeek.ToUnixTimeSeconds();

                        var startTs = timestamp;
                        var endTs = timestamp + Convert.ToInt64(TimeSpan.FromDays(7).TotalSeconds);

                        weatherQuery.DateRange.StartTs = startTs;
                        weatherQuery.DateRange.EndTs = endTs;
                    }
                    else
                    {
                        weatherQuery.DateRange.Name = "Today";

                        var startTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        var endTs = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();

                        weatherQuery.DateRange.StartTs = startTs;
                        weatherQuery.DateRange.EndTs = endTs;
                    }
                    
                }

            }

            //weatherQuery.DateRange = new DateRange(
            //    DateTime.Now.AddDays(1).Date, DateTime.Now.AddDays(5).Date);
            
            return weatherQuery;
        }
    }
}