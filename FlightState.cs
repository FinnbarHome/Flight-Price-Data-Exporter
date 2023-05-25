using CheapestFlights;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OfficeOpenXml;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System;

public class FlightState
{
    private UserInteraction userInteraction;
    private Scraper flightDataFetcher;
    private List<string> Destinations;
    private string Date;
    private bool IsReturn = false;

    public FlightState()
    {
        userInteraction = new UserInteraction();
        flightDataFetcher = new Scraper();
        Destinations = new List<string>();
        Date = "";
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Initialize();
    }

    public void Initialize()
    {
        //Destinations = userInteraction.GetDestinationsFromUser();

        //EDI locations
        Destinations.AddRange("ALC,BCN,VIE,CRL,SOF,ZAD,PRG,BLL,CPH,BZR,BOD,CCF,GNB,MRS,NTE,FNI,BVA,PIS,TLS,BER,NRN,HAM,CFU,RHO,BUD,ORK,DUB,NOC,SNN,BRI,BLQ,BGY,NAP,PMO,PSA,CIA,TRN,VCE,VRN,RIX,KUN,MLA,RAK,EIN,GDN,FAO,LIS,OPO,OTP,BTS,FUE,LPA,IBZ,ACE,MAD,AGP,PMI,SDR,SCQ,SVQ,TFS,VLC,GOT,BFS,BOH,STN,NQY".Split(','));
        
        //ALC alicante, BCN barcelona, VIE vienna, CRL brussels, SOF sofia, ZAD zadar, PRG prague, BLL Billund, CPH Copenhagen, BZR Beziers, BOD Bordeaux, CCF Carcassonne, GNB Grenoble, MRS Marseille, NTE Nantes
        //FNI Nimes, BVA Paris Beauvais, PIS Poitiers, TLS Toulose, BER Berlin Brandenburg, NRN Dusseldorf Weeze, HAM Hamburg, CFU Corfu, RHO Rhodes, BUD Budapest, ORK Cork, DUB Dublin, NOC Knock, SNN Shannon
        //BRI Bari, BLQ Bologna, BGY Milan Bergamo, NAP Naples, PMO Palermo, PSA Pisa, CIA Rome Ciampino, TRN Turin, VCE Venice M.Polo, VRN Verona, RIX Riga, KUN Kaunas, MLA Malta, RAK Marrakesh, EIN Eindhoven
        //GDN Gdansk, FAO Faro, LIS Lisbon, OPO Porto, OTP Bucharest, BTS Bratislava, FUE Fuerteventura, LPA Gran Canaria, IBZ Ibiza, ACE Lanzarote, MAD Madrid, AGP Malaga, PMI Palma de Mallorca, SDR Santander
        //SCQ Santiago, SVQ Seville, TFS Tenerife South, VLC Valencia, GOT Goteborg Landvetter, BFS Belfast International, BOH Bournemouth, STN London Stansted, NQY Newquay Cornwall

        //Destinations.Add("BCN");

        //Date = userInteraction.GetDateFromUser();
        string Date = "2023-06";

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
