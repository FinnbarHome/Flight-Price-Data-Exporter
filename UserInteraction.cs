using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;

namespace CheapestFlights
{
    internal class UserInteraction
    {
        public string Date;
        public List<string> Destinations;

        public UserInteraction() {
            Date = "";
            Destinations = new List<string>();
        }

        public List<string> GetDestinationsFromUser()
        {
            Console.Write("Enter the destinations (separated by a comma): ");
            string destinationsInput = Console.ReadLine() ?? "";

            if (!string.IsNullOrEmpty(destinationsInput))
            {
                Destinations.AddRange(destinationsInput.Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                Console.WriteLine("You must enter at least one destination.");
                Environment.Exit(0); // Stop execution if no destinations were entered
            }

            return Destinations;
        }

        public string GetDateFromUser()
        {
            while (true)
            {
                Console.Write("Enter the date in the form YEAR-MONTH-DAY (eg:2023-05-22) or YEAR-MONTH (eg:2023-02 for February): ");
                string dateInput = Console.ReadLine() ?? "";

                if (!string.IsNullOrEmpty(dateInput))
                {
                    DateTime temp;
                    if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out temp) ||
                        DateTime.TryParseExact(dateInput, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out temp))
                    {
                        Date = dateInput;
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
            return Date;
        }
    }
}
