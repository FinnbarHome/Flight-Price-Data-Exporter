using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CheapestFlightsRewrite
{
    internal class FlightState : IDisposable
    {
        private readonly List<string> _endLocs = new() { "ALC" };
        private readonly List<string> _startLocs = new() { "EDI" };
        private readonly string _adults = "1";
        private readonly string _date = "2025-05";
        private readonly List<FlightData> _flightDataList = new();
        private readonly IWebDriver _driver;
        private readonly FlightScraper _flightScraper;

        public FlightState()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--log-level=3");

            _driver = new ChromeDriver(options);
            _flightScraper = new FlightScraper(_driver);

            Initialize();
            DisplayFlightData();
        }

        private void Initialize()
        {
            foreach (var startLoc in _startLocs)
            {
                foreach (var endLoc in _endLocs)
                {
                    var datesToProcess = GetDatesToProcess();

                    foreach (var fullDate in datesToProcess)
                    {   
                        var url = ConstructUrl(fullDate, startLoc, endLoc);

                        Console.WriteLine("Trying with url:");
                        Console.WriteLine(url);

                        try
                        {
                            _flightDataList.AddRange(_flightScraper.ScrapeFlights(url, startLoc, endLoc, fullDate));
                        }
                        catch (WebDriverTimeoutException)
                        {
                            Console.WriteLine($"WEB DRIVER TIMEOUT for {fullDate}");
                        }
                        Console.WriteLine("");
                    }
                }
            }
        }

        private IEnumerable<string> GetDatesToProcess()
        {
            if (_date.Length != 7) return new List<string> { _date };

            int year = int.Parse(_date.Split('-')[0]);
            int month = int.Parse(_date.Split('-')[1]);
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                             .Select(day => $"{year}-{month:D2}-{day:D2}");
        }

        private void DisplayFlightData()
        {
            if (_flightDataList.Any())
            {
                Console.WriteLine("\nAll flights:");
                foreach (var flight in _flightDataList)
                {
                    Console.WriteLine(flight);
                }
            }
            else
            {
                Console.WriteLine($"No data found for date {_date}");
            }
        }

        private string ConstructUrl(string dateToUse, string startLoc, string endLoc) =>
            $"https://www.ryanair.com/gb/en/trip/flights/select?adults={_adults}&dateOut={dateToUse}&originIata={startLoc}&destinationIata={endLoc}";

        public void Dispose()
        {
            _driver.Quit();
        }
    }
}
