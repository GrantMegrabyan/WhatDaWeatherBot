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
            var query = result.Query;
            var message = await WeatherHub.CheckWeather(query);

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

            var timeRangeEntity = result.Entities
                .SingleOrDefault(a => a.Type == "builtin.weather.time_range");

            var dateRangeEntity = result.Entities
                .SingleOrDefault(a => a.Type == "builtin.weather.date_range");

            weatherQuery.Location = locationEntity?.Entity;

            if (timeRangeEntity != null)
            {
                var fullTimeString = timeRangeEntity.Resolution["time"];
                var indexOfT = fullTimeString.IndexOf("T", StringComparison.Ordinal);
                var timeString = fullTimeString.Substring(indexOfT);

                weatherQuery.TimeRange = TimeRange.Parse(timeString);
            }

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

    public class WeatherQuery
    {
        public string Location { get; set; }
        public DateRange DateRange { get; set; }
        public TimeRange TimeRange { get; set; }

        public bool IsForDate => DateRange != null;
    }

    public class DateRange
    {
        public DateRange()
        {
            
        }

        //public DateRange(DateTime start, DateTime end)
        //{
        //    Start = start;
        //    End = end;
        //}

        public string Name { get; set; }

        public long StartTs { get; set; }
        public long EndTs { get; set; }

        //public DateTime Start { get; set; }
        //public DateTime End { get; set; }

        public DateTimeOffset Start => DateTimeOffset.FromUnixTimeSeconds(StartTs);
        public DateTimeOffset End => DateTimeOffset.FromUnixTimeSeconds(EndTs);

        private static TimeSpan Day => new TimeSpan(1, 0, 0, 0);

        //public bool IsOneDay => Start.Date == End.Date;
        public bool IsOneDay => EndTs - StartTs <= Day.TotalSeconds;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            if (IsOneDay)
            {
                return Start.ToString();
            }
            else
            {
                return $"{Start} - {End}";
            }
        }
    }

    public class TimeRange
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public static TimeRange Parse(string timeString)
        {
            int hour;
            if (int.TryParse(timeString, out hour))
            {
                var time = new TimeSpan(hour, 0, 0);
                return new TimeRange {Start = time, End = time};
            }

            switch (timeString)
            {
                case "MO":
                    return new TimeRange {Start = new TimeSpan(0, 6, 0), End = new TimeSpan(0, 12, 0)};

                case "AF":
                    return new TimeRange {Start = new TimeSpan(0, 12, 0), End = new TimeSpan(0, 18, 0)};

                case "EV":
                    return new TimeRange {Start = new TimeSpan(0, 18, 0), End = new TimeSpan(0, 24, 0)};
            }

            return new TimeRange {Start = new TimeSpan(0, 0, 0), End = new TimeSpan(0, 24, 0)};
        }
    }
}