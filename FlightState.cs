using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace CheapestFlightsRewrite
{
    internal class FlightState
    {
        private List<string> endLocs = new List<string> { "ALC", "STN", "AGP" };
        private List<string> startLocs = new List<string> { "EDI", "GLA" };
        private string adults = "1";
        private string date = "2023-11";
        private List<FlightData> flightDataList;
        private FlightScraper flightScraper;
        private IWebDriver driver;

        public FlightState()
        {
            // Automatically download and setup the latest ChromeDriver
            new DriverManager().SetUpDriver(new ChromeConfig());


            flightDataList = new List<FlightData>();
            flightScraper = new FlightScraper();

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--log-level=3");

            // Now, no need to specify the path, WebDriverManager has handled it
            driver = new ChromeDriver(options);

            Initialize();
            DisplayFlightData();
        }

        public void Initialize()
        {
            foreach (var startLoc in startLocs)
            {


                foreach (var endLoc in endLocs)
                {

                    if (date.Length == 7) // Indicates format is "YYYY-MM"
                    {
                        int year = int.Parse(date.Split('-')[0]);
                        int month = int.Parse(date.Split('-')[1]);
                        int daysInMonth = DateTime.DaysInMonth(year, month);

                        for (int day = 1; day <= daysInMonth; day++)
                        {
                            string fullDate = $"{year}-{month.ToString("00")}-{day.ToString("00")}";
                            string url = ConstructUrl(fullDate, startLoc, endLoc);
                            Console.WriteLine(url);
                            Console.WriteLine();  // Print a blank line

                            try
                            {
                                var dailyFlightDataList = flightScraper.ScrapeFlights(driver, url, startLoc, endLoc, fullDate);

                                flightDataList.AddRange(dailyFlightDataList);
                            }
                            catch (WebDriverTimeoutException)
                            {
                                Console.WriteLine($"WEB DRIVER TIMEOUT for {fullDate}");
                                continue;  // Skip this day and move to the next
                            }
                        }
                    }
                    else
                    {
                        string url = ConstructUrl(date, startLoc, endLoc);
                        Console.WriteLine(url);
                        Console.WriteLine();  // Print a blank line

                        flightDataList = flightScraper.ScrapeFlights(driver, url, startLoc, endLoc, date);
                    }


                }
            }
            driver.Close();
            driver.Quit();
        }

        private void DisplayFlightData()
        {
            if (flightDataList.Count > 0)
            {
                Console.WriteLine("\nAll flights:");

                // Group flight data by departure and then by destination
                var groupedByDeparture = flightDataList.GroupBy(flight => flight.Departure);

                foreach (var departureGroup in groupedByDeparture)
                {
                    Console.WriteLine($"\nFlights from {departureGroup.Key}:"); // departureGroup.Key is the startLoc

                    var groupedByDestination = departureGroup.GroupBy(flight => flight.Destination);
                    foreach (var destinationGroup in groupedByDestination)
                    {
                        Console.WriteLine($"\nTo {destinationGroup.Key}:"); // destinationGroup.Key is the endLoc

                        foreach (FlightData flightData in destinationGroup)
                        {
                            Console.WriteLine($"Departing at {flightData.Time} on {flightData.Date} is priced at {flightData.Price}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"No data found for date {date}");
            }
        }

        private string ConstructUrl(string dateToUse, string startLoc, string endLoc)
        {
            return $"https://www.ryanair.com/gb/en/trip/flights/select?adults={adults}&teens=0&children=0&infants=0&dateOut={dateToUse}&dateIn=&isConnectedFlight=false&isReturn=false&discount=0&promoCode=&originIata={startLoc}&destinationIata={endLoc}&tpAdults={adults}&tpTeens=0&tpChildren=0&tpInfants=0&tpStartDate={dateToUse}&tpEndDate=&tpDiscount=0&tpPromoCode=&tpOriginIata={startLoc}&tpDestinationIata={endLoc}";
        }

    }
}