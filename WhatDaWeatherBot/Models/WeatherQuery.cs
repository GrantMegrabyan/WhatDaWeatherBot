namespace WhatDaWeatherBot.Models
{
    public class WeatherQuery
    {
        public string Location { get; set; }

        public DateRange DateRange { get; set; }

        public bool IsForDate => DateRange != null;
    }
}