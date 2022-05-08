using System;
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
            // chromeOptions.AddArguments("headless");

            _driver = new ChromeDriver(chromeOptions);
            _driver.Manage().Window.Maximize();
            _driver.Url = $"{TestConsts.Url}/trips/";
        }

        [Test]
        public void ShouldDisplayTripsWithDefaultValues()
        {
            Thread.Sleep(2000);

            // Button
            var button = _driver.FindElement(By.XPath("//button[text()='Search Trips']"));
            button.Click();

            Thread.Sleep(2000);


            var trips = _driver.FindElement(By.ClassName("MuiGrid-container"));
            trips.Should().NotBeNull();
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

            var trips = _driver.FindElements(By.ClassName("MuiGrid-container"));

            trips.Should().ContainSingle();
            trips.Single().FindElements(By.XPath(".//*")).Should().BeEmpty();
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

            trips.Should().ContainSingle();
            trips.Single().FindElements(By.XPath(".//*")).Should().BeEmpty();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}
