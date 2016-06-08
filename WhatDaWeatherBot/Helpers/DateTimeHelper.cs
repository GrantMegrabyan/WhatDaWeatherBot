using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WhatDaWeatherBot.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Taken from http://stackoverflow.com/questions/662379/calculate-date-from-week-number
        /// </summary>
        /// <param name="year"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public static DateTimeOffset FirstDayOfWeek(int year, int week)
        {
            var jan1 = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday.UtcDateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = week;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
    }
}