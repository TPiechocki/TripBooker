﻿using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium_Demo;
using System.Threading;

namespace TripBooker.AutomatedTests
{
    internal class TripsTests
    {
        private IWebDriver _driver = null!;

        [OneTimeSetUp]
        public void SetUp()
        {
            // Local Selenium WebDriver
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            _driver = new ChromeDriver(chromeOptions);
            _driver.Manage().Window.Maximize();
            _driver.Url = $"{TestConsts.Url}/trips/";
        }

        [Test]
        public void ShouldDisplayTripsWithDefaultFilterValues()
        {
            Thread.Sleep(2000);

            // Button
            var button = _driver.FindElement(By.XPath("//button[text()='Search Trips']"));
            button.Click();

            Thread.Sleep(2000);

            var trips = _driver.FindElements(By.ClassName("MuiGrid-container")).Last();
            trips.FindElements(By.XPath(".//*")).Should().NotBeEmpty();
        }

        [Test]
        public void AllTripsShouldHavePositivePrices()
        {
            Thread.Sleep(2000);

            // Button
            var button = _driver.FindElement(By.XPath("//button[text()='Search Trips']"));
            button.Click();

            Thread.Sleep(2000);


            var trips = _driver.FindElements(By.ClassName("MuiGrid-container")).Last();
            var prices = trips.FindElements(By.XPath("//p[contains(text(),'From:')]/following-sibling::p"));
            var pricesTexts = prices.Select(x => x.Text).ToList();

            pricesTexts.Should().NotBeEmpty();
            pricesTexts.Select(x => double.Parse(x.Replace("§", string.Empty), CultureInfo.InvariantCulture))
                    .Should().OnlyContain(x => x > 0);
        }

        [Test]
        public void ShouldHaveNoTripsWhenThereIs0Days()
        {
            Thread.Sleep(2000);

            var searchText = _driver.FindElement(By.XPath("//h6[text()='Choose offer details']"));

            searchText.Should().NotBeNull();

            // Number of days
            var days = _driver.FindElement(By.XPath("//label[contains(text(),'Number of days')]/following-sibling::div"))
                .FindElement(By.CssSelector("input"));
            days.SendKeys(Keys.Control + "a");
            days.SendKeys(Keys.Delete);
            days.SendKeys("0");

            // Button
            var button = _driver.FindElement(By.XPath("//button[text()='Search Trips']"));
            button.Click();
            Thread.Sleep(2000);

            var trips = _driver.FindElements(By.ClassName("MuiGrid-container"));

            trips.Should().BeEmpty();
        }

        [Test]
        public void ShouldHaveNoTripsWhenThereIsDateWithoutOffers()
        {
            Thread.Sleep(2000);

            var searchText = _driver.FindElement(By.XPath("//h6[text()='Choose offer details']"));

            searchText.Should().NotBeNull();

            // Date
            var date = _driver.FindElement(By.XPath("//input[@placeholder='mm/dd/yyyy']"));
            date.SendKeys(Keys.Control + "a");
            date.SendKeys(Keys.Delete);
            date.SendKeys("05/01/2022");

            // Button
            var button = _driver.FindElement(By.XPath("//button[text()='Search Trips']"));
            button.Click();
            Thread.Sleep(2000);

            var trips = _driver.FindElements(By.ClassName("MuiGrid-container"));

            trips.Should().BeEmpty();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}
