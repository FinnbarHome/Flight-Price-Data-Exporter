using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CheapestFlightsRewrite
{
    internal static class FlightXPathConstants
    {
        public const string NormalPriceXPath = "//flight-card-summary/div[2]/div/div/span/flights-price-simple";
        public const string SalePriceXPath = "//flight-card-summary/div[2]/div/div/span[2]/flights-price-simple";
        public const string TimeXPath = "//flight-info-new/div[1]/span[1]";
    }

    internal class FlightScraper
    {
        private readonly IWebDriver _driver;

        public FlightScraper(IWebDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public List<FlightData> ScrapeFlights(string url, string startLoc, string endLoc, string date)
        {
            _driver.Navigate().GoToUrl(url);
            var htmlDocument = FetchPageSource();

            return ParseFlightData(htmlDocument, startLoc, endLoc, date);
        }

        private HtmlDocument FetchPageSource()
        {
            const int retries = 1;

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    WaitForElement(FlightXPathConstants.TimeXPath, 3);
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(_driver.PageSource);
                    return htmlDocument;
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("WEB DRIVER TIMEOUT. Retrying...");
                    Thread.Sleep(2000);
                }
            }

            return new HtmlDocument();
        }

        private void WaitForElement(string xPath, int seconds)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
            wait.Until(drv => drv.FindElements(By.XPath(xPath)).Any());
        }

        private List<FlightData> ParseFlightData(HtmlDocument htmlDocument, string startLoc, string endLoc, string date)
        {
            var flightDataList = new List<FlightData>();

            // Selecting all flight cards on the page
            var flightCards = htmlDocument.DocumentNode.SelectNodes("//flight-card-new");

            if (flightCards == null || flightCards.Count == 0)
            {
                Console.WriteLine("No flights found on the page.");
                return flightDataList;
            }

            foreach (var flightCard in flightCards)
            {
                string time = flightCard.SelectSingleNode(".//flight-info-new/div[1]/span[1]")?.InnerText.Trim() ?? "N/A";

                // Check for sale price first within this flight card
                var priceNode = flightCard.SelectSingleNode(".//flight-card-summary/div[2]/div/div/span[2]/flights-price-simple")
                             ?? flightCard.SelectSingleNode(".//flight-card-summary/div[2]/div/div/span/flights-price-simple");

                string price = priceNode?.InnerText.Trim() ?? "N/A";

                var flightData = new FlightData(date, startLoc, endLoc, time, price);
                flightDataList.Add(flightData);
                Console.WriteLine(flightData);
            }

            return flightDataList;
        }


    }
}
