using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WhatDaWeatherBot.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DateTime dt;
            DateTime.TryParse("2016-W22-WE", out dt);

            Console.WriteLine(dt);
        }
    }
}
