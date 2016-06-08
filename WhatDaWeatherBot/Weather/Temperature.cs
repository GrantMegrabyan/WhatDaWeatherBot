using System;
using System.Web.DynamicData;

namespace WhatDaWeatherBot.Weather
{
    public class Temperature : IComparable, IComparable<Temperature>
    {
        public decimal Value { get; set; }
        public TemperatureUnit Unit { get; set; }

        public static Temperature FromCelcius(decimal value)
        {
            return new Temperature
            {
                Value = value,
                Unit = TemperatureUnit.Celcius
            };
        }

        public static Temperature FromKelvin(decimal value)
        {
            return new Temperature
            {
                Value = value,
                Unit = TemperatureUnit.Kelvin
            };
        }

        public static Temperature FromFahrenheit(decimal value)
        {
            return new Temperature
            {
                Value = value,
                Unit = TemperatureUnit.Fahrenheit
            };
        }

        public Temperature ToCelcius()
        {
            var value = Value;
            
            if (Unit == TemperatureUnit.Kelvin)
            {
                value = Value - 273;
            }

            if (Unit == TemperatureUnit.Fahrenheit)
            {
                value = (Value - 30) / 2;
            }

            return Temperature.FromCelcius(value);
        }

        public int CompareTo(Temperature other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return 1;
            }

            var thisInCelcius = ToCelcius();
            var otherInCelcius = other.ToCelcius();

            return decimal.Compare(thisInCelcius.Value, otherInCelcius.Value);
        }

        public override string ToString()
        {
            var value = Math.Round(Value, 0);
            var sign = new[] {"-", "", "+"}[Math.Sign(value)+1];

            return $"{sign}{value}°{Unit.ToString()[0]}";
        }

        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            Temperature other = obj as Temperature; // avoid double casting
            if (other == null)
            {
                throw new ArgumentException("A Temperature object is required for comparison.", nameof(obj));
            }

            return this.CompareTo(other);
        }

        public static int Compare(Temperature left, Temperature right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return 0;
            }
            if (object.ReferenceEquals(left, null))
            {
                return -1;
            }
            return left.CompareTo(right);
        }

        // Omitting any of the following operator overloads 
        // violates rule: OverrideMethodsOnComparableTypes.
        public static bool operator ==(Temperature left, Temperature right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(Temperature left, Temperature right)
        {
            return !(left == right);
        }
        public static bool operator <(Temperature left, Temperature right)
        {
            return (Compare(left, right) < 0);
        }
        public static bool operator >(Temperature left, Temperature right)
        {
            return (Compare(left, right) > 0);
        }
    }
}