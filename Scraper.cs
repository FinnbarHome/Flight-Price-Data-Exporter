using HtmlAgilityPack;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CheapestFlights
{
    internal class Scraper
    {
        // XPaths for the data we want to scrape
        private const string NormalPriceXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span/flights-price-simple";
        private const string SalePriceXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span[2]/flights-price-simple";
        private const string TimeXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/flight-info-new/div[1]/span[1]";


        private UrlNavigator urlNavigator;
        private DataStorage dataStorage;

        public Scraper()
        {
            urlNavigator = new UrlNavigator();
            dataStorage = new DataStorage();
        }

        public void FetchFlightData(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            if (IsMonth(date))
            {
                FetchFlightDataForWholeMonth(driver, date, departureCity, destinations, adultsAmount);
            }
            else
            {
                FetchFlightDataSingleDate(driver, date, departureCity, destinations, adultsAmount);
            }
        }

        public void FetchReturnFlightData(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            if (IsMonth(date))
            {
                FetchFlightDataForWholeMonth(driver, date, departureCity, destinations, adultsAmount, true);
            }
            else
            {
                FetchFlightDataSingleDate(driver, date, departureCity, destinations, adultsAmount, true);
            }
        }

        private void FetchFlightDataForWholeMonth(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount, bool isReturn = false)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                Console.WriteLine($"Invalid date format: {date}");
                return;
            }

            int daysInMonth = DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                string dateForDay = $"{parsedDate.Year}-{parsedDate.Month:D2}-{day:D2}";
                FetchFlightDataSingleDate(driver, dateForDay, departureCity, destinations, adultsAmount, isReturn);
            }

            driver.Quit();
            Environment.Exit(0);
        }

        private void FetchReturnFlightDataForWholeMonth(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(5, 2));
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                string dateForDay = $"{year}-{month.ToString("D2")}-{day.ToString("D2")}";
                FetchFlightDataSingleDate(driver, dateForDay, departureCity, destinations, adultsAmount, true);
            }

            driver.Quit(); // Close the browser session
            Environment.Exit(0); // Exit the program
        }


        private void FetchFlightDataSingleDate(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount, bool isReturn = false)
        {
            // Resetting the flight data list before fetching new data
            dataStorage.ClearFlightDataList();

            foreach (string destination in destinations)
            {
                urlNavigator.NavigateToUrl(driver, date, departureCity, destination, adultsAmount, isReturn);

                var pageSource = driver.PageSource;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pageSource);

                HtmlNodeCollection priceNodes = htmlDocument.DocumentNode.SelectNodes(CheckElementExists(driver, SalePriceXPath) ? SalePriceXPath : NormalPriceXPath);
                HtmlNodeCollection timeNodes = htmlDocument.DocumentNode.SelectNodes(TimeXPath);

                if (priceNodes != null && timeNodes != null)
                {
                    if (timeNodes.Count == priceNodes.Count)
                    {
                        for (int i = 0; i < priceNodes.Count; i++)
                        {
                            string time = timeNodes[i].InnerText.Trim();
                            string price = CorrectCurrencySymbol(priceNodes[i].InnerText.Trim());

                            dataStorage.StoreFlightData(date, departureCity, destination, time, price, isReturn);
                            Console.WriteLine($"A flight from {departureCity} to {destination} departing at {time} is priced at {price}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Mismatch in number of time nodes ({timeNodes.Count}) and price nodes ({priceNodes.Count}) for {date} between {departureCity} and {destination}");
                    }
                }
                else
                {
                    Console.WriteLine($"No data found for {date} between {departureCity} and {destination}");
                }
            }
        }

        private bool CheckElementExists(IWebDriver driver, string xpath)
        {
            try
            {
                driver.FindElement(By.XPath(xpath));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private string CorrectCurrencySymbol(string price)
        {
            if (price[0] != '£')
            {
                price = '£' + price.Substring(1);
            }
            return price;
        }

        private bool IsMonth(string date)
        {
            return date.Length == 7;
        }
    }
}
