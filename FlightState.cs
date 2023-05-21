using CheapestFlights;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

public class FlightState
{
    private UserInteraction userInteraction;
    private Scraper flightDataFetcher;
    private List<string> Destinations;
    private string Date;

    public FlightState()
    {
        userInteraction = new UserInteraction();
        flightDataFetcher = new Scraper();
        Destinations = new List<string>();
        Date = "";
        Initialize();
    }

    public void Initialize()
    {
        //Destinations = userInteraction.GetDestinationsFromUser();
        Destinations.Add("VIE");
        //Date = userInteraction.GetDateFromUser();
        string Date = "2023-08-04";

        //Path to your ChromeDriver executable
        string driverPath = @"C:\path\to\chromedriver";

        ChromeOptions options = new ChromeOptions();
        //Launches the browser in headless mode (without GUI)
        options.AddArgument("--headless");
        //Suppress INFO and WARNING console messages
        options.AddArgument("--log-level=3");  

        //City which the user is departing from
        string DepartureCity = "EDI";

        using (IWebDriver driver = new ChromeDriver(driverPath, options))
        {
            flightDataFetcher.FetchFlightData(driver, Date, DepartureCity, Destinations, "1");
        }
    }
}
