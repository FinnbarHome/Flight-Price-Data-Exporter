
# 🛫 Cheapest Flights Scraper

A **C#** console application that scrapes **Ryanair** flight data using **Selenium** and **HtmlAgilityPack**. This tool automatically retrieves the cheapest flights between selected departure and destination airports over a specified date range.

## ✈️ Features
- **Scrapes flight prices from Ryanair** for specified routes and dates.
- **Uses Selenium WebDriver** to automate browser navigation.
- **Parses HTML with HtmlAgilityPack** for structured data extraction.
- **Outputs flight details** (departure time, price, etc.) in the console.

## ⚡ Technologies Used
- **C# (.NET 8)**
- **Selenium WebDriver** (for browser automation)
- **HtmlAgilityPack** (for HTML parsing)
- **WebDriverManager** (for auto-installing the correct ChromeDriver)


## 🛠️ Installation

### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/yourusername/CheapestFlightsScraper.git
cd CheapestFlightsScraper
```

### **2️⃣ Install Dependencies**
Ensure you have **.NET 8 SDK** installed. Then, restore dependencies:
```sh
dotnet restore
```

### **3️⃣ Install Required NuGet Packages**
Run the following command to ensure all required packages are installed:
```sh
dotnet add package Selenium.WebDriver
dotnet add package Selenium.WebDriver.ChromeDriver
dotnet add package HtmlAgilityPack
dotnet add package WebDriverManager
```

### **4️⃣ Update Chrome & ChromeDriver**
The program requires an up-to-date version of **Google Chrome** and **ChromeDriver**. Ensure that:
- **Google Chrome is installed and updated.**
- The correct **ChromeDriver** version is installed via WebDriverManager.

## 🚀 Usage

### **Run the Program**
```sh
dotnet run
```

### **Modify the Search Parameters**
To customize your search:
1. Open `FlightState.cs`.
2. Modify the following lists to add or remove airports:
   ```csharp
   private List<string> startLocs = new List<string> { "EDI" }; // Departure airports
   private List<string> endLocs = new List<string> { "ALC" };   // Destination airports
   ```
3. Update the **date range**:
   ```csharp
   private string date = "2025-05"; // Format: YYYY-MM for a full month or YYYY-MM-DD for a specific date
   ```
4. Save the changes and re-run the program.

## 📊 Output Format
After running the program, flight data will be displayed in the console:
```
Flights from EDI:
To ALC:
Departing at 10:30 on 2025-05-15 is priced at €49.99
Departing at 14:00 on 2025-05-16 is priced at €39.99
```
Flight details include:
- **Departure Airport**
- **Destination Airport**
- **Time of Flight**
- **Date**
- **Price**


## 🔧 Troubleshooting

### **1️⃣ Getting `InvalidSelectorException`?**
**Issue:**  
```
OpenQA.Selenium.InvalidSelectorException: 'invalid selector: Unable to locate an element with the xpath expression ...'
```
**Solution:**
- Ensure the **XPath values in `FlightScraper.cs`** match the current Ryanair website.
- Use **Chrome DevTools** to inspect elements and update the XPath.

### **2️⃣ `WebDriverTimeoutException` Errors?**
**Issue:** The program times out when loading flights.  
**Solution:**
- Increase the wait time in `FetchPageSource()`:
  ```csharp
  WaitForElement(driver, TimeXPath, 5); // Increase wait time to 5 seconds
  ```
- Ensure your **internet connection is stable**.
- Run the program **without headless mode** to debug:
  ```csharp
  options.AddArgument("--headless"); // REMOVE THIS LINE in FlightState.cs
  ```

### **3️⃣ `ChromeDriver only supports Chrome version X`?**
**Issue:**  
```
session not created: This version of ChromeDriver only supports Chrome version 133
Current browser version is 132.0.6834.197
```
**Solution:**
1. Update **Google Chrome** to match your ChromeDriver version.
2. Manually install the correct ChromeDriver:  
   - Go to: [https://sites.google.com/chromium.org/driver/](https://sites.google.com/chromium.org/driver/)
   - Download the version matching your Chrome browser.

### **4️⃣ The program isn't finding flights**
**Issue:** The program runs successfully but doesn't find flights.  
**Solution:**
- Check if Ryanair **changed their website structure**.
- Manually search for a flight on Ryanair and **compare the data**.
- Ensure the search parameters (dates, airports) **actually have flights available**.
