using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;

internal class Program
{
    static void Main()
    {
        // Path to your ChromeDriver executable
        string driverPath = @"C:\path\to\chromedriver";

        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");  // Optional: Launches the browser in headless mode (without GUI)
        options.AddArgument("--log-level=3");  // Suppress INFO and WARNING console messages

        //List of destinations to travel to
        List<string> destinations = new List<string>();

        Console.Write("Enter the destinations (separated by a comma): ");
        string destinationsInput = Console.ReadLine();

        if (!string.IsNullOrEmpty(destinationsInput))
        {
            destinations.AddRange(destinationsInput.Split(',', StringSplitOptions.RemoveEmptyEntries));
        }
        else
        {
            Console.WriteLine("You must enter at least one destination.");
            return;  //Stop execution if no destinations were entered
        }

        //Date which the user is departing
        string date = "";
        while (true)
        {
            Console.Write("Enter the date in the form YEAR-MONTH-DAY (eg:2023-05-22) or YEAR-MONTH (eg:2023-02 for February): ");
            string dateInput = Console.ReadLine();

            if (!string.IsNullOrEmpty(dateInput))
            {
                DateTime temp;
                if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out temp))
                {
                    date = dateInput;
                    break;
                }
                else if (DateTime.TryParseExact(dateInput, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out temp))
                {
                    date = dateInput;
                    break;
                }
                else
                {
                    Console.WriteLine("Enter a valid date or month format (YEAR-MONTH-DAY or YEAR-MONTH)");
                }
            }
            else
            {
                Console.WriteLine("Enter a valid date or month");
            }
        }



        //City which the user is departing from
        string departureCity = "EDI";

        //Amount of adults flying
        string adultsAmount = "1";


        using (IWebDriver driver = new ChromeDriver(driverPath, options))
        {
            
            foreach (string destination in destinations)
            {
                string url = $"https://www.ryanair.com/gb/en/trip/flights/select?adults={adultsAmount}&teens=0&children=0&infants=0&dateOut={date}&dateIn=&isConnectedFlight=false&isReturn=false&discount=0&promoCode=&originIata={departureCity}&destinationIata={destination}&tpAdults={adultsAmount}&tpTeens=0&tpChildren=0&tpInfants=0&tpStartDate={destination}&tpEndDate=&tpDiscount=0&tpPromoCode=&tpOriginIata={departureCity}&tpDestinationIata={destination}";
                driver.Navigate().GoToUrl(url);

                System.Threading.Thread.Sleep(5000);  // wait for the page to load

                var pageSource = driver.PageSource;

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pageSource);

                var priceNodes = htmlDocument.DocumentNode.SelectNodes("/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span/flights-price-simple");  // change to SelectNodes
                var timeNodes = htmlDocument.DocumentNode.SelectNodes("/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/div/flight-card-new/div/div/flight-info-new/div[1]/span[1]");  // replace with the XPath to the time element

                if (priceNodes != null && timeNodes != null && priceNodes.Count == timeNodes.Count)
                {
                    for (int i = 0; i < priceNodes.Count; i++)
                    {
                        Console.WriteLine($"A flight from {departureCity} to {destination} departing at {timeNodes[i].InnerHtml.Trim()} is priced at {priceNodes[i].InnerHtml.Trim()}");
                    }
                }
            }
        }
    }


}
