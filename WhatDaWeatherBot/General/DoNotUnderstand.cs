using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatDaWeatherBot.General
{
    public static class DoNotUnderstand
    {
        private static readonly string[] Phrases = new[]
        {
            "I have no idea what do you want from me",
            "Are you talking to me? Try one more time",
            "What do you meen '{0}'?"
        };

        public static string GetMessage(string input = null)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var count = Phrases.Length;

            var index = rnd.Next(0, count);
            return string.Format(Phrases[index], input);
        }
    }
}