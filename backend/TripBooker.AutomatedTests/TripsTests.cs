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
            _driver.Url = $"{TestConsts.Url}/trips";
        }

        [Test]
        public void ShouldHaveNoTrips()
        {
            Thread.Sleep(2000);

            var searchText = _driver.FindElement(By.XPath("//h6[text()='Choose offer details']"));

            searchText.Should().NotBeNull();

            var trips = _driver.FindElements(By.ClassName("MuiGrid-container"));

            trips.Should().BeEmpty();
        }

        [Test]
        public void ShouldDisplayList()
        {
            Thread.Sleep(2000);

            // Dest
            var dest = _driver.FindElement(By.Id("destination")).FindElement(By.XPath("./../input"));
            dest.SendKeys("ZTH");

            // Departure
            var dep = _driver.FindElement(By.Id("departure")).FindElement(By.XPath("./../input"));
            dep.SendKeys("WAW");

            // Number of days
            var days = _driver.FindElement(By.XPath("//span[contains(text(),'Number of days')]/../../../input"));
            days.SendKeys("7");

            // Number of adults
            var adults = _driver.FindElement(By.XPath("//span[contains(text(),'Number of adults')]/../../../input"));
            adults.SendKeys("1");

            // Date
            var date = _driver.FindElement(By.XPath("//input[@placeholder='mm/dd/yyyy']"));
            date.SendKeys("07/02/2022");

            // Buton
            var button = _driver.FindElement(By.XPath("//button[text()='Search Trips']"));
            button.Click();

            Thread.Sleep(2000);

            var trips = _driver.FindElement(By.ClassName("MuiGrid-container"));
            trips.Should().NotBeNull();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}
