using System;

namespace WhatDaWeatherBot.Models
{
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
}