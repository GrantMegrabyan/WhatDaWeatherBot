namespace WhatDaWeatherBot.Models
{
    public class Location
    {
        public Location(string city, string country)
        {
            City = city;
            Country = country;
        }

        public string City { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return $"{City}, {Country}";
        }
    }
}