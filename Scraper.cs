using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CheapestFlights
{
    internal class Scraper
    {
        //Data to write to the Excel file 
        private List<FlightData> flightDataList = new List<FlightData>();
        //Path to the Excel file
        private string excelFilePath = "flightData.xlsx";
        //Base URL for the Ryanair website
        private const string BaseUrl = "https://www.ryanair.com/gb/en/trip/flights/select?adults={0}&teens=0&children=0&infants=0&dateOut={1}&dateIn=&isConnectedFlight=false&isReturn=false&discount=0&promoCode=&originIata={2}&destinationIata={3}&tpAdults={0}&tpTeens=0&tpChildren=0&tpInfants=0&tpStartDate={3}&tpEndDate=&tpDiscount=0&tpPromoCode=&tpOriginIata={2}&tpDestinationIata={3}";

        //XPaths for the data we want to scrape
        private const string NormalPriceXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span/flights-price-simple";
        private const string SalePriceXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/div[4]/flight-card-summary/div[2]/div/div/span[2]/flights-price-simple";
        private const string TimeXPath = "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/flight-info-new/div[1]/span[1]";

        //List for error mismatch 
        private List<string> errorMismatchDates = new List<string>();
        private List<string> mismatchData = new List<string>();


        public void FetchFlightData(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            flightDataList.Clear();
            if (date.Length == 7)
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
            flightDataList.Clear();
            if (date.Length == 7)
            {
                FetchReturnFlightDataForWholeMonth(driver, date, departureCity, destinations, adultsAmount);
            }
            else
            {
                FetchFlightDataSingleDate(driver, date, departureCity, destinations, adultsAmount, true);
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

            Console.WriteLine("The following dates had a mismatch in the number of time nodes and price nodes:");
            WriteDataToFile(mismatchData, "mismatchData.txt");
        }

        public void FetchReturnFlightDataForWholeMonth(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount)
        {
            int year = int.Parse(date.Substring(0, 4));
            int month = int.Parse(date.Substring(5, 2));
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                string dateForDay = $"{year}-{month.ToString("D2")}-{day.ToString("D2")}";
                FetchFlightDataSingleDate(driver, dateForDay, departureCity, destinations, adultsAmount, true);
            }

            Console.WriteLine("The following dates had a mismatch in the number of time nodes and price nodes for return flights:");
            WriteDataToFile(mismatchData, "mismatchData.txt");
        }

        public void FetchFlightDataSingleDate(IWebDriver driver, string date, string departureCity, List<string> destinations, string adultsAmount, bool isReturn = false)
        {
            string worksheetName = isReturn ? "ReturnFlights" : "DepartingFlights";

            foreach (string destination in destinations)
            {
                string url = isReturn ? string.Format(BaseUrl, adultsAmount, date, destination, departureCity)
                                      : string.Format(BaseUrl, adultsAmount, date, departureCity, destination);

                Console.WriteLine($"Checking on the date {date}");
                Console.WriteLine(url);

                driver.Navigate().GoToUrl(url);

                System.Threading.Thread.Sleep(100);  // wait for the page to load

                int count = 0;
                while (count < 50)
                {
                    if (CheckElementExists(driver, TimeXPath))
                    {
                        count = 50;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(100);
                        count++;
                    }
                }

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
                            string time = timeNodes[i].InnerHtml.Trim();
                            string price = CorrectCurrencySymbol(priceNodes[i].InnerHtml.Trim());
                            Console.WriteLine($"A flight from {departureCity} to {destination} departing at {time} is priced at {price}");

                            var flightData = new FlightData
                            {
                                Date = date,
                                Departure = departureCity,
                                Destination = destination,
                                Time = time,
                                Price = price
                            };

                            flightDataList.Add(flightData);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Mismatch in number of time nodes ({timeNodes.Count}) and price nodes ({priceNodes.Count})");
                        errorMismatchDates.Add(date);

                        mismatchData.Add($"Mismatch on date {date} at url: {url}. Time nodes: {timeNodes.Count}, Price nodes: {priceNodes.Count}");
                    }
                }
                else
                {
                    if (priceNodes == null)
                    {
                        Console.WriteLine("No price nodes found");
                    }
                    if (timeNodes == null)
                    {
                        Console.WriteLine("No time nodes found");
                    }
                }
            }

            if (!isReturn)
            {
                WriteDataToExcelFile(flightDataList, excelFilePath, false);
            }
            else
            {
                WriteDataToExcelFile(flightDataList, excelFilePath, true);
            }
            
        }


        private bool CheckElementExists(IWebDriver driver, string xpath)
        {
            //Check if the element exists
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

        public void WriteDataToExcelFile(List<FlightData> flightDataList, string excelFilePath, bool isReturnFlight)
        {
            var file = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(file))
            {
                // Get the first sheet for normal flights, second sheet for return flights
                var worksheet = isReturnFlight ? package.Workbook.Worksheets[1] : package.Workbook.Worksheets[0];

                int lastRow = worksheet.Dimension == null ? 1 : worksheet.Dimension.End.Row;

                for (int i = 0; i < flightDataList.Count; i++)
                {
                    // Starting the data from the last empty row
                    var row = lastRow + i + 1;

                    // Update the necessary columns in the worksheet
                    worksheet.Cells[row, 1].Value = flightDataList[i].Date;         // Column A
                    worksheet.Cells[row, 2].Value = flightDataList[i].Departure;    // Column B
                    worksheet.Cells[row, 3].Value = flightDataList[i].Destination;  // Column C
                    worksheet.Cells[row, 4].Value = flightDataList[i].Time;         // Column D
                    worksheet.Cells[row, 5].Value = flightDataList[i].Price;        // Column E
                }

                package.Save();
            }
        }

        private void WriteDataToFile(List<string> data, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            //Append all records to the string builder
            foreach (var record in data)
            {
                fileContent.AppendLine(record);
            }

            //Explicitly use UTF-8 encoding
            File.WriteAllText(filePath, fileContent.ToString(), Encoding.UTF8);
            Console.WriteLine($"Written data to {filePath}");
        }

        private string CorrectCurrencySymbol(string price)
        {
            // If the first character is not a pound sign, replace it with one.
            if (price[0] != '£')
            {
                price = '£' + price.Substring(1);
            }

            return price;
        }


    }
}
