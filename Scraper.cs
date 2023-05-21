using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestFlights
{
    internal class Scraper
    {

        public void FetchFlightData(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            if (date.Length == 7)
            {
                FetchFlightDataForWholeMonth(driver, date, departureCity, destinations, adultsAmount);
            }
            else
            {
                FetchFlightDataSingleDate(driver, date, departureCity, destinations, adultsAmount);
            }
        }

        public void FetchFlightDataForWholeMonth(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(5, 2));
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                string dateForDay = $"{year}-{month.ToString("D2")}-{day.ToString("D2")}";
                FetchFlightDataSingleDate(driver, dateForDay, departureCity, destinations, adultsAmount);
            }
        }

        public void FetchFlightDataSingleDate(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            foreach (string destination in destinations)
            {
                string url = BuildUrl(date, departureCity, destination, adultsAmount);
                Console.WriteLine("");
                Console.WriteLine($"Checking on the date {date} using the current url:");
                Console.WriteLine(url);
                Console.WriteLine("");

                driver.Navigate().GoToUrl(url);

                System.Threading.Thread.Sleep(5000);  // wait for the page to load
                var pageSource = driver.PageSource;

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pageSource);

                //Normal Price
                var priceXNode = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span/flights-price-simple";
                //Sale Price
                var priceXNode2 = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span[2]/flights-price-simple";
                var timeXNode = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/div/flight-card-new/div/div/flight-info-new/div[1]/span[1]";

                var priceNodes = htmlDocument.DocumentNode.SelectNodes(priceXNode);  
                var timeNodes = htmlDocument.DocumentNode.SelectNodes(timeXNode);  

                for(int i = 0; i < priceNodes.Count; i++)
                {
                    Console.WriteLine($"priced at {priceNodes[i].InnerHtml.Trim()}");
                }   
                //If there isn't a normal price available, try the sale price
                if(priceNodes == null)
                {
                    Console.WriteLine("No normal price found, trying sale price");
                    priceNodes = htmlDocument.DocumentNode.SelectNodes(priceXNode2);
                }


                    if (priceNodes != null && timeNodes != null && timeNodes.Count == priceNodes.Count)
                {
                    for (int i = 0; i < priceNodes.Count; i++)
                    {
                        Console.WriteLine($"A flight from {departureCity} to {destination} departing at {timeNodes[i].InnerHtml.Trim()} is priced at {priceNodes[i].InnerHtml.Trim()}");
                    }
                }
            }
        }


        private string BuildUrl(string date, string departureCity, string destination, string adultsAmount)
        {
            return $"https://www.ryanair.com/gb/en/trip/flights/select?adults={adultsAmount}&teens=0&children=0&infants=0&dateOut={date}&dateIn=&isConnectedFlight=false&isReturn=false&discount=0&promoCode=&originIata={departureCity}&destinationIata={destination}&tpAdults={adultsAmount}&tpTeens=0&tpChildren=0&tpInfants=0&tpStartDate={destination}&tpEndDate=&tpDiscount=0&tpPromoCode=&tpOriginIata={departureCity}&tpDestinationIata={destination}";
        }
    }
}

