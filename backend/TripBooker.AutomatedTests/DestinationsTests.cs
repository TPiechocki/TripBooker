using System;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium_Demo;
using System.Threading;

namespace TripBooker.AutomatedTests
{
    class DestinationsTests
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
            _driver.Url = TestConsts.Url;
        }

        [Test]
        public void ShouldContainDestinationsTitle()
        {
            Thread.Sleep(2000);

            var searchText = _driver.FindElement(By.XPath("//h2[text()='Destinations']"));

            searchText.Should().NotBeNull();
        }

        [Test]
        public void ShouldShowDestinationsWithCheckOffersButtons()
        {
            Thread.Sleep(2000);

            var destinations = _driver.FindElements(By.CssSelector(".offerCard"));

            destinations.Should().NotBeEmpty();

            foreach (var card in destinations)
            {
                var buttons = card.FindElements(By.CssSelector("button"));
                buttons.Should().ContainSingle();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}