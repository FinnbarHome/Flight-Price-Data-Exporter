using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CheapestFlightsRewrite
{
    internal class FlightScraper
    {
        private const string NormalPriceXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span/flights-price-simple";
        private const string SalePriceXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span[2]/flights-price-simple";
        private const string TimeXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/flight-info-new/div[1]/span[1]";

        public List<FlightData> ScrapeFlights(IWebDriver driver, string url, string startLoc, string endLoc, string date)
        {
            driver.Navigate().GoToUrl(url);
            var htmlDocument = FetchPageSource(driver);

            return ParseFlightData(htmlDocument, startLoc, endLoc, date);
        }

        private IWebDriver InitializeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--log-level=3");
            return new ChromeDriver(options);
        }

        private HtmlDocument FetchPageSource(IWebDriver driver)
        {
            int retries = 1;  // Number of retries

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    WaitForElement(driver, TimeXPath, 3); // 3 represents how long it waits
                    var pageSource = driver.PageSource;
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(pageSource);
                    return htmlDocument;
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("WEB DRIVER TIMEOUT. Retrying...");
                    if (i < retries - 1)
                    {
                        // If not the last retry, wait for a while before trying again
                        Thread.Sleep(2000);  // Wait for 2 seconds
                    }
                }
            }

            // If all retries failed, return an empty document
            return new HtmlDocument();
        }

        private void WaitForElement(IWebDriver driver, string xPath, int seconds)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            try
            {
                wait.Until(drv => drv.FindElements(By.XPath(xPath)).Count > 0);
            }
            catch (WebDriverTimeoutException)
            {
                throw;  // Re-throwing the exception to handle it in FetchPageSource
            }
        }

        private List<FlightData> ParseFlightData(HtmlDocument htmlDocument, string startLoc, string endLoc, string date)
        {
            var flightDataList = new List<FlightData>();
            HtmlNodeCollection timeNodes = htmlDocument.DocumentNode.SelectNodes(TimeXPath);

            if (timeNodes != null && timeNodes.Count > 0)
            {
                for (int i = 0; i < timeNodes.Count; i++)
                {
                    string time = timeNodes[i].InnerText.Trim();

                    HtmlNode priceNode = htmlDocument.DocumentNode.SelectSingleNode($"{SalePriceXPath}[{i + 1}]");
                    if (priceNode == null)
                    {
                        priceNode = htmlDocument.DocumentNode.SelectSingleNode($"{NormalPriceXPath}[{i + 1}]");
                    }

                    if (priceNode != null)
                    {
                        string price = priceNode.InnerHtml.Trim();

                        // Instantiate FlightData and store the data
                        FlightData flightData = new FlightData
                        {
                            Date = date,
                            Departure = startLoc,
                            Destination = endLoc,
                            Time = time,
                            Price = price
                        };

                        flightDataList.Add(flightData);  // Add to the list

                        // Print the flight data as it's found
                        Console.WriteLine($"A flight from {flightData.Departure} to {flightData.Destination} departing at {flightData.Time} on {flightData.Date} is priced at {flightData.Price}");
                    }
                }
            }

            return flightDataList;
        }
    }
}