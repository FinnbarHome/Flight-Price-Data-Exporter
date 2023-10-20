using OpenQA.Selenium;
using System;

namespace CheapestFlights
{
    internal class UrlNavigator
    {
        private const string BaseUrl = "https://www.ryanair.com/gb/en/trip/flights/select?adults={0}&teens=0&children=0&infants=0&dateOut={1}&dateIn=&isConnectedFlight=false&isReturn=false&discount=0&promoCode=&originIata={2}&destinationIata={3}&tpAdults={0}&tpTeens=0&tpChildren=0&tpInfants=0&tpStartDate={1}&tpEndDate=&tpDiscount=0&tpPromoCode=&tpOriginIata={2}&tpDestinationIata={3}";

        public void NavigateToUrl(IWebDriver driver, string date, string departureCity, string destination, string adultsAmount, bool isReturn = false)
        {
            string url = isReturn ? string.Format(BaseUrl, adultsAmount, date, destination, departureCity)
                                  : string.Format(BaseUrl, adultsAmount, date, departureCity, destination);

            Console.WriteLine($"Navigating to the URL for date {date}");
            Console.WriteLine(url);
            driver.Navigate().GoToUrl(url);

            System.Threading.Thread.Sleep(200);  // wait for the page to load

            int retryCount = 0;
            while (retryCount < 5)
            {
                if (CheckElementExists(driver, "/html/body/app-root/flights-root/div/div/div/div/flights-lazy-content/flights-summary-container/flights-summary/div/div[1]/journey-container/journey/flight-list/ry-spinner/div/flight-card-new/div/div/flight-info-new/div[1]/span[1]"))
                {
                    break;  // exit the loop once the desired element is found
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                    retryCount++;
                }
            }
        }

        private bool CheckElementExists(IWebDriver driver, string xpath)
        {
            // Check if the element exists
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
    }
}
