using CheapestFlights;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OfficeOpenXml;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class FlightState
{
    private UserInteraction userInteraction;
    private Scraper flightDataFetcher;
    private string Date;
    private bool IsReturn = false;

    public FlightState()
    {
        userInteraction = new UserInteraction();
        flightDataFetcher = new Scraper();
        Date = "";
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        Initialize();
    }

    public void Initialize()
    {
        string folderPath = "Airports";

        //ALC alicante, BCN barcelona, VIE vienna, CRL brussels, SOF sofia, ZAD zadar, PRG prague, BLL Billund, CPH Copenhagen, BZR Beziers, BOD Bordeaux, CCF Carcassonne, GNB Grenoble, MRS Marseille, NTE Nantes
        //FNI Nimes, BVA Paris Beauvais, PIS Poitiers, TLS Toulose, BER Berlin Brandenburg, NRN Dusseldorf Weeze, HAM Hamburg, CFU Corfu, RHO Rhodes, BUD Budapest, ORK Cork, DUB Dublin, NOC Knock, SNN Shannon
        //BRI Bari, BLQ Bologna, BGY Milan Bergamo, NAP Naples, PMO Palermo, PSA Pisa, CIA Rome Ciampino, TRN Turin, VCE Venice M.Polo, VRN Verona, RIX Riga, KUN Kaunas, MLA Malta, RAK Marrakesh, EIN Eindhoven
        //GDN Gdansk, FAO Faro, LIS Lisbon, OPO Porto, OTP Bucharest, BTS Bratislava, FUE Fuerteventura, LPA Gran Canaria, IBZ Ibiza, ACE Lanzarote, MAD Madrid, AGP Malaga, PMI Palma de Mallorca, SDR Santander
        //SCQ Santiago, SVQ Seville, TFS Tenerife South, VLC Valencia, GOT Goteborg Landvetter, BFS Belfast International, BOH Bournemouth, STN London Stansted, NQY Newquay Cornwall

        List<Airport> DepartureAirports = new List<Airport>();
        string departureDate = "2023-07";

        if (!Directory.Exists(folderPath))
        {
            // Create directory if it doesn't exist
            Directory.CreateDirectory(folderPath);
        }

        // Fetch all text files from the directory
        var files = Directory.GetFiles(folderPath, "*.txt");

        if (files.Length == 0)
        {
            // If no files exist, create default files
            CreateDefaultAirportFiles(folderPath);
            files = Directory.GetFiles(folderPath, "*.txt");
        }

        // Read airport data from each file
        foreach (var file in files)
        {
            var lines = System.IO.File.ReadLines(file);
            foreach (var line in lines)
            {
                var values = line.Split(',');
                if (values.Length < 2)
                {
                    continue; // skip if line is not valid
                }

                var airport = new Airport(values[0], values[1]);
                airport.AddDestinations(values.Skip(2).ToArray()); // Add all destinations
                DepartureAirports.Add(airport);
            }
        }

        //Path to your ChromeDriver executable
        string driverPath = @"C:\path\to\chromedriver";

        //ChromeOptions object to configure ChromeDriver
        ChromeOptions options = new ChromeOptions();
        //Launches the browser in headless mode (without GUI)
        options.AddArgument("--headless");
        //Suppress INFO and WARNING console messages
        options.AddArgument("--log-level=3");

        using (IWebDriver driver = new ChromeDriver(driverPath, options))
        {
            foreach (Airport airport in DepartureAirports)
            {
                flightDataFetcher.FetchFlightData(driver, departureDate, airport.Code, airport.Destinations, "1");
                flightDataFetcher.FetchReturnFlightData(driver, departureDate, airport.Code, airport.Destinations, "1");
            }
        }
    }

    private void CreateDefaultAirportFiles(string folderPath)
    {
        //GLA Airport file
        using (StreamWriter writer = System.IO.File.CreateText(Path.Combine(folderPath, "GLA.txt")))
        {
            writer.WriteLine("GLW,Glasgow,CRL,DUB,KRK,WMI,WRO,ALC,AGP");
        }

        //PIK Airport file
        using (StreamWriter writer = System.IO.File.CreateText(Path.Combine(folderPath, "GLA.txt")))
        {
            writer.WriteLine("GLW,Glasgow Prestwick,LOCATIONS LIST HERE");
        }

        //EDI Airport file
        using (StreamWriter writer = System.IO.File.CreateText(Path.Combine(folderPath, "EDI.txt")))
        {
            writer.WriteLine("EDI,Edinburgh Airport,ALC,BCN,VIE,CRL,SOF,ZAD,PRG,BLL,CPH,BZR,BOD,CCF,GNB,MRS,NTE,FNI,BVA,PIS,TLS,BER,NRN,HAM,CFU,RHO,BUD,ORK,DUB,NOC,SNN,BRI,BLQ,BGY,NAP,PMO,PSA,CIA,TRN,VCE,VRN,RIX,KUN,MLA,RAK,EIN,GDN,FAO,LIS,OPO,OTP,BTS,FUE,LPA,IBZ,ACE,MAD,AGP,PMI,SDR,SCQ,SVQ,TFS,VLC,GOT,BFS,BOH,STN,NQY");
        }
    }
}
